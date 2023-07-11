using System.Collections.Generic;
using System.Linq;
using Aware.Search.ElasticSearch.Model;

namespace Aware.Search
{
    public class SearchResult<T> where T : class
    {
        public ISearchParams<T> SearchParams { get; set; }
        public List<T> Results { get; set; }
        public List<AggregationResult> Aggregations { get; set; }
        public bool Success { get; set; }
        public long TotalSize { get; set; }
        public int Took { get; set; }
        public string RequestBody { get; set; }

        public bool HasResult
        {
            get
            {
                return Results != null && Results.Any();
            }
        }

        public virtual bool HasMore
        {
            get
            {
                return HasResult && SearchParams != null && TotalSize > ((SearchParams.Page + 1) * SearchParams.Size);
            }
        }

        public bool HasAggregation
        {
            get
            {
                return Aggregations != null && Aggregations.Any();
            }
        }

        protected List<AggregationItem> GetAggregationItems(List<AggregationResult> aggregations, AgregationMapType mapType)
        {
            var item = aggregations.FirstOrDefault(i => i.MapType == mapType);
            if (item != null && item.Items != null)
            {
                return item.Items;
            }
            return new List<AggregationItem>();
        }

        protected int GetAggregationCount(List<AggregationItem> items, string term)
        {
            var item = items.FirstOrDefault(i => i.Term == term);
            return item != null ? (int)item.Count : 0;
        }
    }
}