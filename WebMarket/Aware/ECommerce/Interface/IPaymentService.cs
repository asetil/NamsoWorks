using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aware.Payment.Model;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.ECommerce.Interface
{
    public interface IPaymentService
    {
        Result ProcessPosPayment(string uniqueOrderID, OnlineSales salesInfo, PosDefinition posDefinition, CreditCard cardInfo);
        Result ProcessGarantiPay(string uniqueOrderID, OnlineSales salesInfo);

        Result RefundPayment(string uniqueOrderID, string ipAddress);
        Result CancelPayment(string uniqueOrderID, string ipAddress);
        string Get3DForm(string uniqueOrderID,OnlineSales salesInfo, PosType posType, PosPaymentMethod paymentMethod, CreditCard card = null);
        Result Get3DResult(Dictionary<string,string> bankResponse,string uniqueOrderID);

        PosDefinition GetPosDefinition(Expression<Func<PosDefinition, bool>> filter);
        List<PosDefinition> GetPosDefinitions(bool discardTestPos = false);
        PosDefinitionDetailModel GetPosDefinitionDetail(int posDefinitionID);

        List<BankInfo> GetBankList(Statuses status = Statuses.None);
        List<InstallmentInfo> GetCachedInstallments(int posID = 0);
        List<InstallmentInfo> GetInstallmentList(Statuses status = Statuses.None);
        CardInfo GetCardInfo(string binNumber, int posID);

        Result SavePosDefinition(PosDefinition model);
        Result SaveBankInfo(BankInfo model);
        Result SaveInstallmentInfo(InstallmentInfo model);
        Result DeleteBankInfo(int itemID);
        Result DeleteInstallmentInfo(int itemID);
    }
}