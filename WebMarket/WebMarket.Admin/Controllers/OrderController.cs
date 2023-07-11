using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using WebMarket.Admin.Models.ModelBinder;
using System.Collections.Generic;
using Aware.ECommerce.Search;
using Aware.Payment.Model;
using Aware.Regional;
using Aware.Regional.Model;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IAddressService _addressService;
        public OrderController(IOrderService orderService, IAddressService addressService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _addressService = addressService;
            _paymentService = paymentService;
        }

        public ActionResult Index([ModelBinder(typeof(OrderSearchBinder))] OrderSearchParams searchParams, int customerID = 0)
        {
            customerID = CurrentUser.IsSuper ? customerID : CurrentUser.CustomerID;
            if (!IsValidOperation(customerID) || (CurrentUser.IsSuper && customerID <= 0))
            {
                return RedirectToRoute(RouteNames.NotFoundRoute);
            }

            searchParams.WithCount();
            var model = _orderService.SearchOrders(searchParams, customerID);
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = _orderService.GetOrderViewModel(id, -1, 0, true);
            return View(model);
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult ShippingMethods()
        {
            ViewBag.RegionList = GetRegions();
            var model = _orderService.GetAllShippingMethods();
            return View(model);
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult ShippingMethod(int id)
        {
            ViewBag.RegionList = GetRegions().Select(i => new Item(i.ID, i.Name)).ToList();
            var model = id > 0 ? _orderService.GetShippingMethod(id) : new ShippingMethod();
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult ShippingMethod(ShippingMethod model)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                var isNew = model.ID == 0;
                result = _orderService.SaveShippingMethod(model);
                if (result.OK)
                {
                    model = result.ValueAs<ShippingMethod>();
                    if (isNew)
                    {
                        return RedirectToRoute(Helper.RouteNames.ProductDetailRoute, new { id = model.ID });
                    }
                }
            }

            ViewBag.SaveResult = result;
            ViewBag.RegionList = GetRegions().Select(i => new Item(i.ID, i.Name)).ToList();
            return View(model);
        }

        [HttpPost]
        public JsonResult ChangeStatus(int id, OrderStatuses status)
        {
            var result = _orderService.ChangeOrderStatus(id, status);
            return Json(new { success = result.IsSuccess, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        private List<Region> GetRegions()
        {
            var list = new List<Region>() { new Region() { ID = -1, Name = "Tümü" } };
            list.AddRange(_addressService.SearchRegions(string.Empty,null));
            return list;
        }

        #region Bank Management

        public ActionResult BankInfo()
        {
            if (!IsSuper(false))
            {
                var model = _paymentService.GetBankList();
                return View("BankList", model);
            }
            return View();
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult GetBankList()
        {
            var model = _paymentService.GetBankList();
            return Json(new { model }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult SaveBankInfo(BankInfo model)
        {
            var result = _paymentService.SaveBankInfo(model);
            return Json(new { success = result.IsSuccess, message = result.Message, itemID = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult DeleteBankInfo(int bankID)
        {
            var result = _paymentService.DeleteBankInfo(bankID);
            return ResultValue(result);
        }

        #endregion

        #region InstallmentInfo Management

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult InstallmentInfo()
        {
            return View();
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult GetInstallmentList()
        {
            var posList = new List<Item>();
            var posDefinitions = _paymentService.GetPosDefinitions();

            if (posDefinitions != null && posDefinitions.Any())
            {
                var model = _paymentService.GetInstallmentList();
                posList = posDefinitions.OrderBy(i => !i.IsTest).Select(i => new Item(i.ID, i.Name)).ToList();
                return Json(new { model, posList }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { posList }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult SaveInstallmentInfo(InstallmentInfo model)
        {
            var result = _paymentService.SaveInstallmentInfo(model);
            return Json(new { success = result.IsSuccess, message = result.Message, itemID = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult DeleteInstallmentInfo(int installmentID)
        {
            var result = _paymentService.DeleteInstallmentInfo(installmentID);
            return ResultValue(result);
        }

        #endregion
    }
}
