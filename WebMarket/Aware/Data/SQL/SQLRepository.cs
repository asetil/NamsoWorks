using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aware.Search;

namespace Aware.Data.SQL
{
    public class SqlRepository<T> : IRepository<T> where T : class
    {
        private readonly ISqlHelper _sqlHelper;

        public SqlRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public T Get(int id)
        {
            var sql = _sqlHelper.Query.Table<T>().Where("ID=" + id).ToString();
            var dataRow=_sqlHelper.GetDataRow(sql);
            return default(T);
        }

        public T First(Expression<Func<T, bool>> filter, bool last = false)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<T> GetWithSql(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public List<TX> GetWithSql<TX>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public List<T> GetWithCriteria(ICriteriaHelper criteriaHelper)
        {
            throw new NotImplementedException();
        }

        public List<TX> GetWithCriteria<TX>(ICriteriaHelper criteriaHelper) where TX : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetWithSql(string query, bool inContext, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TX> GetWithSql<TX>(string query, bool inContext, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int GetCount(List<Expression<Func<T, bool>>> filters)
        {
            throw new NotImplementedException();
        }

        public SearchResult<T> Find(ISearchParams<T> searchParams)
        {
            throw new NotImplementedException();
        }

        public SearchHelper<T> Where(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<T> Find(SearchHelper<T> searchHelper)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity, bool save = true)
        {
            throw new NotImplementedException();
        }

        bool IRepository<T>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        bool IRepository<T>.Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity, bool save = true)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public int ExecuteSp(string storedProcedureQuery)
        {
            throw new NotImplementedException();
        }

        public ICriteriaHelper CriteriaHelper { get; private set; }
        Guid IRepository<T>.StartTransaction()
        {
            throw new NotImplementedException();
        }

        public bool Commit(Guid transactionGuid)
        {
            throw new NotImplementedException();
        }

        public bool Rollback(Guid transactionGuid)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
