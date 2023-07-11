using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Search.ElasticSearch.Model;
using Nest;

namespace Aware.Search.ElasticSearch
{
    public class AggregationHelper<T> where T : class
    {
        public AggregationHelper(ElasticHelper<T> helper)
        {
            Helper = helper;
        }

        public AggregationField<T> Aggregation { get; private set; }
        public ElasticHelper<T> Helper { get; private set; }

        public AggregationHelper<T> Init(Expression<Func<T, object>> onField, int size,AgregationType type= AgregationType.Term)
        {
            if (onField!=null)
            {
                Aggregation = new AggregationField<T>()
                {
                    OnField = onField,
                    Size = size,
                    Type = type
                };
            }
            return this;
        }

        public AggregationHelper<T> SetName(string name, AgregationMapType mapType, string searchName="")
        {
            if (Aggregation != null)
            {
                Aggregation.DisplayName = name;
                Aggregation.MapType = mapType;
                Aggregation.Name = searchName;
            }
            return this;
        }

        public AggregationHelper<T> SetInclude(string include, string exclude = "")
        {
            if (Aggregation != null)
            {
                Aggregation.Include = include;
                Aggregation.Exclude = exclude;
            }
            return this;
        }

        public AggregationHelper<T> SetNested(string path,bool withReverseNested=true)
        {
            if (Aggregation != null)
            {
                Aggregation.IsNested = true;
                Aggregation.NestedPath = path;
                Aggregation.WithReverseNested = withReverseNested;
            }
            return this;
        }

        public AggregationHelper<T> AddChild(AggregationField<T> childAggregation)
        {
            if (Aggregation != null)
            {
                Aggregation.ChildAggregations = Aggregation.ChildAggregations ?? new List<AggregationField<T>>();
                Aggregation.ChildAggregations.Add(childAggregation);
            }
            return this;
        }

        public AggregationHelper<T> SetOrder(TermsOrder order)
        {
            if (Aggregation != null)
            {
                Aggregation.Order = order;
            }
            return this;
        }

        public AggregationHelper<T> SetRanges(List<RangeField> rangeList)
        {
            if (Aggregation != null && rangeList != null && rangeList.Any())
            {
                Aggregation.RangeList = rangeList;
            }
            return this;
        }

        public AggregationHelper<T> SetHistogram(IntervalType intervalType, string format = "yyyy-MM-dd")
        {
            if (Aggregation != null)
            {
                Aggregation.Interval = GetHistogramInterval(intervalType);
                Aggregation.Format = format;
            }
            return this;
        }

        public AggregationHelper<T> AddRange(string key,string name, double from, double to)
        {
            if (Aggregation != null && !string.IsNullOrEmpty(key))
            {
                Aggregation.RangeList = Aggregation.RangeList ?? new List<RangeField>();
                Aggregation.RangeList.Add(new RangeField()
                {
                    Name = name,
                    Key = key,
                    From = from,
                    To = to
                });
            }
            return this;
        }

        public AggregationHelper<T> Add()
        {
            if (Aggregation != null && !string.IsNullOrEmpty(Aggregation.Name) && Aggregation.MapType!= AgregationMapType.None)
            {
                Helper.AddAggregation(Aggregation);
                return this;
            }

            throw new Exception("Aggregation definition is not valid!");
        }

        private string GetHistogramInterval(IntervalType intervalType)
        {
            switch (intervalType)
            {
                case IntervalType.Day:
                    return "1d";
                case IntervalType.Month:
                    return "1M";
                case IntervalType.Week:
                    return "1w";
                case IntervalType.Year:
                    return "1y";
            }
            return "1M";
        }
    }
}
