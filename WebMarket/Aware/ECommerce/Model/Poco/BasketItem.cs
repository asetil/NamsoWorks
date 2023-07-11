using System;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class BasketItem
    {
        public virtual int ID { get; set; }

        [ForeignKey("Basket")]
        public virtual int BasketID { get; set; }

        [ForeignKey("Product")]
        public virtual int ProductID { get; set; }

        public virtual int ItemID { get; set; }
        public virtual int StoreID { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal Price { get; set; }

        public virtual decimal ListPrice { get; set; }
        public virtual decimal GrossTotal { get; set; }

        public virtual int VariantCode { get; set; }
        public virtual decimal VariantPrice { get; set; }
        public virtual string VariantDescription { get; set; }

        public virtual Statuses Status { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual Basket Basket { get; set; }
        public virtual Product Product { get; set; }

        public virtual string QuantityDescription
        {
            get
            {
                if (Product != null)
                {
                    var quantity = Quantity;
                    var unit = Product.Unit;
                    return string.Format("{0} {1}", quantity.DecString(unit == MeasureUnits.Unit ? "#" : "#,##0.00"), Product.UnitDescription.Short(2, string.Empty));
                }
                return Quantity.ToString("N1");
            }
        }
    }
}
