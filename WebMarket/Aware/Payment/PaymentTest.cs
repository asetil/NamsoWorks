using System;
using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Interface;
using Aware.Payment.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Payment
{
    public class PaymentTest
    {
        private readonly IPaymentService _paymentService;

        public PaymentTest(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Result XML_SalesTest(string uniqueOrderID, PosType posType, decimal amount = 1.27M, int installment = 0)
        {
            var card = CardList.FirstOrDefault(i => i.Key == (posType.ToString() + "_XML_Sales")).Value;
            var saleInfo = GetSalesInfo(uniqueOrderID, amount, installment);

            var posDefinition=_paymentService.GetPosDefinition(i => i.PosType == posType && i.PaymentMethod == PosPaymentMethod.XmlApi);
            var result = _paymentService.ProcessPosPayment(uniqueOrderID,saleInfo, posDefinition,card);
            return result;
        }

        public string Secure_3D_PAY_Test(string uniqueOrderID, PosType posType, decimal amount = 1.27M, int installment = 0)
        {
            var card = CardList.FirstOrDefault(i => i.Key == (posType.ToString() + "_3DPAY")).Value;
            var saleInfo = GetSalesInfo(uniqueOrderID, amount, installment);

            var result = _paymentService.Get3DForm(uniqueOrderID, saleInfo, posType, PosPaymentMethod.Secure3D_PAY, card);
            return result;
        }

        public string Secure_3D_PAY_Test(string uniqueOrderID, PosType posType, CreditCard card,  decimal amount = 1.27M, int installment = 0)
        {
            var saleInfo = GetSalesInfo(uniqueOrderID, amount, installment);
            var result = _paymentService.Get3DForm(uniqueOrderID,saleInfo, posType, PosPaymentMethod.Secure3D_PAY, card);
            return result;
        }

        public string Secure_3D_OOS_Test(string uniqueOrderID, PosType posType, decimal amount = 1.27M, int installment = 0)
        {
            var card = CardList.FirstOrDefault(i => i.Key == (posType.ToString() + "_3DOOS")).Value;
            var saleInfo = GetSalesInfo(uniqueOrderID, amount, installment);

            var result = _paymentService.Get3DForm(uniqueOrderID,saleInfo, posType, PosPaymentMethod.Secure3D_OOS, card);
            return result;
        }

        public string Garanti3DForm(string uniqeOrderID)
        {
            var card = CardList.FirstOrDefault(i => i.Key == "Garanti_XML_Sales").Value;
            var saleInfo = GetSalesInfo(uniqeOrderID);

            var result = _paymentService.Get3DForm(uniqeOrderID,saleInfo, PosType.GarantiPos, PosPaymentMethod.Secure3D, card);
            return result;
        }


        public string Garanti_3D_OOS_Test(string uniqeOrderID)
        {
            var card = CardList.FirstOrDefault(i => i.Key == "Garanti_XML_Sales").Value;
            var saleInfo = GetSalesInfo(uniqeOrderID, 1.47M);

            var result = _paymentService.Get3DForm(uniqeOrderID,saleInfo, PosType.GarantiPos, PosPaymentMethod.Secure3D_OOS, card);
            return result;
        }

        public Result CancelTest(string uniqueOrderID)
        {
            var result = _paymentService.CancelPayment(uniqueOrderID, "127.0.0.1");
            return result;
        }

        public Result RefundTest(string uniqueOrderID)
        {
            var result = _paymentService.RefundPayment(uniqueOrderID, "127.0.0.1");
            return result;
        }

        private Dictionary<string, CreditCard> _cardList;
        private Dictionary<string, CreditCard> CardList
        {
            get
            {
                if (_cardList == null)
                {
                    _cardList = new Dictionary<string, CreditCard>();
                    _cardList.Add("AkbankPos_XML_Sales", new CreditCard() { CardHolder = "Osman Sokuoğlu", CardNumber = "5571135571135575", ExpireMonth = 12, ExpireYear = 2018, CVC = "000" });
                    _cardList.Add("AkbankPos_3DOOS", new CreditCard() { CardHolder = "Osman Sokuoğlu", CardNumber = "5571135571135575", ExpireMonth = 12, ExpireYear = 2018, CVC = "000" });
                    _cardList.Add("AkbankPos_3DPAY", new CreditCard() { CardHolder = "Osman Sokuoğlu", CardNumber = "5571135571135575", ExpireMonth = 12, ExpireYear = 2018, CVC = "000" });
                    _cardList.Add("GarantiPos_XML_Sales", new CreditCard() { CardHolder = "Osman Sokuoğlu", CardNumber = "4824894728063019", ExpireMonth = 6, ExpireYear = 2017, CVC = "959" });
                }
                return _cardList;
            }
        }

        private OnlineSales GetSalesInfo(string uniqueOrderID, decimal amount = 1.27M, int installment = 0)
        {
            var orderID = Common.GetOrderID(uniqueOrderID)>0 ? Common.GetOrderID(uniqueOrderID) : new Random().Next(1001, 99999);
            var saleInfo = new OnlineSales()
            {
                OrderID = "Aware_" + orderID,
                UserID = 101,
                Amount = amount,
                Installment = installment,
                Type = TransactionType.Sales,
                CurrencyCode = CurrencyCode.TRL,
                IPAddress = "127.0.0.1",
                Email = "osman.sokuoglu@gmail.com"
            };
            return saleInfo;
        }
    }
}


/*
 * =====================================================================
 * GARANTİ GERÇEK ORTAM
 * =====================================================================
 * raporlama için: https://sanalposweb.garanti.com.tr
 * 
 * provizyon için: https://sanalposprov.garanti.com.tr/VPServlet
                   https://sanalposprov.garanti.com.tr/servlet/gt3dengine

 * 
 * =====================================================================
 * GARANTİ TEST ORTAM
 * =====================================================================
 * raporlama için: https://sanalposwebtest.garanti.com.tr
 * 
 * provizyon için: https://sanalposprovtest.garanti.com.tr/VPServlet
                   https://sanalposprovtest.garanti.com.tr/servlet/gt3dengine

 * Test terminal : 111995
 * İşyeri no : 600218
 * Provision User : PROVAUT
 * Provision Password: 123qweASD
 * Securekey : 12345678

 * Raporlar ekranına
 * işyerino : 600218
 * Kullanıcı Adı : dene
 * Parola : deneme
 * Şifre : 123qweASD
 * 
 * 
 */