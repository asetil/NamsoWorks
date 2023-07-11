using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class ShippingMethod
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string RegionInfo { get; set; }
        public virtual decimal Price { get; set; }
        public virtual Statuses Status { get; set; }

        [NotMapped]
        public virtual string DiscountInfo { get; set; }

        public virtual bool HasRegion(int regionID)
        {
            if (!string.IsNullOrEmpty(RegionInfo))
            {
                var regions = RegionInfo.Trim(',').Trim().Split(",");
                return regions.Any(i => i == regionID.ToString());
            }
            return false;
        }
    }
}