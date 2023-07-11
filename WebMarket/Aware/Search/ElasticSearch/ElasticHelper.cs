using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Search.ElasticSearch.Model;
using Nest;

namespace Aware.Search.ElasticSearch
{
    public class ElasticHelper<T> where T : class
    {
        private QueryContainerDescriptor<T> _query;

        public ElasticHelper()
        {
            QueryList = new List<QueryContainer>();
            AggregationList = new List<AggregationField<T>>();
        }

        public bool IncludeAggregations { get; set; }

        public List<QueryContainer> QueryList { get; set; }

        public List<AggregationField<T>> AggregationList { get; private set; }

        public QueryContainerDescriptor<T> QueryDescriptor
        {
            get { return _query ?? (_query = new QueryContainerDescriptor<T>()); }
        }

        public List<Sorter<T>> SortList { get; set; }

        #region Queries
        public ElasticHelper<T> AddQuery(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
        {
            try
            {
                var query = querySelector(QueryDescriptor);
                QueryList.Add(query);
            }
            catch (Exception)
            {

            }
            return this;
        }

        public ElasticHelper<T> Bool(Func<BoolQueryDescriptor<T>, IBoolQuery> clauses)
        {
            if (_nested)
            {
                var query = QueryDescriptor.Nested(n => n.Path(_nestedPath).Query(t => t.Bool(clauses)));
                QueryList.Add(query);
                DisposeNested();
            }
            else
            {
                var query = QueryDescriptor.Bool(clauses);
                QueryList.Add(query);
            }
            return this;
        }

        public ElasticHelper<T> Sub()
        {
            return new ElasticHelper<T>();
        }

        public ElasticHelper<T> Term(Expression<Func<T, object>> onField, string value, double boost = 1)
        {
            var query = QueryDescriptor.Term(i => i.Field(onField).Value(value).Boost(boost));
            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> Terms(Expression<Func<T, object>> onField, IEnumerable<int> values, double boost = 1)
        {
            var query = QueryDescriptor.Terms(i => i.Field(onField).Boost(boost).Terms<int>(values));
            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> Wildcard(Expression<Func<T, object>> onField, string value, double boost = 1)
        {
            var query = QueryDescriptor.Wildcard(i => i.Field(onField).Value(value).Boost(boost));
            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> Match(Expression<Func<T, object>> onField, string value, double boost = 1)
        {
            var query = QueryDescriptor.Match(i => i.Field(onField).Query(value).Boost(boost));
            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> MultiMatch(IEnumerable<string> fields, string queryText, double boost = 1)
        {
            var query = QueryDescriptor.MultiMatch(i => i.Fields(f => f.Fields(fields.ToArray())).Query(queryText).Boost(boost));
            QueryList.Add(query);
            return this;
        }
        public ElasticHelper<T> Regex(Expression<Func<T, object>> onField, string value, double boost = 1)
        {
            var query = QueryDescriptor.Regexp(i => i.Field(onField).Value(value).Boost(boost));
            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> Range(Expression<Func<T, object>> onField, double? lowerRange, double? upperRange, double boost = 1)
        {
            QueryContainer query = new QueryContainer();
            if (lowerRange.HasValue && upperRange.HasValue)
            {
                query = QueryDescriptor.Range(i => i.Field(onField).GreaterThanOrEquals(lowerRange).LessThan(upperRange).Boost(boost));
            }
            else if (lowerRange.HasValue)
            {
                query = QueryDescriptor.Range(i => i.Field(onField).GreaterThanOrEquals(lowerRange).Boost(boost));
            }
            else if (upperRange.HasValue)
            {
                query = QueryDescriptor.Range(i => i.Field(onField).LessThan(upperRange).Boost(boost));
            }

            QueryList.Add(query);
            return this;
        }

        public ElasticHelper<T> Dismax(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector, double tieBreaker, double boost = 1)
        {
            var query = QueryDescriptor.DisMax(i => i.Queries(querySelector).TieBreaker(tieBreaker).Boost(boost));
            QueryList.Add(query);
            return this;
        }

        public QueryContainer GetQueries()
        {
            if (QueryList != null && QueryList.Any())
            {
                return QueryList.Aggregate<QueryContainer, QueryContainer>(null, (current, query) => current & query);
            }
            return new QueryContainerDescriptor<T>().MatchAll();
        }

        #endregion

        #region Aggregation
        public AggregationHelper<T> Aggregation(Expression<Func<T, object>> onField, int size = 0, AgregationType type = AgregationType.Term)
        {
            if (onField != null)
            {
                var aggregationHelper = new AggregationHelper<T>(this);
                aggregationHelper.Init(onField, size, type);
                return aggregationHelper;
            }
            return null;
        }

        public ElasticHelper<T> AddAggregation(AggregationField<T> aggregation)
        {
            if (aggregation != null && AggregationList.All(i => i.Name != aggregation.Name))
            {
                AggregationList.Add(aggregation);
                IncludeAggregations = true;
            }
            return this;
        }

        public IAggregationContainer GetAggregations()
        {
            var descriptor = new AggregationContainerDescriptor<T>();
            if (AggregationList != null && AggregationList.Any())
            {
                AggregationList.ForEach(i => SetAggregationDescriptor(descriptor, i));
            }
            return descriptor;
        }

        private void SetAggregationDescriptor(AggregationContainerDescriptor<T> descriptor, AggregationField<T> aggregationField)
        {
            var filterContainer = new QueryContainer();
            var hasFilter = false;
            if (aggregationField.FilterList != null && aggregationField.FilterList.Any())
            {
                filterContainer = GetFilters(aggregationField.FilterList);
                hasFilter = true;
            }

            var field = aggregationField;
            if (hasFilter)
            {
                if (aggregationField.IsNested && !string.IsNullOrEmpty(field.NestedPath))
                {
                    //descriptor.Filter(field.Name, f => f.Filter(d => filterContainer)
                    //    .Aggregations(a => a
                    //        .Nested(field.Name, n => n.Path(field.NestedPath)
                    //            .Aggregations(agg => agg.Filter(field.Name,
                    //                fl => fl.Filter(d => d.Term(field.Nested.FilterFieldName, field.Nested.FilterValue))
                    //                    .Aggregations(g => g.Terms(field.Name, t => GetTermsAggregation(t, field))))))));
                }
                else
                {
                    descriptor.Filter(field.Name, f => f
                        .Filter(d => filterContainer)
                        .Aggregations(ag => FillAggregationDescriptor(ag, field)));
                }
            }
            else
            {
                if (field.IsNested && !string.IsNullOrEmpty(field.NestedPath))
                {
                    descriptor.Nested(field.Name, n => n
                        .Path(field.NestedPath)
                        .Aggregations(ag => FillAggregationDescriptor(ag, field)));


                    //descriptor.Nested(field.Name, n => n
                    //    .Path(field.Nested.Path)
                    //    .Aggregations(agg => agg.Filter(field.Name, fl => fl.Filter(d => d
                    //        .Term(field.Nested.FilterFieldName, field.Nested.FilterValue))
                    //        .Aggregations(g => g.Terms(field.Name, t => GetTermsAggregation(t, field))))));
                }
                else
                {
                    FillAggregationDescriptor(descriptor, field);
                }
            }
        }

        private AggregationContainerDescriptor<T> FillAggregationDescriptors(AggregationContainerDescriptor<T> descriptor, List<AggregationField<T>> fields)
        {
            foreach (var aggregationField in fields)
            {
                if (aggregationField.IsNested && !string.IsNullOrEmpty(aggregationField.NestedPath))
                {
                    var field = aggregationField;
                    descriptor.Nested(field.Name, n => n.Path(field.NestedPath).Aggregations(ag => FillAggregationDescriptor(ag, field)));
                }
                else
                {
                    FillAggregationDescriptor(descriptor, aggregationField);
                }
            }
            return descriptor;
        }

        private AggregationContainerDescriptor<T> FillAggregationDescriptor(AggregationContainerDescriptor<T> descriptor, AggregationField<T> field)
        {
            if (field.Type == AgregationType.Sum)
            {
                descriptor.Sum(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Min)
            {
                descriptor.Min(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Max)
            {
                descriptor.Max(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Avg)
            {
                descriptor.Average(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Count)
            {
                descriptor.ValueCount(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Cardinality)
            {
                descriptor.Cardinality(field.Name, term => term.Field(field.OnField));
            }
            else if (field.Type == AgregationType.Range && field.RangeList != null && field.RangeList.Any())
            {
                descriptor.Range(field.Name, t => GetRangeAggregation(t, field));
            }
            else if (field.Type == AgregationType.DateHistogram)
            {
                descriptor.DateHistogram(field.Name, t => GetDateHistogramAggregation(t, field));
            }
            else
            {
                descriptor.Terms(field.Name, term => GetTermsAggregation(term, field));
            }
            return descriptor;
        }


        private IRangeAggregation GetRangeAggregation(RangeAggregationDescriptor<T> descriptor, AggregationField<T> aggregationField)
        {
            if (descriptor != null && aggregationField != null && aggregationField.RangeList != null && aggregationField.RangeList.Any())
            {
                var ranges = aggregationField.RangeList.Select(range =>
                {
                    return new Func<RangeDescriptor, IRange>(ff => new Range { Key = range.Key, From = range.From, To = range.To });
                }).ToArray();

                if (aggregationField.IsNested)
                {
                    descriptor.Aggregations(a => a.ReverseNested(aggregationField.Name.ToNestedReverse(), f => f));
                }

                if (aggregationField.ChildAggregations != null && aggregationField.ChildAggregations.Any())
                {
                    descriptor.Aggregations(a => FillAggregationDescriptors(a, aggregationField.ChildAggregations));
                }
                return descriptor.Field(aggregationField.OnField).Ranges(ranges);
            }
            return null;
        }

        private TermsAggregationDescriptor<T> GetTermsAggregation(TermsAggregationDescriptor<T> descriptor, AggregationField<T> aggregationField)
        {
            var termsDescriptor = descriptor.Field(aggregationField.OnField);
            termsDescriptor.Size(aggregationField.Size);

            if (!string.IsNullOrEmpty(aggregationField.Include))
            {
                termsDescriptor.Include(aggregationField.Include);
            }

            if (!string.IsNullOrEmpty(aggregationField.Exclude))
            {
                termsDescriptor.Exclude(aggregationField.Exclude);
            }

            bool isNested = aggregationField.IsNested && aggregationField.WithReverseNested;
            if (isNested && aggregationField.ChildAggregations != null && aggregationField.ChildAggregations.Any())
            {
                termsDescriptor.Aggregations(a => FillAggregationDescriptors(a, aggregationField.ChildAggregations).ReverseNested(aggregationField.Name.ToNestedReverse(), f => f));
            }
            else if (aggregationField.ChildAggregations != null && aggregationField.ChildAggregations.Any())
            {
                termsDescriptor.Aggregations(a => FillAggregationDescriptors(a, aggregationField.ChildAggregations));
            }
            else if (isNested)
            {
                termsDescriptor.Aggregations(a => a.ReverseNested(aggregationField.Name.ToNestedReverse(), f => f));
            }

            if (aggregationField.Order == TermsOrder.TermAscending)
            {
                termsDescriptor.OrderAscending("_term");
            }
            else if (aggregationField.Order == TermsOrder.TermDescending)
            {
                termsDescriptor.OrderDescending("_term");
            }
            else if (aggregationField.Order == TermsOrder.CountAscending)
            {
                termsDescriptor.OrderAscending("_count");
            }

            //Varsayılan bu olması lazım
            //else
            //{
            //    termsDescriptor.OrderDescending("_count");
            //}
            return termsDescriptor;
        }

        private DateHistogramAggregationDescriptor<T> GetDateHistogramAggregation(DateHistogramAggregationDescriptor<T> descriptor, AggregationField<T> aggregationField)
        {
            if (aggregationField.ChildAggregations != null && aggregationField.ChildAggregations.Any())
            {
                descriptor.Aggregations(a => FillAggregationDescriptors(a, aggregationField.ChildAggregations));
            }
            return descriptor.Field(aggregationField.OnField).Interval(aggregationField.Interval).Format(aggregationField.Format);
        }

        #endregion

        private QueryContainer GetFilters(IEnumerable<OFilterContainer> filterList)
        {
            return filterList.Aggregate<OFilterContainer, QueryContainer>(null, (current, filter) => current & filter.FilterContainer);
        }

        private bool _nested;
        private Expression<Func<T, object>> _nestedPath;
        public ElasticHelper<T> Nested(Expression<Func<T, object>> path)
        {
            _nested = true;
            _nestedPath = path;
            return this;
        }

        private void DisposeNested()
        {
            _nested = false;
            _nestedPath = null;
        }

    }
}