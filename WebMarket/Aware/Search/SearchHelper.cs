using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Data;

namespace Aware.Search
{
    public class SearchHelper<T> where T : class
    {
        public List<Expression<Func<T, bool>>> FilterList { get; private set; }
        public List<Sorter<T>> SortList { get; private set; }
        public int Page { get; private set; }
        public int Size { get; private set; }
        public int Skip { get { return (Page - 1) * Size; } }

        private IRepository<T> _repository;
        public SearchHelper<T> SetRepository(IRepository<T> repository)
        {
            _repository = repository;
            return this;
        }

        public SearchHelper<T> FilterBy(Expression<Func<T, bool>> filter)
        {
            FilterList = FilterList ?? new List<Expression<Func<T, bool>>>();
            FilterList.Add(filter);
            return this;
        }

        public SearchHelper<T> SortBy<TKey>(Expression<Func<T, TKey>> orderBy, bool descending = false)
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

        public SearchHelper<T> SetPaging(int page, int size)
        {
            Page = page;
            if (size > 0) { Size = size; }
            return this;
        }

        public List<T> ToList()
        {
            if (_repository != null)
            {
                return _repository.Find(this);
            }
            return new List<T>();
        }

        public T First()
        {
            if (_repository != null)
            {
                return _repository.Find(this).FirstOrDefault();
            }
            return default(T);
        }

        public bool HasSorter
        {
            get { return SortList != null && SortList.Any(); }
        }

        public void ClearFilters()
        {
            FilterList = null;
        }
    }
}
