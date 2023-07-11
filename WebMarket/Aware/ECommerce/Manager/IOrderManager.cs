using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Payment.Model;
using Aware.Util.Model;

namespace Aware.ECommerce.Manager
{
    public interface IOrderManager
    {
        Result ProcessPayment(int userID, int orderID, CreditCard card, int posID, int installment, string ipAddress);
        CardInfo GetCardInfo(string binNumber, int posID, decimal orderTotal);
        Result HandlePaymentResult(int userID,string uniqueOrderID, Dictionary<string, string> bankResponse);
        InstallmentViewModel GetInstallments(decimal total,int mode);
        BankInfo GetBank(int id);
    }
}
