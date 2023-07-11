using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util.Enums;

namespace Aware.Payment.Model
{
    public class PosDefinition
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string TerminalID { get; set; }
        public virtual string UserID { get; set; }
        public virtual string Password { get; set; }
        public virtual string MerchantID { get; set; } // Üye işyeri numarası
        public virtual string StoreKey { get; set; }
        public virtual string PostUrl { get; set; }
        public virtual string XmlUrl { get; set; }
        public virtual PosType PosType { get; set; }
        public virtual PosPaymentMethod PaymentMethod { get; set; }
        public virtual bool IsTest { get; set; }
        public virtual string SuccessUrl { get; set; }
        public virtual string ErrorUrl { get; set; }
        public virtual string RefundUserID { get; set; } //PROVRFN
        public virtual string RefundPassword { get; set; }
        public virtual string ImageUrl { get; set; }

        [NotMapped]
        public virtual bool Is3DSecure
        {
            get
            {
                return PaymentMethod== PosPaymentMethod.Secure3D || PaymentMethod== PosPaymentMethod.Secure3D_PAY || PaymentMethod== PosPaymentMethod.Secure3D_OOS;
            }
        }

        [NotMapped]
        public virtual bool IsOOSPayment
        {
            get
            {
                return PaymentMethod == PosPaymentMethod.Secure3D_OOS || PaymentMethod == PosPaymentMethod.OOS_PAY;
            }
        }
    }
}