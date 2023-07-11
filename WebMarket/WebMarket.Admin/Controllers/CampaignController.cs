using System.Linq;
using System.Web.Mvc;
using Aware;
using Aware.ECommerce.Interface;
using Aware.Util;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Util.Enums;
using Aware.Util.Lookup;
using Aware.Util.Model;
using WebMarket.Admin.Helper;
using WebMarket.Admin.Models;

namespace WebMarket.Admin.Controllers
{
    public class CampaignController : BaseController
    {
        private readonly ICampaignService _campaignService;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IPropertyService _propertyService;
        private readonly ILookupManager _lookupManager;

        public CampaignController(ICampaignService campaignService, IStoreService storeService, ICategoryService categoryService, IPropertyService propertyService, ILookupManager lookupManager)
        {
            _campaignService = campaignService;
            _storeService = storeService;
            _categoryService = categoryService;
            _propertyService = propertyService;
            _lookupManager = lookupManager;
        }

        public ActionResult Index()
        {
            var managerID = IsSuper(false) ? -1 : CurrentUserID;
            var model = _campaignService.GetManagerCampaigns(managerID);
            return View(model);
        }

        public ActionResult Detail(int id, int? templateID)
        {
            var model = GetViewModel(id, templateID);
            ViewBag.SaveResult = TempData.Value<Result>("SaveResult");
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Detail(CampaignViewModel model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid && model.Campaign != null && model.Campaign.IsMine(CurrentUserID))
            {
                result = _campaignService.Save(model.Campaign, CurrentUserID, Request);
                if (result.OK)
                {
                    model.Campaign =  result.ValueAs<Campaign>();
                    SaveMyFiles(model.Campaign.ID, (int)RelationTypes.Campaign);
                }
            }

            TempData["SaveResult"] = result;
            return RedirectToRoute(Helper.RouteNames.CampaignDetailRoute, new { name = model.Campaign.Name.ToSeoUrl(), id = model.Campaign.ID });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = _campaignService.DeleteCampaign(id);
            return ResultValue(result);
        }

        private CampaignViewModel GetViewModel(int id, int? templateID)
        {
            var stores = _storeService.GetCustomerStores(CurrentUser.CustomerID);
            var categories = _categoryService.GetCategories().ToList();
            var properties = _propertyService.GetProperties();

            var ownerID = CurrentUser.IsSuper ? 0 : CurrentUserID;
            var campaign = (id > 0 ? _campaignService.GetAdminCampaign(id, ownerID) : new Campaign() { OwnerID = ownerID });
            var model = new CampaignViewModel
            {
                Campaign = campaign,
                SelectedTemplate = templateID ?? -1,
                StatusList = _lookupManager.GetLookups(LookupType.Status)
            };

            if (model.SelectedTemplate > 0 && model.Campaign != null)
            {
                var selectedTemplate = model.CampaignTemplates.FirstOrDefault(i => i.ID == model.SelectedTemplate);
                model.Campaign = model.Campaign.MergeWith(selectedTemplate);
            }

            if (model.Campaign != null)
            {
                model.SetFilters(model.Campaign.FilterInfo);
            }

            model.AllowEdit = model.Campaign != null && model.Campaign.IsMine(CurrentUserID);
            model.StoreList = stores.Select(i => new Item(i.ID, i.DisplayName)
            {
                OK = model.HasFilter("sid", i.ID)
            }).ToList();

            model.CategoryList = categories.Select(i => new Item(i.ID, i.Name)
            {
                OK = model.HasFilter("cid", i.ID)
            }).ToList();

            model.PropertyList = properties.Select(i => new Item(i.ID, i.Name,string.Empty)
            {
                OK = model.HasFilter("pid", i.ID)
            }).ToList();

            return model;
        }
    }
}