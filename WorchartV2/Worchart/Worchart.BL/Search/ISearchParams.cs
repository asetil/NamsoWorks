using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Worchart.BL.Model;

namespace Worchart.BL.Search
{
    public interface ISearchParams<T> where T : IEntity
    {
        SearchHelper<T> PrepareFilters();
        ISearchParams<T> AddFilter(Expression<Func<T, bool>> filter);

        ISearchParams<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false);
        List<Sorter<T>> SortList { get; }

        ISearchParams<T> SetPaging(int page, int size);
        int Skip { get; }
        int Page { get; }
        int Size { get; }

        ISearchParams<T> WithCount();
        bool IncludeCount { get; }

        IEnumerable<int> IDs { get; set; }
        string Keyword { get; set; }
        string Fields { get; set; }
    }
}