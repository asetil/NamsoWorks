using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Model;

namespace Aware.Util
{
    public interface IBaseService<T> where T : class, IEntity
    {
        T Get(int id);
        List<T> GetAll(int page=1,int pageSize=20);
        SearchResult<T> Search(SearchParams<T> searchParams);
        Result Save(T model);
        Result Delete(int id);
    }
}