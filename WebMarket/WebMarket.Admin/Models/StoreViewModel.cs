using System.Collections.Generic;
using Aware.Crm.Model;
using Aware.ECommerce.Model;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace WebMarket.Admin.Models
{
    public class StoreViewModel
    {
        public Store Store { get; set; }
        public Customer Customer { get; set; }
        public List<Item> RegionList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public string RegionInfo { get; set; }
        public bool AllowRegionSelection { get; set; }

        public string GetTitle()
        {
            var result = "Market Detay";
            if (Store != null)
            {
                result = Store.ID > 0 ? Store.DisplayName : "Yeni Market";
            }
            return result;
        }
    }
}
