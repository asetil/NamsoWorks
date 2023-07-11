using System.Collections.Generic;
using Aware.Payment.Model;
using Aware.Util.Model;

namespace Aware.Payment
{
    public interface IPaymentProvider
    {
        Result ProcessXml(OnlineSales payment, CreditCard card);
        Result RefundPayment(OnlineSales payment);
        Result CancelPayment(OnlineSales payment);
        string Get3DForm(OnlineSales payment, CreditCard card);
        Result Get3DResult(Dictionary<string, string> bankResponse);
    }
}