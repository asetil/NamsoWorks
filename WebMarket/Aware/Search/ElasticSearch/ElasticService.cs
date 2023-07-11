using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Search.ElasticSearch.Data;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;
using Aware.Util.Log;
using Nest;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch
{
    public class ElasticService : IElasticService
    {
        private readonly IElasticRepository _repository;
        private readonly ILogger _logger;

        public ElasticService(IElasticRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public bool IsActive
        {
            get { return Status == ElasticStatus.Active; }
        }

        public ElasticStatus Status
        {
            get { return _repository.Status; }
        }

        public bool IndexExist(string indexName)
        {
            return _repository.IndexExists(indexName);
        }

        public void CreateIndex(string indexName)
        {
            _repository.CreateIndex(indexName);
        }

        public void DeleteIndex(string indexName, bool deleteAll = false)
        {
            _repository.DeleteIndex(indexName, deleteAll);
        }

        public T Get<T>(int id) where T : class
        {
            return _repository.Get<T>(id);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return _repository.GetAll<T>();
        }

        public IEnumerable<T> GetMany<T>(IEnumerable<int> idList) where T : class
        {
            return _repository.GetMany<T>(idList);
        }

        public void Add<T>(T item) where T : class
        {
            _repository.Index(item);
        }

        public void DeleteAll<T>() where T : class
        {
            _repository.DeleteAll<T>();
        }

        public IBulkResponse InsertMany<T>(List<T> itemList, string indexName, int bulkSize = 1000, bool isBulk = false) where T : class
        {
            if (itemList != null && itemList.Any())
            {
                var response = _repository.InsertMany(itemList, indexName, bulkSize, isBulk);
                LogResponse(response);
                return response;
            }
            return null;
        }

        public ISearchResponse<T> Find<T>(ISearchRequest searchDescriptor) where T : class
        {
            var response = _repository.Find<T>(searchDescriptor);
            return response;
        }

        public SearchResult<T> Search<T>(IElasticBuilder elasticBuilder) where T : class
        {
            var searchDescriptor = new SearchDescriptor<T>();
            searchDescriptor.From(elasticBuilder.Skip);

            if (elasticBuilder.Size > 0)
            {
                searchDescriptor.Size(elasticBuilder.Size);
            }

            var helper = elasticBuilder.GetElasticHelper<T>();
            searchDescriptor.Query(q => helper.GetQueries());

            if (helper.SortList != null && helper.SortList.Any())
            {
                searchDescriptor.Sort(s => SetSorters(s, helper.SortList));
            }

            if (helper.IncludeAggregations)
            {
                searchDescriptor.Aggregations(a => helper.GetAggregations());
            }

            var response = _repository.Find<T>(searchDescriptor);
            if (response == null) { return new SearchResult<T>(); }

            var result = new SearchResult<T>
            {
                TotalSize = (int)response.Total,
                Results = response.Documents.ToList(),
                Took = response.Took,
                Success = response.IsValid
            };

            if (Config.IsDebugMode)
            {
                result.RequestBody = System.Text.Encoding.Default.GetString(response.ApiCall.RequestBodyInBytes);
            }

            if (helper.IncludeAggregations)
            {
                result.Aggregations = CollectAggregations(response, helper.AggregationList);
            }
            return result;
        }

        private static SortDescriptor<T> SetSorters<T>(SortDescriptor<T> sortDescriptor, List<Sorter<T>> sortList) where T : class
        {
            foreach (var sortField in sortList)
            {
                //Expression converted = Expression.Convert(sortField.OnField().Body, sortField.ResultType);
                //// Use Expression.Lambda to get back to strong typing
                //var aa = Expression.Lambda<Func<T, object>>(converted, sortField.OnField.Parameters);
                //sortDescriptor.Field(sortField.OnField, sortField.Descending ? SortOrder.Descending : SortOrder.Ascending);

                sortField.ElasticOrderBy(ref sortDescriptor);
            }
            return sortDescriptor;
        }

        private void LogResponse(IResponse response)
        {
            if (response != null && response.ServerError != null)
            {
                var message = !response.IsValid
                    ? string.Format("ElasticSearch Error : {0} - {1} - IsValid:{2} - {3}", response.ServerError.Status, response.ServerError.Error, response.IsValid, response.ApiCall.Uri)
                    : string.Format("ElasticSearch Error : {0} - {1} - {2}", response.ServerError.Status, response.ServerError.Error, response.ApiCall.Uri);
                _logger.Error(message, null);
            }
        }

        private void LogResponse(IBulkResponse response)
        {
            LogResponse((IResponse)response);
            _logger.Info("ElasticSearch Bulk Info");
            if (response != null && response.Errors)
            {
                _logger.Error("ElasticSearch Bulk Error : {0} - {1}", null, response.ItemsWithErrors.Count(), response.ServerError);
            }
        }

        private List<AggregationResult> CollectAggregations<T>(ISearchResponse<T> response, List<AggregationField<T>> aggregations) where T : class
        {
            var result = new List<AggregationResult>();
            if (aggregations != null && aggregations.Any())
            {
                foreach (var field in aggregations)
                {
                    var items = GetAggregationResult(response, field) ?? new List<AggregationItem>();
                    result.Add(new AggregationResult()
                    {
                        Name = field.DisplayName,
                        SearchName = field.Name,
                        MapType = field.MapType,
                        Items = items
                    });
                }
            }
            return result;
        }

        private List<AggregationItem> GetAggregationResult<T>(ISearchResponse<T> response, AggregationField<T> field) where T : class
        {
            if (!response.Aggs.Aggregations.ContainsKey(field.Name)) { return null; }

            ValueAggregate valueMetric = null;
            SingleBucketAggregate singleBucket;

            var isNested = field.IsNested && !string.IsNullOrEmpty(field.NestedPath);
            if (field.FilterList != null && field.FilterList.Any())
            {
                singleBucket = isNested ? response.Aggs.Nested(field.Name).Nested(field.Name).Nested(field.Name)
                                        : response.Aggs.Nested(field.Name);
            }
            else
            {
                singleBucket = isNested ? response.Aggs.Nested(field.Name) : null;
            }

            switch (field.Type)
            {
                case AgregationType.Term:
                    var bucket = singleBucket != null ? singleBucket.Terms(field.Name).Buckets.ToList() : response.Aggs.Terms(field.Name).Buckets.ToList();
                    return GetTermAggregationResult(bucket, field);
                case AgregationType.Range:
                    var rangeBucket = singleBucket != null ? singleBucket.Range(field.Name).Buckets.ToList() : response.Aggs.Range(field.Name).Buckets.ToList();
                    return GetRangeAggregationResult(rangeBucket, field);
                case AgregationType.DateHistogram:
                    var histogramBucket = singleBucket != null ? singleBucket.DateHistogram(field.Name).Buckets.ToList() : response.Aggs.DateHistogram(field.Name).Buckets.ToList();
                    return GetDateHistogramAggregationResult(histogramBucket, field);

                case AgregationType.Sum:
                    valueMetric = singleBucket != null ? singleBucket.Sum(field.Name) : response.Aggs.Sum(field.Name); break;
                case AgregationType.Min:
                    valueMetric = singleBucket != null ? singleBucket.Min(field.Name) : response.Aggs.Min(field.Name); break;
                case AgregationType.Max:
                    valueMetric = singleBucket != null ? singleBucket.Max(field.Name) : response.Aggs.Max(field.Name); break;
                case AgregationType.Avg:
                    valueMetric = singleBucket != null ? singleBucket.Average(field.Name) : response.Aggs.Average(field.Name); break;
                case AgregationType.Count:
                    valueMetric = singleBucket != null ? singleBucket.ValueCount(field.Name) : response.Aggs.ValueCount(field.Name); break;
                case AgregationType.Cardinality:
                    valueMetric = singleBucket != null ? singleBucket.Cardinality(field.Name) : response.Aggs.Cardinality(field.Name); break;
            }

            if (valueMetric != null)
            {
                return new List<AggregationItem> { new AggregationItem() { Term = "0", Count = Convert.ToInt64(valueMetric.Value.GetValueOrDefault()) } };
            }
            return null;
        }

        private List<AggregationItem> GetTermAggregationResult<T>(IList<KeyedBucket> bucket, AggregationField<T> field) where T : class
        {
            if (bucket.Any())
            {
                var items = bucket.Select(item =>
                {
                    var docCount = item.DocCount.GetValueOrDefault();
                    if (field.IsNested)
                    {
                        docCount = item.ReverseNested(field.Name.ToNestedReverse()).DocCount;
                    }

                    var childAggregations = GetChildAggregationResult(field, item);
                    return new AggregationItem
                    {
                        Term = item.Key,
                        Text = item.Key,
                        Count = docCount,
                        Childs = childAggregations
                    };
                }).ToList();
                return items;
            }
            return null;
        }

        private List<AggregationItem> GetRangeAggregationResult<T>(IList<RangeBucket> bucket, AggregationField<T> field) where T : class
        {
            if (bucket.Any())
            {
                var items = bucket.Select(item =>
                {
                    var docCount = item.DocCount;
                    if (field.IsNested)
                    {
                        docCount = item.ReverseNested(field.Name.ToNestedReverse()).DocCount;
                    }

                    var childAggregations = GetChildAggregationResult(field, item);
                    var rangeDefinition = field.RangeList.FirstOrDefault(r => r.Key == item.Key);

                    return new AggregationItem
                    {
                        Term = item.Key,
                        Text = rangeDefinition != null ? rangeDefinition.Name : item.Key,
                        Count = docCount,
                        Childs = childAggregations
                    };
                }).ToList();
                return items;
            }
            return null;
        }

        private List<AggregationItem> GetDateHistogramAggregationResult<T>(IList<DateHistogramBucket> bucket, AggregationField<T> field) where T : class
        {
            if (bucket.Any())
            {
                var items = bucket.Select(item =>
                {
                    var docCount = item.DocCount;
                    if (field.IsNested)
                    {
                        docCount = item.ReverseNested(field.Name.ToNestedReverse()).DocCount;
                    }

                    var childAggregations = GetChildAggregationResult(field, item);
                    return new AggregationItem
                    {
                        Term = item.KeyAsString,
                        Text = item.KeyAsString,
                        Count = docCount,
                        HistogramKey = item.Key,
                        Childs = childAggregations
                    };
                }).ToList();
                return items;
            }
            return null;
        }

        private List<AggregationResult> GetChildAggregationResult<T>(AggregationField<T> field, BucketBase item) where T : class
        {
            if (field.ChildAggregations != null && field.ChildAggregations.Any())
            {
                var result = new List<AggregationResult>();
                foreach (var childAggregation in field.ChildAggregations)
                {
                    List<AggregationItem> childResult = null;
                    if (childAggregation.Type == AgregationType.Term)
                    {
                        childResult = GetTermAggregationResult(item.Terms(childAggregation.Name).Buckets, childAggregation);
                    }
                    else if (childAggregation.Type == AgregationType.Range)
                    {
                        childResult = GetRangeAggregationResult(item.Range(childAggregation.Name).Buckets, childAggregation);
                    }
                    else if (childAggregation.Type == AgregationType.DateHistogram)
                    {
                        childResult = GetDateHistogramAggregationResult(item.DateHistogram(childAggregation.Name).Buckets, childAggregation);
                    }
                    else
                    {
                        ValueAggregate valueMetric = null;
                        switch (childAggregation.Type)
                        {
                            case AgregationType.Sum:
                                valueMetric = item.Sum(childAggregation.Name);
                                break;
                            case AgregationType.Min:
                                valueMetric = item.Min(childAggregation.Name);
                                break;
                            case AgregationType.Max:
                                valueMetric = item.Max(childAggregation.Name);
                                break;
                            case AgregationType.Avg:
                                valueMetric = item.Average(childAggregation.Name);
                                break;
                            case AgregationType.Count:
                                valueMetric = item.ValueCount(childAggregation.Name);
                                break;
                            case AgregationType.Cardinality:
                                valueMetric = item.Cardinality(childAggregation.Name);
                                break;
                        }

                        if (valueMetric != null)
                        {
                            childResult = new List<AggregationItem> { new AggregationItem() { Term = "0", Count = Convert.ToInt64(valueMetric.Value.GetValueOrDefault()) } };
                        }
                    }

                    if (childResult != null)
                    {
                        result.Add(new AggregationResult()
                        {
                            Name = childAggregation.DisplayName,
                            SearchName = childAggregation.Name,
                            MapType = field.MapType,
                            Items = childResult
                        });
                    }
                }
                return result;
            }
            return null;
        }
    }
}