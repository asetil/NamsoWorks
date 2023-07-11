using System.Web.Mvc;
using System.Linq;
using Aware;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Util;
using WebMarket.Admin.Models.ModelBinder;
using Aware.ECommerce.Manager;
using Aware.ECommerce.Search;
using Aware.Util.Model;

namespace WebMarket.Admin.Controllers
{
    public class StoreItemController : BaseController
    {
        private readonly IUploadItemManager _uploadManager;
        private readonly IStoreItemService _itemService;

        public StoreItemController(IStoreItemService itemService,IUploadItemManager uploadManager)
        {
            _itemService = itemService;
            _uploadManager = uploadManager;
        }

        public ActionResult Index([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams)
        {
            var result = _itemService.GetItems(searchParams, CurrentUser.CustomerID);
            return View("Index", result);
        }

        public ActionResult AllItems([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams)
        {
            return Index(searchParams);
        }

        public ActionResult Detail(int id, int storeID = 0, Result result = null)
        {
            var model = _itemService.GetDetailModel(CurrentUser.CustomerID, id);
            if (model != null)
            {
                if (id <= 0 && storeID > 0)
                {
                    var store = model.StoreList.FirstOrDefault(i => i.ID == storeID);
                    model.StoreItem.Store = store;
                    model.StoreItem.StoreID = storeID;
                }
                model.SaveResult = result;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Detail(StoreItem model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid)
            {
                var isNew = model.ID == 0;
                result = _itemService.Save(model);
                if (result.OK && isNew)
                {
                    var item = result.ValueAs<StoreItem>();
                    model = _itemService.Get(CurrentUser.CustomerID, item.ID);
                    return RedirectToRoute(Helper.RouteNames.StoreItemDetailRoute,
                        new { storename = model.Store.DisplayName.ToSeoUrl(), productname = model.Product.Name.ToSeoUrl(), id = model.ID });
                }
            }
            return Detail(model.ID, model.StoreID, result);
        }

        [HttpPost]
        public JsonResult QuickUpdate(int itemID, decimal price, decimal listPrice, decimal stock)
        {
            var result = _itemService.QuickUpdateItem(itemID, price, listPrice, stock);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult ImportItems(int storeID)
        {
            var result = _uploadManager.UploadStoreItems(HttpContext.Request, storeID);
            return ResultValue(result);
        }
    }
}