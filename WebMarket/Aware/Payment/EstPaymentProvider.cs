/**************************************************************************************************
 *
 *    GERÇEK ÇEKİMLER İÇİN
 *      XML_URL => Normal : https://www.sanalakpos.com/servlet/cc5ApiServer
 *      3D Secure : https://sanalposprov.garanti.com.tr/servlet/gt3dengine 
 *
 *    TEST İÇİN
 *      XML_URL => Normal : https://testsanalpos.est.com.tr/servlet/cc5ApiServer
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
    public class EstPaymentProvider : BasePaymentProvider
    {
        public EstPaymentProvider(PosDefinition definition)
            : base(definition)
        {
        }

        public override Result ProcessXml(OnlineSales payment, CreditCard card)
        {
            try
            {
                card = card ?? new CreditCard();
                payment.PosID = PosDefinition.ID;

                var result = PostXmlData(payment, card, string.Empty);
                return result;
            }
            catch (Exception ex)
            {

            }
            return Result.Error();
        }
        public override string Get3DForm(OnlineSales payment, CreditCard card)
        {
            try
            {
                var orderID = GetOrderID(payment);
                var installment = GetInstallment(payment);
                var amount = GetAmount(payment.Amount);
                var rnd = DateTime.Now.Ticks.ToString();

                var extraHashParams = PosDefinition.PaymentMethod == PosPaymentMethod.Secure3D ? string.Empty : GetPaymentType(payment) + installment;
                var hashString = PosDefinition.TerminalID + orderID + amount + PosDefinition.SuccessUrl + PosDefinition.ErrorUrl + extraHashParams + rnd + PosDefinition.StoreKey;
                var hashData = ComputeSHA1(hashString);

                var secureLevel = "3d";
                switch (PosDefinition.PaymentMethod)
                {
                    case PosPaymentMethod.Secure3D_PAY:
                        secureLevel = "3d_pay"; break;
                    case PosPaymentMethod.Secure3D_OOS:
                        secureLevel = "3d_pay_hosting"; break;
                    case PosPaymentMethod.OOS_PAY:
                        secureLevel = "pay_hosting"; break;
                }

                var formHtml = new StringBuilder();
                formHtml.AppendFormat("<form id='PaymentForm' action='{0}' method='POST'>", PosDefinition.PostUrl);
                formHtml.AppendFormat("<input type='hidden' name='clientid' id='clientid' value='{0}'/>", PosDefinition.TerminalID);
                formHtml.AppendFormat("<input type='hidden' name='storetype' id='storetype' value='{0}'/>", secureLevel);
                formHtml.AppendFormat("<input type='hidden' name='hash' id='hash' value='{0}'/>", hashData);

                if (card != null && PosDefinition.PaymentMethod != PosPaymentMethod.Secure3D_OOS)
                {
                    formHtml.AppendFormat("<input type='hidden' name='pan' id='pan' value='{0}'/>", card.CardNumber);
                    formHtml.AppendFormat("<input type='hidden' name='Ecom_Payment_Card_ExpDate_Month' id='Ecom_Payment_Card_ExpDate_Month' value='{0}'/>", card.ExpireMonth.ToString().PadLeft(2, '0'));
                    formHtml.AppendFormat("<input type='hidden' name='Ecom_Payment_Card_ExpDate_Year' id='Ecom_Payment_Card_ExpDate_Year'value='{0}'/>", card.ExpireYear.ToString().PadLeft(2, '0'));
                    formHtml.AppendFormat("<input type='hidden' name='cv2' id='cv2'value='{0}'/>", card.CVC);
                    formHtml.AppendFormat("<input type='hidden' name='cardType' id='cardType'value='{0}'/>", 2); //1:Visa, 2:MasterCard, Maestro = 3, Troy =4
                }

                formHtml.AppendFormat("<input type='hidden' name='islemtipi' id='islemtipi' value='{0}'/>", GetPaymentType(payment));
                formHtml.AppendFormat("<input type='hidden' name='amount' id='amount' value='{0}'/>", amount);
                formHtml.AppendFormat("<input type='hidden' name='taksit' id='taksit' value='{0}'/>", installment);

                formHtml.AppendFormat("<input type='hidden' name='currency' id='currency' value='{0}'/>", (int)payment.CurrencyCode);
                formHtml.AppendFormat("<input type='hidden' name='oid' id='oid' value='{0}'/>", orderID);
                formHtml.AppendFormat("<input type='hidden' name='okUrl' id='okUrl' value='{0}'/>", PosDefinition.SuccessUrl);
                formHtml.AppendFormat("<input type='hidden' name='failUrl' id='failUrl' value='{0}'/>", PosDefinition.ErrorUrl);
                formHtml.AppendFormat("<input type='hidden' name='lang' id='lang' value='tr'/>");
                formHtml.AppendFormat("<input type='hidden' name='rnd' id='rnd' value='{0}'/>", rnd);

                formHtml.AppendFormat("<input type='hidden' name='firmaadi' id='firmaadi' value='{0}'/>", PosDefinition.Name);
                formHtml.AppendFormat("<input type='hidden' name='userid' id='userid' value='{0}'/>", payment.UserID);
                formHtml.AppendFormat("<input type='hidden' name='email' id='email' value='{0}'/>", payment.Email);
                formHtml.AppendFormat("<input type='hidden' name='customeripaddress' id='customeripaddress' value='{0}'/>", payment.IPAddress);
                formHtml.AppendFormat("<input type='hidden' name='refreshtime' id='refreshtime' value='5'/>");
                formHtml.AppendFormat("<input type='hidden' name='PosID' id='PosID' value='{0}'/>", PosDefinition.ID);

                formHtml.AppendFormat("</form>");
                return formHtml.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Garanti Pos > Get3DForm - Failed", ex);
            }
            return string.Empty;
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
                        Amount = bankResponse.Value("amount").Dec(),
                        Type = TransactionType.Sales,
                        Installment = bankResponse.Value("taksit").Int(),
                        UserID = bankResponse.Value("userid").Int(),
                        Email = bankResponse.Value("email"),
                        IPAddress = bankResponse.Value("customeripaddress"),
                        PosID=PosDefinition.ID,

                        Message = bankResponse.Value("Response"),
                        RetrefNum = bankResponse.Value("HostRefNum"),
                        AuthCode = bankResponse.Value("AuthCode"),
                        Code = bankResponse.Value("ProcReturnCode"),
                        ErrorMsg = bankResponse.Value("ErrMsg"),
                        BankResponse = new BankResponse
                        {
                            MdStatus = bankResponse.Value("mdStatus").Int(),
                            Md = bankResponse.Value("md"),
                            Cavv = bankResponse.Value("cavv"),
                            Eci = bankResponse.Value("eci"),
                            Xid = bankResponse.Value("xid"),
                            ErrorMessage = bankResponse.Value("ErrMsg"),
                            MdErrorMessage = bankResponse.Value("mdErrorMsg")
                        }
                    };

                    //CHECK RESPONSE
                    var mdStatus = payment.BankResponse.MdStatus;
                    var hashParams = bankResponse.Value("HASHPARAMS");
                    var hashParamsVal = bankResponse.Value("HASHPARAMSVAL");

                    var digestValue = string.Empty;
                    var paramList = hashParams.Split(":");
                    foreach (var param in paramList)
                    {
                        digestValue += bankResponse.Value(param);
                    }

                    var hash = bankResponse.Value("HASH");
                    var calculatedHash = ComputeSHA1(digestValue + PosDefinition.StoreKey);

                    payment.IsSuccess = payment.Code == "00" && digestValue == hashParamsVal && mdStatus >= 1 && mdStatus <= 4 && hash == calculatedHash;
                    if (payment.IsSuccess)
                    {
                        if (PosDefinition.PaymentMethod == PosPaymentMethod.Secure3D)
                        {
                            return ProcessXml(payment, new CreditCard()); //Kredi kartı bilgileri boş gidecek
                        }
                        return Result.Success(payment, string.Empty, payment.OrderID.Int());
                    }

                    var message = Resource.Order_CannotPaidWithCreditCard;
                    if (!string.IsNullOrEmpty(payment.Message) || !string.IsNullOrEmpty(payment.ErrorMsg))
                    {
                        message = string.Format("{0}:{1}-{2}", payment.Message, payment.ErrorMsg, payment.SysErrMsg);
                    }
                    return Result.Error(message, payment);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ESTPaymentProvider > Get3DResult - Failed : {0}", ex, bankResponse.LogString());
            }
            return Result.Error();
        }

        protected override XmlDocument GetXmlData(OnlineSales payment, CreditCard card, string hashData)
        {
            var doc = new XmlDocument();
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
            doc.AppendChild(xmlDeclaration);

            var xmlRequest = doc.CreateElement("CC5Request");
            doc.AppendChild(xmlRequest);

            AddData(doc, xmlRequest, "Name", PosDefinition.UserID);
            AddData(doc, xmlRequest, "Password", PosDefinition.Password);
            AddData(doc, xmlRequest, "ClientId", PosDefinition.TerminalID); //Üye iş yeri numarası
            AddData(doc, xmlRequest, "Type", GetPaymentType(payment)); //Auth, PreAuth, PostAuth, Void, Credit
            AddData(doc, xmlRequest, "Mode", PosDefinition.IsTest ? "T" : "P");

            AddData(doc, xmlRequest, "IPAddress", payment.IPAddress);
            AddData(doc, xmlRequest, "Email", payment.Email);

            var orderID = string.IsNullOrEmpty(payment.OrderID) ? Guid.NewGuid().ToString() : GetOrderID(payment);
            AddData(doc, xmlRequest, "OrderId", orderID);
            AddData(doc, xmlRequest, "GroupID", " ");
            AddData(doc, xmlRequest, "TransId", string.Empty); //İşlem numarası
            AddData(doc, xmlRequest, "Total", GetAmount(payment.Amount));
            AddData(doc, xmlRequest, "Currency", ((int)payment.CurrencyCode).ToString());
            AddData(doc, xmlRequest, "UserId", string.Empty);

            if (payment.Installment > 1)
            {
                AddData(doc, xmlRequest, "Taksit", GetInstallment(payment));
            }

            if (card != null && !PosDefinition.Is3DSecure && !IsRefund(payment))
            {
                AddData(doc, xmlRequest, "Number", card.CardNumber);
                AddData(doc, xmlRequest, "Expires", GetExpireDate(card));
                AddData(doc, xmlRequest, "Cvv2Val", card.CVC);
                AddData(doc, xmlRequest, "CardholderPresentCode", string.Empty);
            }
            else if (PosDefinition.Is3DSecure)
            {
                AddData(doc, xmlRequest, "CardholderPresentCode", "13");
            }

            if (payment.BankResponse != null)
            {
                AddData(doc, xmlRequest, "PayerSecurityLevel", payment.BankResponse.Eci);
                AddData(doc, xmlRequest, "PayerTxnId", payment.BankResponse.Xid);
                AddData(doc, xmlRequest, "PayerAuthenticationCode", payment.BankResponse.Cavv);
                AddData(doc, xmlRequest, "Number", payment.BankResponse.Md);
            }
            return doc;
        }

        protected override Result ToPaymentResult(OnlineSales payment, XmlDocument documentResponse)
        {
            try
            {
                payment.Code = GetNodeValue(documentResponse, "//CC5Response//ProcReturnCode");
                payment.Message = GetNodeValue(documentResponse, "//CC5Response//Response");
                payment.ErrorMsg = GetNodeValue(documentResponse, "//CC5Response//ErrMsg");
                payment.RetrefNum = GetNodeValue(documentResponse, "//CC5Response//HostRefNum");
                payment.AuthCode = GetNodeValue(documentResponse, "//CC5Response//AuthCode");
                payment.IsSuccess = payment.Code == "00" && payment.Message == "Approved";
                return Result.Success(payment, string.Empty, payment.OrderID.Int());
            }
            catch (Exception)
            {
            }
            return Result.Error();
        }

        private string GetInstallment(OnlineSales payment)
        {
            //Taksit yoksa boş gönder
            return payment.Installment > 1 ? payment.Installment.ToString() : string.Empty;
        }

        protected override string GetExpireDate(CreditCard card)
        {
            string result = string.Format("{0}/{1}", card.ExpireMonth.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), card.ExpireYear.ToString(CultureInfo.InvariantCulture).Substring(2, 2));
            return result;
        }

        protected override string GetPaymentType(OnlineSales payment)
        {
            switch (payment.Type)
            {
                case TransactionType.Sales:
                    return "Auth";
                case TransactionType.Cancel:
                    return "void";
                case TransactionType.Refund:
                    return "Credit";
                case TransactionType.PreSale:
                    return "PreAuth";
                case TransactionType.PostSale:
                    return "PostAuth";
            }
            return string.Empty;
        }

        private string ComputeSHA1(string data)
        {
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(data);
            byte[] inputbytes = sha.ComputeHash(hashbytes);

            var hashData = Convert.ToBase64String(inputbytes);   //Günvelik amaçlı oluşturulan hash
            return hashData;
        }
    }
}

/*
 Hash için Plain Text Oluşturma
Hash hesaplamasında kullanılan parametreler şöyledir: clientid, oid, AuthCode, 
ProcReturnCode, Response, rnd, md, eci, cavv, mdStatus.
İşlemin tipine göre aşağıdaki parametrelerin bir alt kümesi hash nesil olarak dahil 
edilecektir:
• 3D olmayan kart işlemleri 
clientid, oid, AuthCode, ProcReturnCode, Response, rnd
• 3D güvenlikli kart işlemleri 
clientid, oid, AuthCode, ProcReturnCode, Response, mdStatusi eci, cavv ,md, rnd
Bu parametrelerin yerini tutan tüm değerler aynı sırayla eklenir. Sonuç dizesi 
HASHPARAMSVAL parametre değerleri aynı olacaktır. Üye iş yeri şifresi bu dizenin 
sonuna nihai bir değer olarak eklenir. Ortaya çıkan hash 'lenmiş metin SHA1 
algoritmasına göre base64 versiyonu ile kodlanmıştır. Normal şartlar altında üretilen 
hash metni Nestpay tarafından yayınlanan HASH parametre değeri ile aynı olmalıdır. 
Aksi takdirde üye iş yeri Nestpay destek takımı ile iletişime geçmelidir.
Örnek: 3D olmayan kart işlemleri
İşlemin yanıt parametreleri olduğunu varsayarak:
clientid, oid, AuthCode, ProcReturnCode, Response, rnd
HASHPARAMSVAL: 990000000000001129189941142132165400Approvedasdf
HASHPARAMS: clientid:oid:ProcReturnCode:Response:rnd:
HASH: CVJssbkrhIzqZXVTwGobciDZI+A=
Üye iş yeri hash metni; clientid, oid, ProcReturnCode, Response, rnd (ve üye iş yerinin 
gizli hash elementinin storekey'i) ile oluşturulur. Varsayalım storekey 123456 olsun 
hash metni aşağıdaki gibi oluşur:
990000000000001129189941142132165400Approvedasdf123456
Ve üye iş yeri hash metni based64 versiyonuna göre kodlanmıştır (SHA1(plain)). Ortaya 
çıkan hash, HASH parametresinin dönüşündeki hash değeri ile aynı olmalıdır.
Not: Üye iş yeri, bankadan HASHPARAMS & HASHPARAMSVAL & Odeme 
sonucunda dönen HASH'i kendi tarafında kontrol etmelidir.
 */