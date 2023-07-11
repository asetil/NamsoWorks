using Aware.Model;
using Aware.Search;
using Aware.Util;
using Aware.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aware.Data.Fake
{
    public class FakeRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IFakeDataProvider _fakeDataProvider;
        private readonly IAwareLogger _logger;
        private readonly List<T> _list;
        public FakeRepository(IFakeDataProvider fakeDataProvider, IAwareLogger logger)
        {
            _fakeDataProvider = fakeDataProvider;
            _logger = logger;
            _list = GetList();
        }

        public T Get(int id)
        {
            if (id > 0)
            {
                return Query.FirstOrDefault(i => (int)i.GetType().GetProperty("ID").GetValue(i) == id);
            }
            return default(T);
        }

        public T First(Expression<Func<T, bool>> filter, bool last = false)
        {
            try
            {
                var query = Query.Where(filter);
                var list = query.ToList();
                return last ? list.LastOrDefault() : list.FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        public List<T> GetAll()
        {
            return Query.ToList();
        }

        public List<T> GetWithSql(string query, params object[] parameters)
        {
            return Query.Take(5).ToList();
        }

        public List<TX> GetWithSql<TX>(string query, params object[] parameters)
        {
            if (typeof(TX).Name == "Int32")
            {
                return new List<int>() { 5 } as List<TX>;
            }

            var list = _fakeDataProvider.GetFakeData<TX>().Take(3).ToList();
            return list;
        }

        public int GetCount(List<Expression<Func<T, bool>>> filters)
        {

            Expression<Func<T, bool>> predicate = (p => p != null);
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    predicate = predicate.Combine(filter, Expression.AndAlso);
                }
            }

            var result = Query.Where(predicate);
            return result.Count();
        }

        public SearchResult<T> Find(ISearchParams<T> searchParams)
        {
            var result = new SearchResult<T>();
            try
            {
                if (searchParams != null)
                {
                    var query = Query;
                    if (searchParams.FilterList != null && searchParams.FilterList.Any())
                    {
                        foreach (var filter in searchParams.FilterList)
                        {
                            query = query.Where(filter);
                        }
                    }

                    //if (!string.IsNullOrEmpty(searchParams.Fields))
                    //{
                    //    foreach (var field in searchParams.Fields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    //    {
                    //        //query = query.Include(field);
                    //    }
                    //}

                    if (searchParams.SortList != null && searchParams.SortList.Any())
                    {
                        foreach (var sorter in searchParams.SortList)
                        {
                            sorter.OrderBy(ref query);
                        }
                    }

                    if (searchParams.Size > 0)
                    {
                        query = query.Skip((searchParams.Page - 1) * searchParams.Size).Take(searchParams.Size);
                    }

                    result.Results = query.ToList();
                    result.TotalSize = GetCount(searchParams.FilterList);
                    result.Success = true;
                    result.SearchParams = searchParams;
                }
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public void Add(T entity, bool save = true)
        {
            var id = (int)entity.GetType().GetProperty("ID").GetValue(entity);
            if (id == 0)
            {
                var newID = new Random().Next(1000, 99999);
                entity.GetType().GetProperty("ID").SetValue(entity, newID);
            }
            _list.Add(entity);
        }

        public bool Delete(int id)
        {
            var item = Get(id);
            return Delete(item);
        }

        public bool Delete(T entity)
        {
            if (entity != null)
            {
                var id = entity.GetType().GetProperty("ID").GetValue(entity).ToString();
                T foundItem = default(T);
                foreach (var listItem in _list)
                {
                    var itemID = listItem.GetType().GetProperty("ID").GetValue(listItem).ToString();
                    if (itemID == id)
                    {
                        foundItem = listItem;
                    }
                }

                if (foundItem != null)
                {
                    _list.Remove(foundItem);
                    return true;
                }
            }
            return false;
        }

        public bool Update(T entity, bool save = true)
        {
            Delete(entity);
            Add(entity, save);
            return true;
        }

        public void Save()
        {

        }

        public int ExecuteSp(string storedProcedureQuery)
        {
            return 1;
        }

        public Guid StartTransaction()
        {
            return Guid.Empty;
        }

        public bool Commit(Guid transactionGuid)
        {
            return true;
        }

        public bool Rollback(Guid transactionGuid)
        {
            return true;
        }

        private IQueryable<T> OrderBy(IQueryable<T> query, List<Sorter<T>> sortList)
        {
            if (sortList != null && sortList.Any())
            {
                foreach (var sorter in sortList)
                {
                    if (sorter.ResultType == typeof(string))
                    {
                        var onfield = sorter.DynamicOnField as Expression<Func<T, string>>;
                        query = sorter.Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
                    }
                    else if (sorter.ResultType == typeof(int))
                    {
                        var onfield = sorter.DynamicOnField as Expression<Func<T, int>>;
                        query = sorter.Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
                    }
                    else if (sorter.ResultType == typeof(DateTime))
                    {
                        var onfield = sorter.DynamicOnField as Expression<Func<T, DateTime>>;
                        query = sorter.Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
                    }
                }
            }
            return query;
        }

        public void Dispose()
        {
        }

        private IQueryable<T> Query
        {
            get { return new EnumerableQuery<T>(_list); }
        }

        private List<T> GetList()
        {
            var list = _fakeDataProvider.GetFakeData<T>().ToList();
            return list;
        }
    }
}
