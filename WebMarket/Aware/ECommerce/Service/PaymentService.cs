using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using Aware.Util;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Aware.Payment;
using Aware.Payment.Model;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly object _salesLock = new object();
        private readonly object _cancelRefundLock = new object();
        private readonly IRepository<OnlineSales> _paymentRepository;
        private readonly IRepository<PosDefinition> _posDefinitionRepository;
        private readonly IRepository<BankInfo> _bankRepository;
        private readonly IRepository<InstallmentInfo> _installmentRepository;
        private readonly IApplication _application;

        public PaymentService(IRepository<PosDefinition> posDefinitionRepository, IRepository<OnlineSales> paymentRepository, IRepository<BankInfo> bankRepository,
            IRepository<InstallmentInfo> installmentRepository, IApplication application)
        {
            _posDefinitionRepository = posDefinitionRepository;
            _paymentRepository = paymentRepository;
            _bankRepository = bankRepository;
            _installmentRepository = installmentRepository;
            _application = application;
        }

        #region Payment Process

        public Result ProcessPosPayment(string uniqueOrderID,OnlineSales salesInfo,  PosDefinition posDefinition, CreditCard cardInfo)
        {
            if (salesInfo != null && posDefinition!=null)
            {
                return ProcessPayment(uniqueOrderID, salesInfo,posDefinition, cardInfo);
            }
            return Result.Error(Resource.Order_CannotPaidWithCreditCard);
        }
        
        public Result ProcessGarantiPay(string uniqueOrderID,OnlineSales salesInfo)
        {
            if (salesInfo != null)
            {
                var posDefinition = _posDefinitionRepository.First(i=>i.PosType== PosType.GarantiPos && i.PaymentMethod== PosPaymentMethod.GarantiPAY);
                return ProcessPayment(uniqueOrderID,salesInfo, posDefinition, null);
            }
            return Result.Error(Resource.Order_CannotPaidWithGarantiPay);
        }

        private Result ProcessPayment(string uniqueOrderID, OnlineSales salesInfo, PosDefinition posDefinition, CreditCard cardInfo)
        {
            lock (_salesLock)
            {
                var guid = _posDefinitionRepository.StartTransaction();

                try
                {
                    if (salesInfo != null && salesInfo.IsValid() && posDefinition != null)
                    {
                        _application.Log.Info("PaymentService > ProcessPayment - Start for orderID:{0}, with sales : {1}", salesInfo.OrderID, salesInfo);
                        var paymentProvider = GetPosPaymentProvider(posDefinition, uniqueOrderID);
                        if (paymentProvider == null)
                        {
                            throw new Exception("Ödeme için sanal pos bulunamadı!");
                        }

                        Result result = null;
                        if (posDefinition.PaymentMethod == PosPaymentMethod.GarantiPAY)
                        {
                            var paymentForm = paymentProvider.Get3DForm(salesInfo,null);
                            if (!string.IsNullOrEmpty(paymentForm))
                            {
                                result = Result.Success(paymentForm, Resource.General_Success, 1);
                            }
                        }
                        else
                        {
                            if (!posDefinition.IsOOSPayment && (cardInfo == null || !cardInfo.IsValid().OK))
                            {
                                var message = cardInfo != null ? cardInfo.IsValid().Message : Resource.Card_InvalidCardNumber;
                                throw new Exception(message);
                            }

                            if (posDefinition.PaymentMethod == PosPaymentMethod.XmlApi)
                            {
                                result = paymentProvider.ProcessXml(salesInfo, cardInfo);
                                if (result.OK)
                                {
                                    var onlineSale = result.ValueAs<OnlineSales>();
                                    _paymentRepository.Add(onlineSale);
                                    result = Result.Success(onlineSale);
                                }
                            }
                            else
                            {
                                var paymentForm = paymentProvider.Get3DForm(salesInfo, cardInfo);
                                if (!string.IsNullOrEmpty(paymentForm))
                                {
                                    result = Result.Success(paymentForm, Resource.General_Success, 1);
                                }
                            }
                        }

                        if (result != null && result.OK)
                        {
                            _posDefinitionRepository.Commit(guid);
                            _application.Log.Info("PaymentService > ProcessPayment - Finish for order : {0}", salesInfo.OrderID);
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _posDefinitionRepository.Rollback(guid);
                    _application.Log.Error("PaymentService > ProcessPayment - Failed", ex);
                }
                return Result.Error(Resource.Order_CannotPaidWithCreditCard);
            }
        }

        public Result RefundPayment(string uniqueOrderID, string ipAddress)
        {
            return RefundCancelPayment(uniqueOrderID, ipAddress, false);
        }

        public Result CancelPayment(string uniqueOrderID, string ipAddress)
        {
            return RefundCancelPayment(uniqueOrderID, ipAddress, true);
        }

        private Result RefundCancelPayment(string uniqueOrderID, string ipAddress, bool isCancel)
        {
            lock (_cancelRefundLock)
            {
                var guid = _posDefinitionRepository.StartTransaction();
                try
                {
                    if (!string.IsNullOrEmpty(uniqueOrderID))
                    {
                        var orderID = Common.GetOrderID(uniqueOrderID);
                        var payment = _paymentRepository.Where(i => i.OrderID == orderID.ToString() && i.Type == TransactionType.Sales && i.IsSuccess).First();
                        if (payment == null || payment.PosID == 0 || string.IsNullOrEmpty(payment.RetrefNum)) { return Result.Error("Bu sipariş No için iade/iptal yapabileceğiniz bir ödeme bulunamadı!"); }

                        var posDefinition = _posDefinitionRepository.Get(payment.PosID);
                        var onlinePos = GetPosPaymentProvider(posDefinition, uniqueOrderID);
                        if (onlinePos == null) { return Result.Error(); }

                        var paymentModel = payment.Clone();
                        paymentModel.Type = isCancel ? TransactionType.Cancel : TransactionType.Refund;
                        paymentModel.IPAddress = ipAddress;

                        var result = isCancel ? onlinePos.CancelPayment(paymentModel) : onlinePos.RefundPayment(paymentModel);
                        if (result.OK)
                        {
                            var onlineSale = result.ValueAs<OnlineSales>();
                            _paymentRepository.Add(onlineSale);
                        }

                        _posDefinitionRepository.Commit(guid);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _application.Log.Error("PaymentService > RefundCancelPayment - Failed", ex);
                    _posDefinitionRepository.Rollback(guid);
                }
                return Result.Error(Resource.General_Error);
            }
        }

        public string Get3DForm(string uniqueOrderID,OnlineSales salesInfo, PosType posType, PosPaymentMethod paymentMethod, CreditCard card = null)
        {
            lock (_salesLock)
            {
                try
                {
                    if (!string.IsNullOrEmpty(uniqueOrderID) && salesInfo!=null)
                    {
                        var posDefinition = _posDefinitionRepository.Where(i => i.PosType == posType && i.PaymentMethod == paymentMethod).First();
                        var onlinePos = GetPosPaymentProvider(posDefinition, uniqueOrderID);
                        if (onlinePos != null)
                        {
                            var result = onlinePos.Get3DForm(salesInfo, card);
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _application.Log.Error("PaymentService > Get3DForm - Failed", ex);
                }
                return string.Empty;
            }
        }

        public Result Get3DResult(Dictionary<string, string> bankResponse,string uniqueOrderID)
        {
            try
            {
                if (bankResponse != null && bankResponse.Any())
                {
                    var posID = bankResponse.Value("PosID").Int();
                    var posDefinition = _posDefinitionRepository.Get(posID);

                    var onlinePos = GetPosPaymentProvider(posDefinition, uniqueOrderID);
                    if (onlinePos != null)
                    {
                        var result = onlinePos.Get3DResult(bankResponse);
                        if (result.Value != null)
                        {
                            var onlineSale = result.ValueAs<OnlineSales>();
                            _paymentRepository.Add(onlineSale);
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > Get3DResult - Failed", ex);
            }
            return Result.Error(Resource.Payment_BankResponseNotReadable);
        }
        
        #endregion

        #region PosDefinition
        private IPaymentProvider GetPosPaymentProvider(PosDefinition definition,string uniqueOrderID)
        {
            if (definition != null && !string.IsNullOrEmpty(uniqueOrderID))
            {
                definition.SuccessUrl = definition.SuccessUrl.Replace("#OID#", uniqueOrderID);
                definition.ErrorUrl = definition.ErrorUrl.Replace("#OID#", uniqueOrderID);

                switch (definition.PosType)
                {
                    case PosType.GarantiPos:
                        return new GarantiPaymentProvider(definition);
                    case PosType.AkbankPos:
                        return new EstPaymentProvider(definition);
                    case PosType.Isbank:
                        return new EstPaymentProvider(definition);
                }
            }
            return null;
        }

        public PosDefinition GetPosDefinition(Expression<Func<PosDefinition, bool>> filter)
        {
            try
            {
                if (filter!=null)
                {
                    return _posDefinitionRepository.First(filter);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > GetPosDefinition - failed", ex);
            }
            return null;
        }

        public List<PosDefinition> GetPosDefinitions(bool discardTestPos = false)
        {
            try
            {
                if (discardTestPos)
                {
                    return _posDefinitionRepository.Where(i => i.IsTest == false).ToList();
                }
                return _posDefinitionRepository.GetAll();
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > GetPosDefinitions - failed", ex);
            }
            return null;
        }

        public PosDefinitionDetailModel GetPosDefinitionDetail(int posDefinitionID)
        {
            try
            {
                var result = new PosDefinitionDetailModel()
                {
                    PosDefinition = posDefinitionID > 0 ? _posDefinitionRepository.Get(posDefinitionID) : new PosDefinition(),
                    PosTypeList = _application.Lookup.GetLookups(LookupType.PosTypes),
                    PaymentMethodList = _application.Lookup.GetLookups(LookupType.PosPaymentMethods)
                };
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > GetPosDefinition - failed", ex);
            }
            return null;
        }

        public Result SavePosDefinition(PosDefinition model)
        {
            try
            {
                if (model == null) { return null; }
                if (model.ID > 0)
                {
                    var posDefinition = _posDefinitionRepository.Get(model.ID);
                    if (posDefinition != null)
                    {
                        Mapper.Map(ref posDefinition, model);
                        _posDefinitionRepository.Update(posDefinition);
                        model = posDefinition;
                    }
                }
                else
                {
                    _posDefinitionRepository.Add(model);
                }
                return Result.Success(model, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > Save - Fail with ID:{0}", ex, model.ID);
            }
            return Result.Error(Resource.General_Error);
        }
        #endregion

        #region BankInfo

        public List<BankInfo> GetBankList(Statuses status = Statuses.None)
        {
            if (status != Statuses.None)
            {
                return _bankRepository.Where(i => i.Status == status).ToList();
            }
            return _bankRepository.GetAll();
        }

        public Result SaveBankInfo(BankInfo model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID <= 0)
                    {
                        _bankRepository.Add(model);
                        return Result.Success(model.ID, Resource.General_Success);
                    }

                    var bank = _bankRepository.Get(model.ID);
                    if (bank != null)
                    {
                        Mapper.Map(ref bank, model);
                        _bankRepository.Update(bank);
                        return Result.Success(bank.ID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > SaveBankInfo - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteBankInfo(int itemID)
        {
            try
            {
                if (itemID > 0)
                {
                    _bankRepository.Delete(itemID);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > DeleteBankInfo - Failed for id:{0}", ex, itemID);
            }
            return Result.Error(Resource.General_Error);
        }

        #endregion

        #region Installments & CardInfo
        public CardInfo GetCardInfo(string binNumber, int posID)
        {
            if (!string.IsNullOrEmpty(binNumber))
            {
                var infoList = _application.Cacher.Get<List<CardInfo>>(Constants.CK_BinMumbers);
                if (infoList == null)
                {
                    infoList = ReadBinList();
                    _application.Cacher.Add(Constants.CK_BinMumbers, infoList);
                }

                if (infoList != null && infoList.Any())
                {
                    var convertedBin = binNumber.Short(6, string.Empty);
                    var result = infoList.FirstOrDefault(i => i.BinNumber == convertedBin);
                    if (result != null)
                    {
                        var posType = (PosType)result.PosType;
                        var posDefinition = _application.Order.PosList.FirstOrDefault(i => i.PosType == posType);
                        if (posDefinition != null)
                        {
                            var installments = GetCachedInstallments(posDefinition.ID);
                            result.PosID = posDefinition.ID;
                            result.Installments = installments;
                        }
                        else //Varsayılan pos üzerinden devam et
                        {
                            result.PosID = _application.Order.DefaultPosID;
                        }
                        return result;
                    }
                }
            }
            else if (posID > 0)
            {
                var posDefinition = _application.Order.PosList.FirstOrDefault(i => i.ID == posID);
                if (posDefinition != null)
                {
                    var installments = GetCachedInstallments(posDefinition.ID);
                    var result = new CardInfo()
                    {
                        PosID = posDefinition.ID,
                        Installments = installments
                    };
                    return result;
                }
            }
            return null;
        }

        public List<InstallmentInfo> GetCachedInstallments(int posID = 0)
        {
            var result = _application.Cacher.Get<List<InstallmentInfo>>(Constants.CK_Installments);
            if (result == null)
            {
                result = GetInstallmentList(Statuses.Active).OrderBy(i => i.Count).ToList();
                _application.Cacher.Add(Constants.CK_Installments, result);
            }

            if (posID > 0)
            {
                return result.Where(i => i.PosID == posID).ToList();
            }
            return result;
        }

        public List<InstallmentInfo> GetInstallmentList(Statuses status = Statuses.None)
        {
            if (status != Statuses.None)
            {
                return _installmentRepository.Where(i => i.Status == status).ToList();
            }
            return _installmentRepository.GetAll();
        }

        public Result SaveInstallmentInfo(InstallmentInfo model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID <= 0)
                    {
                        model.Status = Statuses.Active;
                        _installmentRepository.Add(model);
                        return Result.Success(model.ID, Resource.General_Success);
                    }

                    var installment = _installmentRepository.Get(model.ID);
                    if (installment != null)
                    {
                        Mapper.Map(ref installment, model);
                        _installmentRepository.Update(installment);
                        return Result.Success(installment.ID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > SaveInstallmentInfo - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteInstallmentInfo(int itemID)
        {
            try
            {
                if (itemID > 0)
                {
                    _installmentRepository.Delete(itemID);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PaymentService > DeleteInstallmentInfo - Failed for id:{0}", ex, itemID);
            }
            return Result.Error(Resource.General_Error);
        }

        private List<CardInfo> ReadBinList()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Aware.Payment.binlist.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string binInfo = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<List<CardInfo>>(binInfo);
                return result;
            }
        }

        #endregion

    }
}