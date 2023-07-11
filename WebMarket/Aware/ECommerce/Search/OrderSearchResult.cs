using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Search
{
    public class OrderSearchResult : SearchResult<Order>
    {
        public List<Store> StoreList { get; set; }
        public List<Lookup> OrderStatusList { get; set; }
        public List<Lookup> PaymentTypes { get; set; }
    }
}
