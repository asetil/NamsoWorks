using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Payment.Model;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Manager
{
    public class OrderManager : IOrderManager
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IApplication _application;
        private readonly ILogger _logger;

        public OrderManager(IUserService userService, IOrderService orderService, IPaymentService paymentService, IApplication application, ILogger logger)
        {
            _userService = userService;
            _orderService = orderService;
            _paymentService = paymentService;
            _application = application;
            _logger = logger;
        }

        public Result ProcessPayment(int userID, int orderID, CreditCard card, int posID, int installment, string ipAddress)
        {
            try
            {
                if (orderID > 0 && userID > 0)
                {
                    var order = _orderService.GetUserOrder(userID, orderID);
                    if (order != null && order.Status == OrderStatuses.WaitingPayment
                                      && ((order.PaymentType == (int)PaymentType.CreditCard && order.SubPaymentType == posID) || order.PaymentType == (int)PaymentType.GarantiPay))
                    {
                        var user = _userService.GetUser(userID);
                        var salesInfo = new OnlineSales()
                        {
                            PosID = posID,
                            Installment = installment,
                            OrderID = orderID.ToString(),
                            Amount = order.GrossTotal,
                            Type = TransactionType.Sales,
                            CurrencyCode = CurrencyCode.TRL,
                            UserID = user.ID,
                            Email = user.Email,
                            IPAddress = ipAddress
                        };

                        var paymentType = (PaymentType)order.PaymentType;
                        if (paymentType == PaymentType.GarantiPay)
                        {
                            return _paymentService.ProcessGarantiPay(order.UniqueID, salesInfo);
                        }
                        else
                        {
                            var posDefinition = _paymentService.GetPosDefinition(i => i.ID == posID);
                            return _paymentService.ProcessPosPayment(order.UniqueID, salesInfo, posDefinition, card);
                        }
                    }
                }

                _logger.Error("OrderManager > ProcessPayment - order not found with ID:{0} and userID:{1}", null, orderID, userID);
                return Result.Error("Siparişiniz sistemde bulunamadığı için ödeme işlemine devam edilemiyor!");
            }
            catch (Exception ex)
            {
                _logger.Error("OrderManager > ProcessPayment - fail for userID:{0}, orderID:{1}", ex, userID, orderID);
            }
            return Result.Error(Resource.Order_CannotPaidWithCreditCard);
        }

        public CardInfo GetCardInfo(string binNumber, int posID, decimal orderTotal)
        {
            try
            {
                var cardInfo = _paymentService.GetCardInfo(binNumber, posID);
                if (cardInfo != null)
                {
                    cardInfo.OrderTotal = orderTotal;
                }
                return cardInfo;
            }
            catch (Exception ex)
            {
                _logger.Error("OrderManager > GetCardInfo - fail for binNumber:{0}, posID:{1}", ex, binNumber, posID);
            }
            return null;
        }

        public Result HandlePaymentResult(int userID, string uniqueOrderID, Dictionary<string, string> bankResponse)
        {
            try
            {
                if (userID > 0 && !string.IsNullOrEmpty(uniqueOrderID) && bankResponse != null && bankResponse.Any())
                {
                    var logString = string.Join(", ", bankResponse.Select(i => string.Format("{0}:{1}", i.Key, i.Value)));
                    _logger.Warn(string.Format("OrderManager > HandlePaymentResult - for UserID:{0},{1}", userID, logString), "Ödeme Sonucu");

                    var order = _orderService.GetUserOrder(userID, Common.GetOrderID(uniqueOrderID));
                    if (order == null)
                    {
                        _logger.Critical("OrderManager > HandlePaymentResult", "Order with ID:{0} not found for user:{1} to complete payment!", false, uniqueOrderID, userID);
                        return Result.Error(string.Format(Resource.Order_CompletePaymentFailedSinceOrderNotFound, uniqueOrderID));
                    }

                    var result = _paymentService.Get3DResult(bankResponse, uniqueOrderID);
                    if (result.OK && result.ResultCode > 0)
                    {
                        var success = _orderService.ChangeOrderStatus(result.ResultCode, OrderStatuses.WaitingApproval);
                        if (!success.OK)
                        {
                            _logger.Critical("OrderManager > HandlePaymentResult", "Payment Done But Status Change Failed for Order:{0} & User:{1}", false, result.ResultCode, userID);
                            result = Result.Error(result.ResultCode, Resource.Order_CompletePaymentFailed, result.Value);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("OrderManager > HandlePaymentResult", ex);
            }
            return Result.Error(Resource.Order_CannotPaidWithCreditCard);
        }

        public InstallmentViewModel GetInstallments(decimal total, int mode)
        {
            return new InstallmentViewModel()
            {
                Installments = _paymentService.GetCachedInstallments(),
                PosList = _application.Order.PosList,
                Total = total,
                DrawMode = mode
            };
        }

        public BankInfo GetBank(int id)
        {
            return _application.Order.GetBank(id);
        }
    }
}