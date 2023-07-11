using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aware.Search
{
    public interface ISearchParams<T> where T : class
    {
        SearchHelper<T> PrepareFilters();
        ISearchParams<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false);
        ISearchParams<T> SetPaging(int page, int size);
        ISearchParams<T> WithAggs(bool include = true);
        ISearchParams<T> WithCount();
        IEnumerable<int> IDs { get; set; }
        List<Sorter<T>> SortList { get; }

        int Skip { get; }
        int Page { get; }
        int Size { get; }
        bool IncludeCount { get; }
        string Fields { get; set; }
    }
}