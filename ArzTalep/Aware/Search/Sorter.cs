using Aware.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Aware.Search
{
    public class Sorter<T> where T : IEntity
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
    }
}