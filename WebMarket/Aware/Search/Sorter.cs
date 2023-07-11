using System;
using System.Linq;
using System.Linq.Expressions;
using Nest;

namespace Aware.Search
{
    public class Sorter<T> where T:class 
    {
        public Expression DynamicOnField { get; set; }
        public bool Descending { get; set; }
        public Type ResultType { get; set; }

        public void OrderBy(ref IQueryable<T> query)
        {
            if (ResultType == typeof(string))
            {
                var onfield = DynamicOnField as Expression<Func<T, string>>;
                query = Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
            }
            else if (ResultType == typeof(int))
            {
                var onfield = DynamicOnField as Expression<Func<T, int>>;
                query = Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
            }
            else if (ResultType == typeof(decimal))
            {
                var onfield = DynamicOnField as Expression<Func<T, decimal>>;
                query = Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
            }
            else if (ResultType == typeof(DateTime))
            {
                var onfield = DynamicOnField as Expression<Func<T, DateTime>>;
                query = Descending ? query.OrderByDescending(onfield) : query.OrderBy(onfield);
            }
        }

        public void ElasticOrderBy(ref SortDescriptor<T> sortDescriptor)
        {
            if (ResultType == typeof(string))
            {
                var onfield = DynamicOnField as Expression<Func<T, string>>;
                sortDescriptor.Field(onfield, Descending ? SortOrder.Descending : SortOrder.Ascending);
            }
            else if (ResultType == typeof(int))
            {
                var onfield = DynamicOnField as Expression<Func<T, int>>;
                sortDescriptor.Field(onfield, Descending ? SortOrder.Descending : SortOrder.Ascending);
            }
            else if (ResultType == typeof(decimal))
            {
                var onfield = DynamicOnField as Expression<Func<T, decimal>>;
                sortDescriptor.Field(onfield, Descending ? SortOrder.Descending : SortOrder.Ascending);
            }
            else if (ResultType == typeof(DateTime))
            {
                var onfield = DynamicOnField as Expression<Func<T, DateTime>>;
                sortDescriptor.Field(onfield, Descending ? SortOrder.Descending : SortOrder.Ascending);
            }
        }

    }
}