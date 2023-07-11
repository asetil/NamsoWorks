using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util.Enums;

namespace Aware.Payment.Model
{
    public class OnlineSales
    {
        public virtual int ID { get; set; }
        public virtual int UserID { get; set; }
        public virtual string Email { get; set; }
        public virtual string OrderID { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual int Installment { get; set; }
        public virtual TransactionType Type { get; set; } //satış, iade, iptal
        public virtual CurrencyCode CurrencyCode { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual int PosID { get; set; }
        public virtual string Code { get; set; }
        public virtual string ReasonCode { get; set; }
        public virtual string Message { get; set; }
        public virtual string ErrorMsg { get; set; }
        public virtual string SysErrMsg { get; set; }
        public virtual string RetrefNum { get; set; }
        public virtual string AuthCode { get; set; }
        public virtual string ProvDate { get; set; }
        public virtual string HashData { get; set; }
        public virtual bool IsSuccess { get; set; }

        [NotMapped]
        public virtual BankResponse BankResponse { get; set; }

        public virtual OnlineSales Clone()
        {
            var result = MemberwiseClone() as OnlineSales;
            result.ID = 0;
            return result;
        }

        public virtual bool IsValid()
        {
            var result = true;
            if (string.IsNullOrEmpty(OrderID))
            {
                result = false;
            }
            else if (string.IsNullOrEmpty(IPAddress))
            {
                result = false;
            }
            else if (Amount < 0.01M)
            {
                result = false;
            }
            return result;
        }
    }
}

/* Payment Types

•         sales   (Satış)
•         preauth (Ön otorizasyon)
•         postauth (Ön otorizasyon kapama)
•         void (İptal)
•         refund (İade)
•         orderinq (Sipariş Sorgulama)
•         orderhistoryinq (Sipariş detay sorgulama)
•         rewardinq (Bonus sorgulama)
*/