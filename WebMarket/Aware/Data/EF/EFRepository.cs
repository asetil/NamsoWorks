using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Aware.Dependency;
using Aware.Search;
using Aware.Util.Log;

namespace Aware.Data.EF
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly ILogger _logger;
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        private IEFUnitOfWork _unitOfWork;

        public EFRepository(ILogger logger)
        {
            _logger = logger;
            _context = UnitOfWork.Context;
            _dbSet = UnitOfWork.Context.Set<T>();
        }

        protected IEFUnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (_unitOfWork = (IEFUnitOfWork)WindsorBootstrapper.Resolve<IUnitOfWork>()); }
        }

        public T Get(int id)
        {
            return _dbSet.Find(id);
        }

        public T First(Expression<Func<T, bool>> filter, bool last = false)
        {
            try
            {
                if (filter != null)
                {
                    IQueryable<T> query = _dbSet;
                    query = query.Where(filter);

                    if (!last)
                    {
                        query = query.Take(1);
                    }

                    var list = query.ToList();
                    return last?list.LastOrDefault():list.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        public List<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public List<T> GetWithSql(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).ToList();
        }

        public List<TX> GetWithSql<TX>(string query, params object[] parameters)
        {
            return _context.Database.SqlQuery<TX>(query).ToList();
        }

        public List<T> GetWithCriteria(ICriteriaHelper criteriaHelper)
        {
            throw new NotSupportedException("Entity Framework does not support ICriteria!");
        }

        public List<TX> GetWithCriteria<TX>(ICriteriaHelper criteriaHelper) where TX : class
        {
            throw new NotSupportedException("Entity Framework does not support ICriteria!");
        }

        public virtual int ExecuteSp(string storedProcedureQuery)
        {
            return _context.Database.ExecuteSqlCommand(storedProcedureQuery);
        }

        public ICriteriaHelper CriteriaHelper
        {
            get
            {
                throw new NotSupportedException("Entity Framework does not support ICriteria!");
            }
        }

        public SearchResult<T> Find(ISearchParams<T> searchParams)
        {
            var result = new SearchResult<T>();
            IQueryable<T> query = _dbSet;
            if (searchParams != null)
            {
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
                        query = query.Include(field);
                    }
                }

                if (searchHelper.SortList != null && searchHelper.SortList.Any())
                {
                    foreach (var sorter in searchHelper.SortList)
                    {
                        sorter.OrderBy(ref query);
                    }
                }

                if (searchParams.Size > 0)
                {
                    query = query.Skip((searchParams.Page - 1) * searchParams.Size).Take(searchParams.Size);
                }

                result.Results = query.ToList();
                result.TotalSize = GetCount(searchHelper.FilterList);
                result.Success = true;
                result.SearchParams = searchParams;
            }
            return result;
        }

        public SearchHelper<T> Where(Expression<Func<T, bool>> filter)
        {
            var searchHelper = new SearchHelper<T>();
            searchHelper.SetRepository(this);
            return searchHelper.FilterBy(filter);
        }

        public List<T> Find(SearchHelper<T> searchHelper)
        {
            try
            {
                if (searchHelper != null && searchHelper.FilterList != null)
                {
                    IQueryable<T> query = _dbSet;
                    foreach (var filter in searchHelper.FilterList)
                    {
                        query = query.Where(filter);
                    }

                    //if (!string.IsNullOrEmpty(relatedFields))
                    //{
                    //    foreach (var field in relatedFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    //    {
                    //        query = query.Include(field);
                    //    }
                    //}

                    if (searchHelper.SortList != null && searchHelper.SortList.Any())
                    {
                        foreach (var sorter in searchHelper.SortList)
                        {
                            sorter.OrderBy(ref query);
                        }
                        //query = orderBy.Aggregate(query, (current, sorter) => sorter.Descending ? current.OrderByDescending(sorter.OnField) : current.OrderBy(sorter.OnField));
                    }

                    //if (fieldSelector != null)
                    //{
                    //    return query.Select(fieldSelector).ToList();
                    //}

                    if (searchHelper.Size > 0)
                    {
                        query = query.Skip(searchHelper.Skip).Take(searchHelper.Size);
                    }
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EFRepository > Find - failed",ex);
            }
            return new List<T>();
        }

        public void Add(T entity, bool save = true)
        {
            _dbSet.Add(entity);

            if (save)
            {
                Save();
            }
        }

        public bool Delete(int id)
        {
            var entity = _dbSet.Find(id);
            return Delete(entity);
        }

        public bool Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            return true;
        }

        public bool Update(T entity, bool save = true)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _context.Entry(entity).State = EntityState.Modified;

            if (save)
            {
                Save();
            }
            return true;
        }

        public int GetCount(List<Expression<Func<T, bool>>> filters)
        {
            IQueryable<T> query = _dbSet;
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            return query.Count();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Guid StartTransaction()
        {
            return UnitOfWork.StartTransaction();
        }

        public bool Commit(Guid transactionGuid)
        {
            return UnitOfWork.Commit(transactionGuid);
        }

        public bool Rollback(Guid transactionGuid)
        {
            return UnitOfWork.Rollback(transactionGuid);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}