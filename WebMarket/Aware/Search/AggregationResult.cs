using System.Collections.Generic;
using Aware.Search.ElasticSearch.Model;

namespace Aware.Search
{
    public class AggregationResult
    {
        public string Name { get; set; }
        public string SearchName { get; set; }
        public AgregationMapType MapType { get; set; }
        public List<AggregationItem> Items { get; set; }
    }
}