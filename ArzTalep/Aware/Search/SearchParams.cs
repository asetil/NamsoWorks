using Aware.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aware.Search
{
    public class SearchParams<T> : ISearchParams<T> where T : IEntity
    {
        public List<Expression<Func<T, bool>>> FilterList { get; private set; }
        public List<Sorter<T>> SortList { get; private set; }

        public int Page { get; private set; }
        public int Size { get; private set; }
        public bool IncludeCount { get; private set; }
        public string NavigationFields { get; set; }

        public ISearchParams<T> SetPaging(int page, int size)
        {
            Page = page;
            if (size > 0) { Size = size; }
            return this;
        }

        public ISearchParams<T> WithCount()
        {
            IncludeCount = true;
            return this;
        }

        public int Skip
        {
            get
            {
                return (Page > 0 ? Page - 1 : 0) * Size;
            }
        }

        public ISearchParams<T> FilterBy(Expression<Func<T, bool>> filter)
        {
            FilterList = FilterList ?? new List<Expression<Func<T, bool>>>();
            FilterList.Add(filter);
            return this;
        }

        public ISearchParams<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false)
        {
            SortList = SortList ?? new List<Sorter<T>>();
            SortList.Add(new Sorter<T>
            {
                DynamicOnField = orderBy,
                Descending = descending,
                ResultType = typeof(TKey)
            });
            return this;
        }

        protected List<KeyValuePair<double, double>> GetFilterRanges(string filterAsString, int maxValue)
        {
            var result = new List<KeyValuePair<double, double>>();
            if (!string.IsNullOrEmpty(filterAsString))
            {
                var rangeList = filterAsString.Replace("[", "").Replace("]", "").Split(',');
                foreach (var range in rangeList)
                {
                    var ranges = range.Split(new char[] { ':' });
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