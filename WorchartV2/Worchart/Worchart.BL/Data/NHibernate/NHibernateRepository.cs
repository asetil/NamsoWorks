using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Worchart.Search;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;
using Worchart.BL.Model;
using Worchart.BL.Search;
using Worchart.BL.Log;
using Worchart.BL;

namespace Worchart.Data.NHibernate
{
    public class NHibernateRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public NHibernateRepository(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public T Get(int id)
        {
            var transactionGuid = Guid.Empty;
            try
            {
                transactionGuid = StartTransaction();
                var criteria = Session.CreateCriteria<T>();
                criteria.Add(Restrictions.Eq("ID", id));
                var result = criteria.UniqueResult<T>();
                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Get", ex);
                Rollback(transactionGuid);
            }
            return default(T);
        }

        public T First(Expression<Func<T, bool>> filter, bool last = false)
        {
            var transactionGuid = StartTransaction();
            try
            {
                var query = Session.Query<T>().Where(filter);
                if (!last)
                {
                    query = query.Take(1);
                }
                var list = query.ToList();

                Commit(transactionGuid);
                return last ? list.LastOrDefault() : list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|First", ex);
                Rollback(transactionGuid);
            }
            return null;
        }

        public List<T> GetAll()
        {
            var transactionGuid = Guid.Empty;
            try
            {
                transactionGuid = StartTransaction();
                var result = Session.Query<T>().ToList();
                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|GetAll", ex);
                Rollback(transactionGuid);
            }
            return null;
        }
        public SearchResult<T> Find(ISearchParams<T> searchParams)
        {
            var transactionGuid = Guid.Empty;
            try
            {
                if (searchParams != null)
                {
                    transactionGuid = StartTransaction();
                    var query = Session.Query<T>();
                    var searchHelper = searchParams.PrepareFilters();

                    if (searchHelper.FilterList != null && searchHelper.FilterList.Any())
                    {
                        foreach (var filter in searchHelper.FilterList)
                        {
                            query = query.Where(filter);
                        }
                    }

                    if (!string.IsNullOrEmpty(searchParams.Fields))
                    {
                        foreach (var field in searchParams.Fields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            //TODO ?
                            //query = query.Include(field);
                        }
                    }

                    if (searchHelper.SortList != null && searchHelper.SortList.Any())
                    {
                        foreach (var sorter in searchHelper.SortList)
                        {
                            sorter.OrderBy(ref query);
                        }
                        //query = sortList.Aggregate(query, (current, sorter) => sorter.IsDescending ? current.OrderByDescending(sorter.OnField2) : current.OrderBy(sorter.OnField2));
                    }

                    if (searchParams.Size > 0)
                    {
                        query = query.Skip(searchParams.Skip).Take(searchParams.Size);
                    }

                    var list = query.ToList();
                    var count = searchParams.IncludeCount ? GetCount(searchHelper.FilterList) : 0;
                    Commit(transactionGuid);

                    var result = new SearchResult<T>
                    {
                        Results = list,
                        SearchParams = searchParams,
                        TotalSize = count,
                        Success = true
                    };
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Find", ex);
                Rollback(transactionGuid);
            }
            return null;
        }

        public SearchHelper<T> Where(Expression<Func<T, bool>> filter)
        {
            var searchHelper = new SearchHelper<T>();
            searchHelper.SetRepository(this);
            return searchHelper.FilterBy(filter);
        }

        public List<T> Find(SearchHelper<T> searchHelper)
        {
            if (searchHelper == null || searchHelper.FilterList == null)
            {
                return new List<T>();
            }

            var transactionGuid = StartTransaction();
            try
            {
                var query = Session.Query<T>();
                foreach (var filter in searchHelper.FilterList)
                {
                    query = query.Where(filter);
                }

                if (searchHelper.SortList != null && searchHelper.SortList.Any())
                {
                    foreach (var sorter in searchHelper.SortList)
                    {
                        sorter.OrderBy(ref query);
                    }
                    //query = sortList.Aggregate(query, (current, sorter) => sorter.IsDescending ? current.OrderByDescending(sorter.OnField2) : current.OrderBy(sorter.OnField2));
                }

                if (searchHelper.Size > 0)
                {
                    query = query.Skip(searchHelper.Skip).Take(searchHelper.Size);
                }

                var result = query.ToList();
                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Find", ex);
                Rollback(transactionGuid);
                throw;
            }
        }

        public int GetCount(List<Expression<Func<T, bool>>> filters)
        {
            var transactionGuid = Guid.Empty;
            try
            {
                transactionGuid = StartTransaction();
                Expression<Func<T, bool>> predicate = (p => p != null);
                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        predicate = predicate.Combine(filter, System.Linq.Expressions.Expression.AndAlso);
                    }
                }

                var result = Session.Query<T>().Where(predicate).Count();
                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|GetCount", ex);
                Rollback(transactionGuid);
                throw;
            }
        }

        public List<T> GetWithSql(string query, params object[] parameters)
        {
            return GetWithSql<T>(query, parameters);
        }

        public List<TX> GetWithSql<TX>(string query, params object[] parameters)
        {
            var transactionGuid = StartTransaction();
            try
            {
                List<TX> result;
                if (typeof(TX).IsPrimitive)
                {
                    result = Session.CreateSQLQuery(query).List<TX>().ToList();
                }
                else
                {
                    result = Session.CreateSQLQuery(query)  //.AddEntity(typeof(TX))
                                    .SetResultTransformer(Transformers.AliasToBean(typeof(TX)))
                                    .List<TX>().ToList();
                }

                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|GetWithSql", ex);
                Rollback(transactionGuid);
                throw;
            }
        }

        public List<T> GetWithCriteria(ICriteriaHelper criteriaHelper)
        {
            return GetWithCriteria<T>(criteriaHelper);
        }

        public List<TX> GetWithCriteria<TX>(ICriteriaHelper criteriaHelper) where TX : class
        {
            var transactionGuid = StartTransaction();
            try
            {
                List<TX> result = null;
                var criteria = Session.CreateCriteria<TX>();
                criteriaHelper.Build(ref criteria);

                if (criteriaHelper.LoadCount)
                {
                    // 1) we don’t care about how the query is ordered if we’re just getting the total row count; and 
                    // 2) the order statements can cause problems if there are groupings in the criteria. Trust me on this.
                    var countCrit = (ICriteria)criteria.Clone();
                    countCrit.ClearOrders();

                    var multiCriteria = Session.CreateMultiCriteria()
                        .Add(criteria)
                        .Add(countCrit.SetProjection(Projections.RowCount())); //Projections.RowCountInt64() for long

                    var multiResults = multiCriteria.List();
                    if (multiResults != null && multiResults.Any())
                    {
                        result = (List<TX>)multiResults[0];
                        criteriaHelper.Count = (int)((IList)multiResults[1])[0];
                    }
                }
                else
                {
                    result = criteria.List<TX>().ToList();
                }

                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|GetWithCriteria", ex);
                Rollback(transactionGuid);
                throw;
            }
        }

        public void Add(T entity, bool save = true)
        {
            var transactionGuid = StartTransaction();
            try
            {
                Session.SaveOrUpdate(entity);
                Commit(transactionGuid);
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Add", ex);
                Rollback(transactionGuid);
                throw ex;
            }
        }

        public bool Delete(int id)
        {
            var entity = Get(id);
            if (entity == null)
            {
                throw new Exception(string.Format("Entity of type {0} with id:{1} not found!", typeof(T).Name, id));
            }

            return Delete(entity);
        }

        public bool Delete(T entity)
        {
            var transactionGuid = StartTransaction();
            try
            {
                Session.Delete(entity);
                Commit(transactionGuid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Delete", ex);
                Rollback(transactionGuid);
                throw ex;
            }
        }

        public bool Update(T entity, bool save = true)
        {
            var transactionGuid = StartTransaction();
            try
            {
                Session.SaveOrUpdate(entity);
                Commit(transactionGuid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|Update", ex);
                Rollback(transactionGuid);
                throw ex;
            }
        }

        public void Save()
        {
            //Nothing to save
        }

        public int ExecuteSp(string storedProcedureQuery)
        {
            var transactionGuid = StartTransaction();
            try
            {
                var result = Session.CreateSQLQuery(storedProcedureQuery).ExecuteUpdate();
                Commit(transactionGuid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("NHibernateRepository|ExecuteSp", ex);
                Rollback(transactionGuid);
                throw ex;
            }
        }

        public ICriteriaHelper CriteriaHelper
        {
            get
            {
                return new CriteriaHelper();
            }
        }

        public Guid StartTransaction()
        {
            return _unitOfWork.StartTransaction();
        }

        public bool Commit(Guid transactionGuid)
        {
            return _unitOfWork.Commit(transactionGuid);
        }

        public bool Rollback(Guid transactionGuid)
        {
            return _unitOfWork.Rollback(transactionGuid);
        }

        private ISession Session
        {
            get
            {
                return _unitOfWork.GetCurrentSession();
            }
        }

        public void Dispose()
        {

        }
    }
}
