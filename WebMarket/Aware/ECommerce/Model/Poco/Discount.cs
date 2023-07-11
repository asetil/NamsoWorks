using System;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Discount
    {
        public virtual int ID { get; set; }
        public virtual int CampaignID { get; set; }
        public virtual DiscountType DiscountType { get; set; }
        public virtual int BasketID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual decimal Factor { get; set; }
        public virtual decimal Total { get; set; }
        public virtual int UserID { get; set; } //0:Genel, >0 user spesific
        public virtual string Code { get; set; }
        public virtual int IsUsed { get; set; } //0: Hayır 1:Evet..
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime ExpireDate { get; set; }
        public virtual Statuses Status { get; set; }

        public virtual Discount Clone(int userID = -1, int isUsed = -1)
        {
            var result = this.MemberwiseClone() as Discount;
            result.ID = 0;
            result.IsUsed = isUsed == -1 ? IsUsed : isUsed;
            result.UserID = userID == -1 ? UserID : userID;
            return result;
        }
    }
}
