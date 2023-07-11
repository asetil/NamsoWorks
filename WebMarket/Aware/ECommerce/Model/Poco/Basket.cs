using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Basket
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual int UserID { get; set; }
        public virtual Statuses Status { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual ICollection<BasketItem> Items { get; set; }

        [NotMapped]
        public virtual IEnumerable<Discount> Discounts { get; set; }

        public virtual decimal Total
        {
            get
            {
                if (Items!=null && Items.Any())
                {
                    return Items.Where(i => i.Status == Statuses.Active).Sum(i => i.GrossTotal);
                }
                return 0;
            }
        }

        public virtual decimal DiscountTotal
        {
            get
            {
                if (Discounts != null && Discounts.Any())
                {
                    return Discounts.Where(i => i.Status == Statuses.Active).Sum(i => i.Total);
                }
                return 0;
            }
        }
        public virtual decimal GrossTotal
        {
            get { return Total - DiscountTotal; }
        }

        public virtual int StoreID
        {
            get
            {
                if (Items != null && Items.Any())
                {
                    return Items.FirstOrDefault().StoreID;
                }
                return 0;
            }
        }
    }
}
