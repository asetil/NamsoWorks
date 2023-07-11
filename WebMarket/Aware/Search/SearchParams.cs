﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Search.ElasticSearch;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.Search
{
    public abstract class SearchParams<T> : IElasticBuilder, ISearchParams<T> where T : class
    {
        public int UserID { get; set; }
        public IEnumerable<int> IDs { get; set; }
        public string Keyword { get; set; }
        public Statuses? Status { get; set; }
        public string Fields { get; set; }
        public int Page { get; private set; }
        public int Size { get; private set; }
        public bool IncludeAggregations { get; private set; }
        public bool IncludeCount { get; private set; }

        public abstract SearchHelper<T> PrepareFilters();

        public virtual ElasticHelper<TX> GetElasticHelper<TX>() where TX : class
        {
            return null;
        }

        public ISearchParams<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false)
        {
            SearchHelper.SortBy(orderBy, descending);
            return this;
        }

        public List<Sorter<T>> SortList
        {
            get { return SearchHelper.SortList; }
        }

        public ISearchParams<T> SetPaging(int page, int size)
        {
            Page = page;
            if (size > 0) { Size = size; }
            return this;
        }

        public ISearchParams<T> WithAggs(bool include = true)
        {
            IncludeAggregations = include;
            return this;
        }

        public ISearchParams<T> WithCount()
        {
            IncludeCount = true;
            return this;
        }

        public string IDsString
        {
            get
            {
                return IDs != null && IDs.Any() ? string.Join(",", IDs) : string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IDs = value.Split(',').Select(i => i.Int()).Where(i => i > 0);
                }
            }
        }

        public int Skip
        {
            get
            {
                return (Page > 0 ? Page - 1 : 0) * Size;
            }
        }

        private SearchHelper<T> _searchHelper;
        protected SearchHelper<T> SearchHelper
        {
            get
            {
                if (_searchHelper == null)
                {
                    _searchHelper = new SearchHelper<T>();
                }
                return _searchHelper;
            }
        }

        protected List<KeyValuePair<double, double>> GetFilterRanges(string filterAsString, int maxValue)
        {
            var result = new List<KeyValuePair<double, double>>();
            if (!string.IsNullOrEmpty(filterAsString))
            {
                var rangeList = filterAsString.Replace("[", "").Replace("]", "").Split(',');
                foreach (var range in rangeList)
                {
                    var ranges = range.Split(":");
                    double from, to = maxValue;
                    double.TryParse(ranges[0], out from);

                    if (ranges.Length > 1)
                    {
                        double.TryParse(ranges[1], out to);
                        to = to < from ? maxValue : to;
                    }
                    result.Add(new KeyValuePair<double, double>(from, to));
                }
            }
            return result;
        }
    }
}