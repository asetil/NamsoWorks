using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Aware.ECommerce.Interface;
using Aware.Util;
using WebMarket.Helper;
using Aware.ECommerce.Model;
using Aware.ECommerce.Manager;
using Aware.Mail;
using Aware.Payment.Model;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace WebMarket.Controllers
{
    [Aware.Util.Filter.Authorize("Order/GetAllInstallments")]
    public class OrderController : BaseController
    {
        private readonly IOrderManager _orderManager;
        private readonly IOrderService _orderService;
        private readonly IMailService _mailService;

        public OrderController(IOrderManager orderManager, IOrderService orderService, IMailService mailService)
        {
            _orderManager = orderManager;
            _orderService = orderService;
            _mailService = mailService;
        }

        public ActionResult Index()
        {
            var result = _orderService.GetRawOrder(CurrentUserID);
            if (!result.OK)
            {
                TempData["CheckResult"] = result.Message;
                return RedirectToAction("Index", "Basket");
            }

            var model = result.ValueAs<OrderViewModel>();
            return View(model);
        }

        public ActionResult Detail(string id)
        {
            var orderID = Aware.Util.Common.GetOrderID(id);
            if (orderID <= 0) { return RedirectToRoute(RouteNames.NotFound); }

            var model = _orderService.GetOrderViewModel(orderID, CurrentUserID);
            return View(model);
        }

        public ActionResult Approval()
        {
            var model = _orderService.GetOrderViewModel(0, CurrentUserID, OrderStatuses.WaitingCustomerApproval, true);
            if (model == null || !model.IsValid())
            {
                return RedirectToAction("Index", "Basket");
            }
            return View(model);
        }

        public ActionResult MyOrders()
        {
            var model = _orderService.GetUserOrders(CurrentUserID);
            return View(model);
        }

        public ActionResult Payment(string id)
        {
            var orderID = Aware.Util.Common.GetOrderID(id);
            if (orderID <= 0)
            {
                return RedirectToRoute(RouteNames.NotFound);
            }
            return PaymentWithResult(orderID);
        }

        public ActionResult OnlinePaymentResult(string uniqueOrderID, int success = 0)
        {
            var result = Result.Success(null, string.Empty, -1);
            var orderID = Aware.Util.Common.GetOrderID(uniqueOrderID);
            if (orderID > 0 && success != 2)
            {
                var bankResponse = Request.Form.Keys.Cast<string>().ToDictionary(key => key, key => Request.Form.Get(key));
                if (bankResponse.ContainsKey("PosID"))
                {
                    result = _orderManager.HandlePaymentResult(CurrentUserID, uniqueOrderID, bankResponse);
                    return PaymentWithResult(orderID, result);
                }
                return PaymentWithResult(orderID);
            }
            return View("OnlinePaymentResult", result);
        }

        public ActionResult PaymentWithResult(int orderID, Result paymentResult = null)
        {
            var model = _orderService.GetOrderViewModel(orderID, CurrentUserID);
            if (model == null || !model.IsValid())
            {
                return RedirectToAction("Index", "Basket");
            }

            if (!model.IsValid(true))
            {
                return RedirectToAction("MyOrders", "Order");
            }

            ViewBag.PaymentResult = paymentResult;
            return View("Payment", model);
        }

        [HttpPost]
        public JsonResult EditOrder(Order order)
        {
            var result = _orderService.SaveOrder(order, CurrentUserID);
            var idinfo = result.OK ? result.ValueAs<Order>().UniqueID : string.Empty;

            return Json(new { success = result.IsSuccess, message = result.Message, idinfo }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Approve(int orderID)
        {
            var result = _orderService.Approve(CurrentUserID, orderID);
            if (result.OK)
            {
                SendOrderMail(orderID, Resources.Resource.Order_ApprovedMessage);
            }
            return Json(new { success = result.IsSuccess, message = result.Message, idinfo = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SavePaymentInfo(int orderID, PaymentType paymentType, int subPaymentType, int installment = 0)
        {
            bool savedBefore;
            var result = _orderService.SavePaymentInfo(CurrentUserID, orderID, paymentType, subPaymentType, installment, out savedBefore);
            if (result.OK)
            {
                var order = (Order)result.Value;
                var bank = paymentType == PaymentType.Remittance ? _orderManager.GetBank(subPaymentType) : new BankInfo();

                if (!savedBefore)
                {
                    SendOrderMail(orderID, Resources.Resource.Order_ApprovedMessage);
                }
                return Json(new { success = result.IsSuccess, message = result.Message, order, bank }, JsonRequestBehavior.DenyGet);
            }
            return ResultValue(result);
        }

        [HttpPost]
        public ActionResult ProcessPayment(int orderID, int posID, CreditCard card, int installment = 0)
        {
            var result = _orderManager.ProcessPayment(CurrentUserID, orderID, card, posID, installment, IPAddress);
            return Json(new { success = result.IsSuccess, message = result.Message, value = result.Value, code = result.ResultCode }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult CancelOrder(int id)
        {
            var result = _orderService.CancelOrder(CurrentUserID, id);
            if (result.OK) SendOrderMail(id, "Siparişiniz İptal Edildi.");
            return Json(result.IsSuccess, result.Message);
        }

        [HttpPost]
        public JsonResult GetShippingMethods(int regionID)
        {
            var success = 0;
            object data = null;

            var result = _orderService.GetRegionShippingMethods(CurrentUserID, new List<int> { regionID });
            if (result != null && result.Any())
            {
                data = result.Select(i => new { id = i.ID, name = i.Description, region = i.RegionInfo, price = i.Price.DecToStr() }).ToList();
                success = 1;
            }
            return Json(new { success, data }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult GetCardInfo(string binNumber, int posID, decimal orderTotal)
        {
            var cardInfo = _orderManager.GetCardInfo(binNumber, posID, orderTotal);
            var installmentsModel = new InstallmentViewModel()
            {
                Installments = cardInfo.Installments,
                Total = cardInfo.OrderTotal
            };

            var html = this.RenderPartialView("_InstallmentView", installmentsModel);
            var success = !string.IsNullOrEmpty(html);
            return Json(new { success, card = cardInfo, html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult GetAllInstallments(int productID, decimal total, int mode = 1)
        {
            var model = _orderManager.GetInstallments(total, mode);
            var html = this.RenderPartialView("_InstallmentView", model);
            var success = !string.IsNullOrEmpty(html);
            return Json(new { success, html }, JsonRequestBehavior.DenyGet);
        }

        private void SendOrderMail(int orderID, string subject)
        {
            var orderModel = _orderService.GetOrderViewModel(orderID, CurrentUserID, 0, true);
            var html = this.RenderPartialView("_OrderForMail", orderModel);
            if (!string.IsNullOrEmpty(html))
            {
                _mailService.SendOrderMail(orderModel.User.Email, subject, html);
            }
        }
    }
}