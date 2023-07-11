using Aware.Model;
using Aware.Search;
using System;
using System.Linq.Expressions;
using Aware.BL.Model;

namespace Aware.Manager
{
    public interface IBaseManager<T> where T : BaseEntity
    {
        T Get(int id);
        T First(Expression<Func<T, bool>> filter = null, T defaultValue = default(T));

        SearchResult<T> SearchBy(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 0);
        SearchResult<T> Search(SearchParams<T> searchParams = null);

        OperationResult<T> Save(T model);
        OperationResult<T> Clone(int id, bool create = false);
        OperationResult<T> Delete(int id);
    }
}