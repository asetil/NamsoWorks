using System.Collections.Generic;
using Nest;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch
{
    public interface IElasticService
    {
        bool IsActive { get;}
        ElasticStatus Status { get; }
        bool IndexExist(string indexName);
        void CreateIndex(string indexName);
        void DeleteIndex(string indexName,bool deleteAll=false);
        T Get<T>(int id) where T:class;
        IEnumerable<T> GetAll<T>() where T:class;
        IEnumerable<T> GetMany<T>(IEnumerable<int> idList) where T : class;
        void Add<T>(T item) where T : class;
        void DeleteAll<T>() where T : class;
        IBulkResponse InsertMany<T>(List<T> itemList, string indexName, int bulkSize = 1000, bool isBulk = false) where T : class;
        ISearchResponse<T> Find<T>(ISearchRequest searchDescriptor) where T : class;
        SearchResult<T> Search<T>(IElasticBuilder  elasticBuilder) where T : class;
    }
}