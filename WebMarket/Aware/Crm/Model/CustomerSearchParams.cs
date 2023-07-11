using System.Linq;
using Aware.Search;
using Aware.Util.Enums;

namespace Aware.Crm.Model
{
    public class CustomerSearchParams : SearchParams<Customer>
    {
        public override SearchHelper<Customer> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            if (!string.IsNullOrEmpty(Keyword))
            {
                searchHelper.FilterBy(i => i.Name.ToLower().Contains(Keyword.ToLower()));
            }

            if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID));
            }

            if (Status.HasValue && Status.Value != Statuses.None)
            {
                searchHelper.FilterBy(i => i.Status == Status.Value);
            }
            return searchHelper;
        }
    }
}
