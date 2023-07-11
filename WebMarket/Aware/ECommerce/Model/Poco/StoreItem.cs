using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class StoreItem
    {
        [Key]
        public virtual int ID { get; set; }

        public virtual decimal ListPrice { get; set; }

        public virtual decimal SalesPrice { get; set; }

        public virtual decimal Stock { get; set; }

        [ForeignKey("Store")]
        public virtual int StoreID { get; set; }

        public virtual Store Store { get; set; }

        [ForeignKey("Product")]
        public virtual int ProductID { get; set; }

        public virtual Product Product { get; set; }

        public virtual Statuses Status { get; set; }

        public virtual bool IsForSale { get; set; }

        public virtual bool HasVariant { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual int DiscountRate
        {
            get
            {
                if (ListPrice > SalesPrice && ListPrice > 0)
                {
                    return Convert.ToInt32(Math.Round((ListPrice - SalesPrice) / ListPrice, 2) * 100);
                }
                return 0;
            }
        }


        [NotMapped]
        public virtual int Score { get; set; }

        public virtual int AddScore(int score)
        {
            Score += score;
            return Score;
        }
    }
}