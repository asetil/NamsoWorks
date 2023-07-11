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
using System.Xml;
using Aware.Payment.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Payment
{
    public class YapiKrediPaymentProvider : BasePaymentProvider
    {
        public YapiKrediPaymentProvider(PosDefinition definition)
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
                Logger.Error("BasePosPayment > ProcessXml - Failed", ex);
            }
            return null;
        }

        public override string Get3DForm(OnlineSales payment, CreditCard card)
        {
            throw new NotImplementedException();
        }

        protected override XmlDocument GetXmlData(OnlineSales payment, CreditCard card, string hashData)
        {
            var doc = new XmlDocument();
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
            doc.AppendChild(xmlDeclaration);

            var posnetRequest = doc.CreateElement("posnetRequest");
            doc.AppendChild(posnetRequest);

            var isRefund = IsRefund(payment);
            AddData(doc, posnetRequest, "tid", PosDefinition.TerminalID);
            AddData(doc, posnetRequest, "mid", PosDefinition.MerchantID);

            var sale = AddData(doc, posnetRequest, "sale", string.Empty);
            if (!isRefund && card != null)
            {
                AddData(doc, sale, "ccno", card.CardNumber);
                AddData(doc, sale, "expDate", GetExpireDate(card));
                AddData(doc, sale, "cvc", PosDefinition.Is3DSecure ? string.Empty : card.CVC);
            }

            var orderID = string.Format("{0}{1}", PosDefinition.IsTest ? Guid.NewGuid().ToString() : string.Empty, GetOrderID(payment));
            AddData(doc, sale, "orderID", orderID);
            AddData(doc, sale, "currencyCode", ((int)payment.CurrencyCode).ToString());
            AddData(doc, sale, "amount", GetAmount(payment.Amount));

            if (payment.Installment > 1)
            {
                AddData(doc, sale, "amount", GetInstallment(payment));
            }

            return doc;
        }

        public override Result Get3DResult(Dictionary<string, string> bankResponse)
        {
            throw new NotImplementedException();
        }

        protected override Result ToPaymentResult(OnlineSales payment, XmlDocument documentResponse)
        {
            try
            {
                payment.Code = GetNodeValue(documentResponse, "//posnetResponse//approved");
                payment.ReasonCode = GetNodeValue(documentResponse, "//Response//ReasonCode");
                payment.Message = GetNodeValue(documentResponse, "//posnetResponse//respText");
                payment.ErrorMsg = GetNodeValue(documentResponse, "//posnetResponse//respCode");
                payment.AuthCode = GetNodeValue(documentResponse, "//posnetResponse//AuthCode");

                payment.IsSuccess = payment.Code == "1";
                return Result.Success(payment, string.Empty, payment.OrderID.Int());
            }
            catch (Exception ex)
            {
                Logger.Error("YapıKrediPayment > ToPaymentResult - Failed", ex);
            }
            return Result.Error();
        }

        protected override string GetAmount(decimal amount)
        {
            if (PosDefinition.IsTest) { return "127"; } //Test modu için 1.27 TL, USD, EURO
            return amount.ToString("N").Replace(".", "").Replace(",", "");
        }

        protected override string GetExpireDate(CreditCard card)
        {
            if (PosDefinition.Is3DSecure)
            {
                return string.Empty;
            }

            var result = string.Format("{0}{1}", card.ExpireYear.ToString().Substring(2, 2), card.ExpireMonth.ToString().PadLeft(2, '0'));
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

        private string ToCurrencyString(CurrencyCode currencyCode)
        {
            switch (currencyCode)
            {
                case CurrencyCode.TRL:
                    return "YT";
                default:
                    return "YT";
            }
        }
    }
}