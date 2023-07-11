using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nest;

namespace Aware.Search.ElasticSearch.Model
{
    public class AggregationField<T> where T : class
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }

        public Expression<Func<T, object>> OnField { get; set; }

        public int Size { get; set; }

        public TermsOrder Order { get; set; }

        public string Include { get; set; }

        public string Exclude { get; set; }

        public List<OFilterContainer> FilterList { get; set; }

        public AgregationType Type { get; set; }
        public AgregationMapType MapType { get; set; }

        public string Interval { get; set; }

        public string Format { get; set; }

        public List<AggregationField<T>> ChildAggregations { get; set; }

        public List<RangeField> RangeList { get; set; }

        public bool IsNested { get; set; }
        public string NestedPath { get; set; }
        public bool WithReverseNested { get; set; }
    }

    public class RangeField
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public double From { get; set; }
        public double To { get; set; }
    }

    public enum AgregationType
    {
        Term = 0,
        Sum = 1,
        Min = 2,
        Max = 3,
        Avg = 4,
        DateHistogram = 5,
        Range = 6,
        Count=7,
        Cardinality=8
    }

    public enum AgregationMapType
    {
        None=0,
        Category=1,
        Store=2,
        Property=3,
        PropertyValue=4,
        Price=5,
        Stock=6,
        CommentRating=7
    }

    public enum IntervalType
    {
        Month=0,
        Day=1,
        Week=2,
        Year=3
    }
}
