using System;
using System.Linq;
using System.Web.Mvc;
using Aware.Crm;
using Aware.Crm.Model;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Regional;
using Aware.Util;
using Aware.Util.Model;
using WebMarket.Admin.Models;
using Aware.Util.Enums;
using Aware.Util.Lookup;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IAddressService _addressService;
        private readonly ILookupManager _lookupManager;
        private readonly ICustomerService _customerService;

        public StoreController(IStoreService storeService, IAddressService addressService, ILookupManager lookupManager, ICustomerService customerService)
        {
            _storeService = storeService;
            _addressService = addressService;
            _lookupManager = lookupManager;
            _customerService = customerService;
        }

        public ActionResult Index(int page = 1, int customerID = 0)
        {
            page = Math.Max(page, 1);
            int totalSize;

            customerID = CurrentUser.IsSuper ? customerID : CurrentUser.CustomerID;
            var stores = _storeService.GetCustomerStores(customerID, page, out totalSize);
            var model = new StoreListModel
            {
                StoreList = stores.OrderByDescending(i => i.ID).ToList(),
                CustomerID = customerID,
                Page = page,
                Size = Constants.STORE_PAGE_SIZE,
                TotalSize = totalSize
            };
            return View(model);
        }

        public ActionResult Detail(int id,int customerID=0)
        {
            customerID = CurrentUser.IsSuper ? customerID : CurrentUser.CustomerID;
            if (!IsValidOperation(customerID))
            {
                return RedirectToRoute(RouteNames.UnauthorizedRoute);
            }

            var model = new StoreViewModel
            {
                Customer = customerID > 0 ? _customerService.Get(customerID) : new Customer(),
                Store = id > 0 ? _storeService.GetCustomerStore(customerID, id) : new Store()
            };
            return StoreDetail(model);
        }

        [HttpPost]
        public ActionResult Detail(StoreViewModel model)
        {
            var result = Result.Error();
            if (ModelState.IsValid && model.Store != null)
            {
                model.Store.RegionInfo = model.RegionInfo.ToSelected();
                var isNew = model.Store.ID == 0;

                result = _storeService.Save(model.Store);
                if (result.OK)
                {
                    model.Store = result.ValueAs<Store>();
                    var imgSaveResult = SaveMyFiles(model.Store.ID, (int)RelationTypes.Store);
                    if (imgSaveResult.OK)
                    {
                        model.Store.ImageInfo = imgSaveResult.Value.ToString();
                    }
                    _storeService.RefreshCache(CurrentUser.CustomerID, model.Store);

                    if (isNew)
                    {
                        return RedirectToRoute(Helper.RouteNames.StoreDetail, new { storename = model.Store.DisplayName.ToSeoUrl(), id = model.Store.ID });
                    }
                }
            }

            ViewBag.SaveResult = result;
            return StoreDetail(model);
        }

        [HttpPost]
        public JsonResult SaveStoreWorkTimeInfo(int storeID, string workTimeInfo)
        {
            var result = _storeService.SaveWorkTimeInfo(CurrentUser.CustomerID, storeID, workTimeInfo);
            return Json(new { success = result ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult StoreDetail(StoreViewModel model)
        {
            var districtList=_addressService.SearchRegions(string.Empty,null, RegionType.District);
            model.RegionList = districtList.Select(i => new Item(i.ID, i.Name)).ToList();
            model.AllowRegionSelection = true;
            model.StatusList = _lookupManager.GetLookups(LookupType.Status);
            return View(model);
        }
    }
}