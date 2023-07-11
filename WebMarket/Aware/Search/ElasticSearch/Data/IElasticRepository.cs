using System.Collections.Generic;
using Aware.Util;
using Nest;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch.Data
{
    public interface IElasticRepository
    {
        ElasticStatus Status { get; }
        bool CreateIndex(string indexName);
        bool DeleteIndex(string indexName, bool deleteAll = false);
        bool IndexExists(string indexName);
        T Get<T>(int id) where T : class;
        IEnumerable<T> GetAll<T>() where T : class;
        IEnumerable<T> GetMany<T>(IEnumerable<int> idList) where T : class;
        IIndexResponse Index<T>(T entity) where T : class;
        IBulkResponse InsertMany<T>(IEnumerable<T> entityList, string indexName, int bulkSize = 1000, bool isBulk = false) where T : class;
        IUpdateResponse<T> Update<T>(T entity) where T : class;
        IDeleteResponse Delete<T>(int id) where T : class;
        IDeleteResponse Delete<T>(T entity) where T : class;
        IBulkResponse DeleteMany<T>(IEnumerable<T> entityList) where T : class;
        void DeleteAll<T>() where T:class;
        ISearchResponse<T> Find<T>(ISearchRequest searchDescriptor) where T : class;
        void Refresh(string indexName);
    }
}
