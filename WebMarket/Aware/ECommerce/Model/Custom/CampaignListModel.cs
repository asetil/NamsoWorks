using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Model.Custom
{
    public class CampaignListModel
    {
        public List<Campaign> CampaignList { get; set; }
        public List<Lookup> ScopeList { get; set; }
        public List<Lookup> DiscountTypeList { get; set; }
    }
}
