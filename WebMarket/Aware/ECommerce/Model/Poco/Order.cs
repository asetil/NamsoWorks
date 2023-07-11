using System;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Authenticate.Model;
using Aware.ECommerce.Util;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class Order
    {
        public virtual int ID { get; set; }
        public virtual int UserID { get; set; }
        public virtual int StoreID { get; set; }
        public virtual int BasketID { get; set; }
        public virtual int ShippingAddressID { get; set; }
        public virtual int BillingAddressID { get; set; }
        public virtual int ShippingMethodID { get; set; }
        public virtual int PaymentType { get; set; }
        public virtual int SubPaymentType { get; set; }
        public virtual int Installment { get; set; }
        public virtual string Note { get; set; }
        public virtual decimal BasketTotal { get; set; }
        public virtual decimal DiscountTotal { get; set; }
        public virtual decimal ShipmentCost { get; set; }
        public virtual decimal PaymentCost { get; set; }
        public virtual decimal GrossTotal { get; set; }
        public virtual OrderStatuses Status { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }

        [NotMapped]
        public virtual string Description { get; set; }

        [NotMapped]
        public virtual OrderDetail OrderDetail { get; set; }

        public virtual string UniqueID
        {
            get { return string.Format("{0}{1}", DateCreated.ToString(Constants.ORDER_DATE_FORMAT), ID); }
        }
    }

    public class OrderDetail
    {
        public virtual User User { get; set; }
        public virtual Store Store { get; set; }
    }
}
