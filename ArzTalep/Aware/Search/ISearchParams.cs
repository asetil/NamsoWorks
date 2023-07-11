using Aware.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aware.Search
{
    public interface ISearchParams<T> where T : IEntity
    {
        List<Expression<Func<T, bool>>> FilterList { get; }
        List<Sorter<T>> SortList { get; }

        bool IncludeCount { get; }
        int Page { get; }
        int Size { get; }
        int Skip { get; }

        /// <summary>
        /// A comma seperated string value 
        /// </summary>
        string NavigationFields { get; }

        ISearchParams<T> SetPaging(int page, int size);
        ISearchParams<T> FilterBy(Expression<Func<T, bool>> filter);
        ISearchParams<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false);
    }
}