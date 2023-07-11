using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class CampaignItem
    {
        public int ID { get; set; }
        public int CampaignID { get; set; }
        public int ItemID { get; set; }
        public Statuses Status { get; set; }
    }
}
