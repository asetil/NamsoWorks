using System;
using System.Linq.Expressions;
using Worchart.BL.Model;
using Worchart.BL.Search;

namespace Worchart.BL.Manager
{
    public interface IBaseManager<T> where T : IEntity
    {
        T Get(int id);
        T First(Expression<Func<T, bool>> filter = null, T defaultValue = default(T));

        SearchResult<T> SearchBy(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 0);
        SearchResult<T> Search(SearchParams<T> searchParams = null);

        OperationResult Save(T model);
        OperationResult Clone(int id, bool create = false);
        OperationResult Delete(int id);
    }
}