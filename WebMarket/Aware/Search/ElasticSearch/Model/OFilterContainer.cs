using Nest;

namespace Aware.Search.ElasticSearch.Model
{
    public class OFilterContainer
    {
        public string Name { get; set; }
        public QueryContainer FilterContainer { get; set; }
    }
}
