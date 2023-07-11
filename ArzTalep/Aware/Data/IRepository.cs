using Aware.Model;
using Aware.Search;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aware.Data
{
    public interface IRepository<T> : IDisposable where T : IEntity
    {
        T Get(int id);
        T First(Expression<Func<T, bool>> filter, bool last = false);

        List<T> GetAll();
        SearchResult<T> Find(ISearchParams<T> searchParams);

        int GetCount(List<Expression<Func<T, bool>>> filters);
        List<T> GetWithSql(string query, params object[] parameters);
        List<TX> GetWithSql<TX>(string query, params object[] parameters);

        //DML Statements
        void Add(T entity, bool save = true);
        bool Delete(int id);
        bool Delete(T entity);
        bool Update(T entity, bool save = true);
        void Save();

        int ExecuteSp(string storedProcedureQuery);
    }
}
