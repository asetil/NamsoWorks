using System.Collections.Generic;
using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.Util.Filter;
using WebMarket.Helper;

namespace WebMarket.Controllers
{
    [RequireSSL]
    [Aware.Util.Filter.Authorize]
    public class BasketController : BaseController
    {
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public ActionResult Index()
        {
            var model = _basketService.GetBasketWithDiscounts(CurrentUserID, 0);
            return View(model);
        }

        public ActionResult CompletePurchase()
        {
            var result = _basketService.CheckBasketBeforePurchase(CurrentUserID);
            if (result.OK)
            {
                return RedirectToAction("Index", "Order");
            }

            TempData["CheckResult"] = result.Message;
            return RedirectToAction("Index", "Basket");
        }

        [HttpPost]
        public JsonResult GetBasketSummary()
        {
            var basket = _basketService.GetBasketSummary(CurrentUserID, 0);
            var itemCount = basket.Items.Count;
            var html = this.RenderPartialView("_BasketSummaryView", new Models.BasketSummaryModel
            {
                Basket = basket,
                DrawMode = Models.BasketDrawMode.ForBasketSummary
            });
            return Json(new { success = 1, html, itemCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToBasket(int storeItemID, decimal quantity, string variantSelection = "")
        {
            var result = _basketService.AddItemToBasket(CurrentUserID, storeItemID, quantity, variantSelection);
            return ResultValue(result, JsonRequestBehavior.DenyGet, true);
        }

        [HttpPost]
        public JsonResult ChangeBasketItemQuantity(int basketID, int basketItemID, decimal quantity)
        {
            var html = string.Empty;
            var result = _basketService.ChangeBasketItemQuantity(CurrentUserID, basketID, basketItemID, quantity);
            if (result.OK)
            {
                var model = _basketService.GetBasketSummary(CurrentUserID, 0);
                html = this.RenderPartialView("_BasketView", model);
            }
            return Json(new { success = result.IsSuccess, message = result.Message, html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteBasketItem(int basketID, int basketItemID)
        {
            var html = string.Empty;
            var result = _basketService.DeleteBasketItem(CurrentUserID, basketID, basketItemID);
            if (result.OK)
            {
                var model = _basketService.GetBasketWithDiscounts(CurrentUserID, 0);
                if (model.Items.Count > 0)
                {
                    html = this.RenderPartialView("_BasketView", model);
                }
            }
            return Json(new { success = result.IsSuccess, message = result.Message, html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteBasketSummaryItem(int basketID, int basketItemID)
        {
            var result = _basketService.DeleteBasketItem(CurrentUserID, basketID, basketItemID);
            if (result.OK)
            {
                return GetBasketSummary();
            }
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult UseCoupon(int basketID, string code)
        {
            var result = _basketService.UseCoupon(CurrentUserID, basketID, code);
            return Json(new { success = result.IsSuccess, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddFavoritesToBasket(int storeID, string productIDs)
        {
            var result = _basketService.AddFavoritesToBasket(CurrentUserID, storeID, productIDs);
            var fails = string.Empty;
            if (result.Value != null)
            {
                fails = string.Join("<br>", result.ValueAs<List<string>>());
                fails = string.Format("<br><div style='font-size:12px;color:#F3E30B;'>{0}</div>", fails);
            }
            return Json(new { success = result.IsSuccess, message = result.Message, fails }, JsonRequestBehavior.AllowGet);
        }
    }
}