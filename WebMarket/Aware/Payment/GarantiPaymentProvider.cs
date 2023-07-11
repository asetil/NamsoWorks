/**************************************************************************************************
 *
 *    GERÇEK ÇEKİMLER İÇİN
 *      XML_URL => Normal : https://sanalposprov.garanti.com.tr/VPServlet
 *      3D Secure : https://sanalposprov.garanti.com.tr/servlet/gt3dengine 
 *
 *    TEST İÇİN
 *      XML_URL => Normal : https://sanalposprovtest.garanti.com.tr/VPServlet
 *      3D Secure : https://sanalposprovtest.garanti.com.tr/servlet/gt3dengine 
 *     
 *    3D_PAY, 3D_FULL, 3D_HALF
 *
 * ***********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Aware.Payment.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Payment
{
    public class GarantiPaymentProvider : BasePaymentProvider
    {
        public GarantiPaymentProvider(PosDefinition definition)
            : base(definition)
        {

        }

        public override Result ProcessXml(OnlineSales payment, CreditCard card)
        {
            try
            {
                if (payment != null && card != null)
                {
                    payment.PosID = PosDefinition.ID;
                    var password = IsRefund(payment) ? PosDefinition.RefundPassword : PosDefinition.Password;
                    var securityData = GetSHA1(password + PosDefinition.TerminalID.PadLeft(9, '0')).ToUpperInvariant();

                    var hashString = GetOrderID(payment) + PosDefinition.TerminalID + card.CardNumber + GetAmount(payment.Amount) + securityData;
                    var hashData = GetSHA1(hashString).ToUpperInvariant();

                    var result = PostXmlData(payment, card, hashData);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GarantiPaymentProvider > ProcessXml - Failed", ex);
            }
            return Result.Error("Ödeme işlemi gerçekleştirilemedi!");
        }

        public override string Get3DForm(OnlineSales payment, CreditCard card)
        {
            try
            {
                var orderID = GetOrderID(payment);
                var installment = GetInstallment(payment);
                var amount = GetAmount(payment.Amount);
                var paymentType=PosDefinition.PaymentMethod== PosPaymentMethod.GarantiPAY ? "gpdatarequest" : GetPaymentType(payment);
                var securityData = GetSHA1(PosDefinition.Password + PosDefinition.TerminalID.PadLeft(9, '0')).ToUpperInvariant();
                var hashString = PosDefinition.TerminalID + orderID + amount + PosDefinition.SuccessUrl + PosDefinition.ErrorUrl + paymentType + installment + PosDefinition.StoreKey + securityData;
                var hashData = GetSHA1(hashString).ToUpperInvariant();

                var secureLevel = "3D";
                switch (PosDefinition.PaymentMethod)
                {
                    case PosPaymentMethod.Secure3D_PAY:
                        secureLevel = "3D_PAY"; break; // Ayrıca 3D_FULL,3D_HALF
                    case PosPaymentMethod.Secure3D_OOS:
                        secureLevel = "3D_OOS_PAY"; break; //Ayrıca 3D_OOS_FULL,3D_OOS_HALF
                    case PosPaymentMethod.OOS_PAY:
                        secureLevel = "OOS_PAY"; break;
                    case PosPaymentMethod.GarantiPAY:
                        secureLevel = "CUSTOM_PAY"; break;
                }

                var formHtml = new StringBuilder();
                formHtml.AppendFormat("<form id='PaymentForm' action='{0}' method='POST'>", PosDefinition.PostUrl);
                formHtml.AppendFormat("<input type='hidden' name='secure3dsecuritylevel' id='secure3dsecuritylevel' value='{0}'/>", secureLevel);
                formHtml.AppendFormat("<input type='hidden' name='mode' id='mode' value='{0}'/>", PosDefinition.IsTest ? "TEST" : "PROD");
                formHtml.AppendFormat("<input type='hidden' name='apiversion' id='apiversion' value='v0.01'/>");
                formHtml.AppendFormat("<input type='hidden' name='terminalid' id='terminalid' value='{0}'/>", PosDefinition.TerminalID);
                formHtml.AppendFormat("<input type='hidden' name='terminalprovuserid' id='terminalprovuserid' value='{0}'/>", PosDefinition.UserID); //PROVAUT
                formHtml.AppendFormat("<input type='hidden' name='terminaluserid' id='terminaluserid' value='{0}'/>", payment.UserID);
                formHtml.AppendFormat("<input type='hidden' name='terminalmerchantid' id='terminalmerchantid' value='{0}'/>", PosDefinition.MerchantID);
                formHtml.AppendFormat("<input type='hidden' name='txntype' id='txntype' value='{0}'/>", paymentType);

                if (PosDefinition.PaymentMethod == PosPaymentMethod.GarantiPAY)
                {
                    formHtml.AppendLine("<input type='hidden' name='txnsubtype' id='txnsubtype' value='sales'/>");
                    formHtml.AppendLine("<input type='hidden' name='garantipay' id='garantipay' value='Y'/>");
                }
                else if (card != null && PosDefinition.PaymentMethod != PosPaymentMethod.Secure3D_OOS)
                {
                    formHtml.AppendFormat("<input type='hidden' name='cardnumber' id='cardnumber' value='{0}'/>", card.CardNumber);
                    formHtml.AppendFormat("<input type='hidden' name='cardexpiredatemonth' id='cardexpiredatemonth' value='{0}'/>", card.ExpireMonth.ToString().PadLeft(2, '0'));
                    formHtml.AppendFormat("<input type='hidden' name='cardexpiredateyear' id='cardexpiredateyear'value='{0}'/>", card.ExpireYear.ToString().PadLeft(2, '0'));
                    formHtml.AppendFormat("<input type='hidden' name='cardcvv2' id='cardcvv2'value='{0}'/>", card.CVC);
                }

                formHtml.AppendFormat("<input type='hidden' name='companyname' id='companyname' value='{0}'/>", PosDefinition.Name);
                formHtml.AppendFormat("<input type='hidden' name='txnamount' id='txnamount' value='{0}'/>", amount);
                formHtml.AppendFormat("<input type='hidden' name='txncurrencycode' id='txncurrencycode' value='{0}'/>", (int)payment.CurrencyCode);
                formHtml.AppendFormat("<input type='hidden' name='txninstallmentcount' id='txninstallmentcount' value='{0}'/>", installment);
                formHtml.AppendFormat("<input type='hidden' name='orderid' id='orderid' value='{0}'/>", orderID);
                formHtml.AppendFormat("<input type='hidden' name='successurl' id='successurl' value='{0}'/>", PosDefinition.SuccessUrl);
                formHtml.AppendFormat("<input type='hidden' name='errorurl' id='errorurl' value='{0}'/>", PosDefinition.ErrorUrl);
                formHtml.AppendFormat("<input type='hidden' name='customeremailaddress' id='customeremailaddress' value='{0}'/>", payment.Email);
                formHtml.AppendFormat("<input type='hidden' name='customeripaddress' id='customeripaddress' value='{0}'/>", payment.IPAddress);
                formHtml.AppendFormat("<input type='hidden' name='secure3dhash' id='secure3dhash' value='{0}'/>", hashData);

                formHtml.AppendFormat("<input type='hidden' name='txntimestamp' id='txntimestamp' value='{0}'/>", DateTime.Now.Ticks);
                formHtml.AppendFormat("<input type='hidden' name='lang' id='lang' value='tr'/>");
                formHtml.AppendFormat("<input type='hidden' name='refreshtime' id='refreshtime' value='5'/>");
                formHtml.AppendFormat("<input type='hidden' name='PosID' id='PosID' value='{0}'/>", PosDefinition.ID);

                if (PosDefinition.PaymentMethod == PosPaymentMethod.Secure3D_OOS)
                {
                    formHtml.AppendFormat("<input type='hidden' name='motoind' id='motoind'value='N'/>");
                }

                formHtml.AppendFormat("</form>");
                return formHtml.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("GarantiPaymentProvider > Get3DForm - Failed", ex);
            }
            return string.Empty;
        }

        protected override XmlDocument GetXmlData(OnlineSales payment, CreditCard card, string hashData)
        {
            var doc = new XmlDocument();
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
            doc.AppendChild(xmlDeclaration);

            XmlElement gvpsRequest = doc.CreateElement("GVPSRequest");
            doc.AppendChild(gvpsRequest);

            var mode = PosDefinition.IsTest ? "TEST" : "PROD";
            AddData(doc, gvpsRequest, "Mode", mode);
            AddData(doc, gvpsRequest, "Version", "v0.01");
            AddData(doc, gvpsRequest, "ChannelCode", string.Empty);

            var isRefund = IsRefund(payment);
            var terminal = AddData(doc, gvpsRequest, "Terminal", string.Empty);
            AddData(doc, terminal, "ProvUserID", isRefund ? PosDefinition.RefundUserID : PosDefinition.UserID);
            AddData(doc, terminal, "HashData", hashData);
            AddData(doc, terminal, "UserID", "PROVAUT"); //TODO#
            AddData(doc, terminal, "ID", PosDefinition.TerminalID);
            AddData(doc, terminal, "MerchantID", PosDefinition.MerchantID);

            var customer = AddData(doc, gvpsRequest, "Customer", string.Empty);
            AddData(doc, customer, "IPAddress", payment.IPAddress);
            AddData(doc, customer, "EmailAddress", payment.Email);

            if (!isRefund && card != null)
            {
                var cardInfo = AddData(doc, gvpsRequest, "Card", string.Empty);
                AddData(doc, cardInfo, "Number", card.CardNumber);
                AddData(doc, cardInfo, "ExpireDate", GetExpireDate(card));
                AddData(doc, cardInfo, "CVV2", PosDefinition.Is3DSecure ? string.Empty : card.CVC);
            }

            var orderID = string.Format("{0}{1}", PosDefinition.IsTest ? Guid.NewGuid().ToString() : string.Empty, GetOrderID(payment));
            var order = AddData(doc, gvpsRequest, "Order", string.Empty);
            AddData(doc, order, "OrderID", orderID);
            AddData(doc, order, "GroupID", " ");
            AddData(doc, order, "Description", " ");

            var transaction = AddData(doc, gvpsRequest, "Transaction", string.Empty);
            AddData(doc, transaction, "Type", GetPaymentType(payment));
            AddData(doc, transaction, "InstallmentCnt", payment.Installment > 0 ? payment.Installment.ToString() : string.Empty);
            AddData(doc, transaction, "Amount", GetAmount(payment.Amount));
            AddData(doc, transaction, "CurrencyCode", ((int)payment.CurrencyCode).ToString());
            AddData(doc, transaction, "MotoInd", "N");

            if (PosDefinition.Is3DSecure && payment.BankResponse != null)
            {
                AddData(doc, transaction, "CardholderPresentCode", "13");
                var secure3D = AddData(doc, transaction, "Secure3D", string.Empty);
                AddData(doc, secure3D, "AuthenticationCode", payment.BankResponse.Cavv); //cavv
                AddData(doc, secure3D, "SecurityLevel", payment.BankResponse.Eci);      //eci
                AddData(doc, secure3D, "TxnID", payment.BankResponse.Xid);              //xid
                AddData(doc, secure3D, "Md", payment.BankResponse.Md);                  //md
            }
            else
            {
                AddData(doc, transaction, "CardholderPresentCode", "0");
                AddData(doc, transaction, "OriginalRetrefNum", payment.RetrefNum);
            }
            return doc;
        }

        public override Result Get3DResult(Dictionary<string, string> bankResponse)
        {
            try
            {
                if (bankResponse != null && bankResponse.Any())
                {
                    var payment = new OnlineSales
                    {
                        OrderID = bankResponse.Value("oid").Replace("Aware_",""), //orderid
                        Amount = bankResponse.Value("txnamount").Dec(),
                        Type = TransactionType.Sales, //txntype üzerinden okunabilir
                        Installment = bankResponse.Value("txninstallmentcount").Int(),
                        UserID = bankResponse.Value("terminaluserid").Int(),
                        Email = bankResponse.Value("customeremailaddress"),
                        IPAddress = bankResponse.Value("customeripaddress"),
                        PosID = PosDefinition.ID,
                        ErrorMsg = bankResponse.Value("errmsg"),
                        SysErrMsg = bankResponse.Value("mderrormessage"),

                        BankResponse = new BankResponse
                        {
                            MdStatus = bankResponse.Value("mdstatus").Int(),
                            Md = bankResponse.Value("md"),
                            Cavv = bankResponse.Value("cavv"),
                            Eci = bankResponse.Value("eci"),
                            Xid = bankResponse.Value("xid"),
                            ErrorMessage = bankResponse.Value("errmsg"),
                            MdErrorMessage = bankResponse.Value("mderrormessage"),
                        }
                    };

                    //CHECK RESPONSE
                    var mdStatus = payment.BankResponse.MdStatus;
                    var procReturnCode = bankResponse.Value("procreturncode");
                    var hashParams = bankResponse.Value("hashparams");

                    var calculatedHash = string.Empty;
                    var message = !string.IsNullOrEmpty(payment.Message) ? payment.Message : Resource.Order_CannotPaidWithCreditCard;
                    if ( !string.IsNullOrEmpty(payment.ErrorMsg))
                    {
                        message = string.Format("{0} ({1})", message, payment.ErrorMsg);
                    }

                    if (PosDefinition.PaymentMethod == PosPaymentMethod.Secure3D)
                    {
                        var installment = payment.Installment > 0 ? payment.Installment.ToString() : string.Empty;
                        var secure3dHash = bankResponse.Value("secure3dhash");
                        var securityData = GetSHA1(PosDefinition.Password + PosDefinition.TerminalID.PadLeft(9, '0')).ToUpperInvariant();
                        var hashString = PosDefinition.TerminalID + GetOrderID(payment) + GetAmount(payment.Amount) + PosDefinition.SuccessUrl + PosDefinition.ErrorUrl + GetPaymentType(payment) + installment + PosDefinition.StoreKey + securityData;
                        calculatedHash = GetSHA1(hashString).ToUpperInvariant();

                        payment.IsSuccess = mdStatus >= 1 && mdStatus <= 4 && procReturnCode == "00" && secure3dHash == calculatedHash;
                        if (!payment.IsSuccess)
                        {
                            message = "3D Ödeme Doğrulaması Başarısz! " + message;
                            return Result.Error(message, payment);
                        }
                        return ProcessXml(payment, new CreditCard()); //Kredi kartı bilgileri boş gidecek
                    }

                    if (!string.IsNullOrEmpty(hashParams))
                    {
                        var hash = bankResponse.Value("hash");
                        var digestValue = string.Empty;
                        var paramList = hashParams.Split(":");

                        foreach (var param in paramList)
                        {
                            digestValue += bankResponse.Value(param);
                        }

                        digestValue += PosDefinition.StoreKey;
                        calculatedHash = GetSHA1(digestValue).ToUpperInvariant();

                        payment.IsSuccess = mdStatus >= 1 && mdStatus <= 4 && procReturnCode == "00" && hash == calculatedHash;
                        if (payment.IsSuccess)
                        {
                            return Result.Success(payment, string.Empty, payment.OrderID.Int());
                        }
                        message = "Bankadan dönen cevap onaylanmadı! " + message;
                    }
                    return Result.Error(message, payment);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GarantiPaymentProvider > Get3DResult - Failed : {0}", ex, bankResponse.LogString());
            }
            return Result.Error();
        }

        protected override Result ToPaymentResult(OnlineSales payment, XmlDocument documentResponse)
        {
            try
            {
                payment.Code = GetNodeValue(documentResponse, "//Response//Code");
                payment.ReasonCode = GetNodeValue(documentResponse, "//Response//ReasonCode");
                payment.Message = GetNodeValue(documentResponse, "//Response//Message");
                payment.ErrorMsg = GetNodeValue(documentResponse, "//Response//ErrorMsg");
                payment.SysErrMsg = GetNodeValue(documentResponse, "//Response//SysErrMsg");

                payment.RetrefNum = GetNodeValue(documentResponse, "//RetrefNum");
                payment.AuthCode = GetNodeValue(documentResponse, "//AuthCode");
                payment.ProvDate = GetNodeValue(documentResponse, "//ProvDate");
                payment.HashData = GetNodeValue(documentResponse, "//HashData");

                var password = payment.Type == TransactionType.Sales ? PosDefinition.Password : PosDefinition.RefundPassword;
                var securityData = GetSHA1(password + PosDefinition.TerminalID.PadLeft(9, '0')).ToUpperInvariant();
                var hashString = payment.Code + payment.RetrefNum + payment.AuthCode + payment.ProvDate + GetOrderID(payment) + securityData;
                var hashData = GetSHA1(hashString).ToUpperInvariant();

                payment.IsSuccess = (hashData == payment.HashData && payment.Code == "00");
                return Result.Success(payment, string.Empty, payment.OrderID.Int());
            }
            catch (Exception ex)
            {
                Logger.Error("GarantiPaymentProvider > ToPaymentResult - Failed", ex);
            }
            return Result.Error();
        }

        protected override string GetAmount(decimal amount)
        {
            //Garanti için ., karakterleri olmayacak. Örn: 175,95 olan tutar 17595 olarak gönderilmelidir. 
            //Garanti son 2 haneyi kuruş olarak görüyor

            if (PosDefinition.IsTest) { return "127"; } //Test modu için 1.27 TL, USD, EURO
            return amount.ToString("N").Replace(".", "").Replace(",", "");
        }

        protected override string GetExpireDate(CreditCard card)
        {
            if (PosDefinition.Is3DSecure)
            {
                return string.Empty;
            }

            var result = string.Format("{0}{1}",
                                card.ExpireMonth.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                 card.ExpireYear.ToString(CultureInfo.InvariantCulture).Substring(2, 2));
            return result;
        }

        protected override string GetPaymentType(OnlineSales payment)
        {
            switch (payment.Type)
            {
                case TransactionType.Sales:
                    return "sales";
                case TransactionType.Cancel:
                    return "void";
                case TransactionType.Refund:
                    return "refund";
                case TransactionType.PreSale:
                    return "preauth";
                case TransactionType.PostSale:
                    return "postauth ";
            }
            return string.Empty;
        }

        private string GetInstallment(OnlineSales payment)
        {
            //Taksit yoksa boş gönder
            return payment.Installment > 1 ? payment.Installment.ToString() : string.Empty;
        }
    }
}