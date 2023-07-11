using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Aware.Dependency;
using Aware.Payment.Model;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Payment
{
    public abstract class BasePaymentProvider : IPaymentProvider
    {
        private readonly PosDefinition _definition;
        protected BasePaymentProvider(PosDefinition definition)
        {
            _definition = definition;
        }

        public abstract Result ProcessXml(OnlineSales payment, CreditCard card);
        protected abstract XmlDocument GetXmlData(OnlineSales payment, CreditCard card, string hashData);
        protected abstract Result ToPaymentResult(OnlineSales payment, XmlDocument documentResponse);

        public abstract string Get3DForm(OnlineSales payment, CreditCard card);
        public abstract Result Get3DResult(Dictionary<string, string> bankResponse);

        public Result RefundPayment(OnlineSales payment)
        {
            if (payment != null)
            {
                payment.Type = TransactionType.Refund;
                return ProcessXml(payment, new CreditCard());
            }
            return Result.Error();
        }

        public Result CancelPayment(OnlineSales payment)
        {
            if (payment != null)
            {
                payment.Type = TransactionType.Cancel;
                return ProcessXml(payment, new CreditCard());
            }
            return Result.Error();
        }

        /* ========================================================================================= */

        protected Result PostXmlData(OnlineSales payment, CreditCard card, string hashData)
        {
            var xmlData = GetXmlData(payment, card, hashData);
            string data = "data=" + xmlData.OuterXml;

            var request = WebRequest.Create(PosDefinition.XmlUrl);
            request.Method = "POST";
            request.Timeout = 30000;

            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseString = reader.ReadToEnd();
            Logger.Info("Bank Response : {0}", response);

            var doc = new XmlDocument();
            doc.LoadXml(responseString);
            return ToPaymentResult(payment, doc);
        }

        protected XmlElement AddData(XmlDocument doc, XmlElement parentNode, string key, string value)
        {
            XmlElement node = doc.CreateElement(key);
            if (!string.IsNullOrEmpty(value))
            {
                node.AppendChild(doc.CreateTextNode(value));
            }
            parentNode.AppendChild(node);
            return node;
        }

        protected string GetSHA1(string data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }

        private string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }

        protected virtual string GetAmount(decimal amount)
        {
            return amount.ToString("#.##", NumberFormatInfo.InvariantInfo);
        }

        protected virtual string GetOrderID(OnlineSales payment)
        {
            return string.Format("Aware_{0}",payment.OrderID);
        }

        protected abstract string GetPaymentType(OnlineSales payment);

        protected virtual string GetExpireDate(CreditCard card)
        {
            return string.Format("{0}{1}", card.ExpireMonth, card.ExpireYear);
        }

        protected string GetNodeValue(XmlDocument xmlDocument, string xPath)
        {
            if (xmlDocument != null)
            {
                var node = xmlDocument.SelectSingleNode(xPath);
                return node != null ? node.InnerText : string.Empty;
            }
            return string.Empty;
        }

        protected bool IsRefund(OnlineSales payment)
        {
            return payment.Type == TransactionType.Refund || payment.Type == TransactionType.Cancel;
        }

        protected PosDefinition PosDefinition
        {
            get { return _definition; }
        }

        protected ILogger Logger
        {
            get { return WindsorBootstrapper.Resolve<ILogger>(); }
        }
    }
}