using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aware.Search;

namespace Aware.Data
{
    public interface IRepository<T> : IDisposable where T : class
    {
        T Get(int id);
        T First(Expression<Func<T, bool>> filter,bool last=false);

        List<T> GetAll();
        SearchResult<T> Find(ISearchParams<T> searchParams);
        SearchHelper<T> Where(Expression<Func<T, bool>> filter);
        List<T> Find(SearchHelper<T> searchHelper);
        int GetCount(List<Expression<Func<T, bool>>> filters);
        List<T> GetWithSql(string query, params object[] parameters);
        List<TX> GetWithSql<TX>(string query, params object[] parameters);

        ICriteriaHelper CriteriaHelper { get; }
        List<T> GetWithCriteria(ICriteriaHelper criteriaHelper);
        List<TX> GetWithCriteria<TX>(ICriteriaHelper criteriaHelper) where TX : class;
       
        //DML Statements
        void Add(T entity, bool save = true);
        bool Delete(int id);
        bool Delete(T entity);
        bool Update(T entity, bool save = true);
        void Save();
        int ExecuteSp(string storedProcedureQuery);
        
        Guid StartTransaction();
        bool Commit(Guid transactionGuid);
        bool Rollback(Guid transactionGuid);
    }
}
