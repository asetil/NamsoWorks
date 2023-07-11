using System.Collections.Generic;

namespace Aware.Search
{
    public class AggregationItem
    {
        public string Term { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public long Count { get; set; }
        public long HistogramKey { get; set; }
        public List<AggregationResult> Childs { get; set; } 
    }
}
