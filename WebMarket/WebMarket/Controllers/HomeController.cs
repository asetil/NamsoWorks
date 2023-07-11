using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Manager;
using Aware.ECommerce.Model;
using Aware.Regional;
using Aware.Mail;
using WebMarket.Models;
using WebMarket.Filters;
using Aware.Authenticate;
using Aware.Util.Enums;
using Aware.Util.Slider;
using Aware.Dependency;
using Aware.Util;

namespace WebMarket.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductSearchManager _productSearchManager;
        private readonly ICategoryService _categoryService;
        private readonly IAddressService _addressService;
        private readonly ICommonService _commonService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMailService _mailService;
        private readonly IApplication _application;
        private readonly ISessionManager _sessionManager;
        private readonly ISliderManager _sliderManager;

        public HomeController(ICategoryService categoryService, ICommonService commonService, IAddressService addressService, IFavoriteService favoriteService, IMailService mailService, IApplication application,
            IProductSearchManager productSearchManager, ISessionManager sessionManager, ISliderManager sliderManager)
        {
            _categoryService = categoryService;
            _commonService = commonService;
            _addressService = addressService;
            _favoriteService = favoriteService;
            _mailService = mailService;
            _application = application;
            _productSearchManager = productSearchManager;
            _sessionManager = sessionManager;
            _sliderManager = sliderManager;
        }

        [RegionSelection]
        public ActionResult Index()
        {
            var sliderItems = _sliderManager.GetCachedSliderItems(SliderType.Main);
            var model = Helper.Util.GetMainSlider(sliderItems);
            return View(model);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Contact()
        {
            var model = new ContactModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            var result = _commonService.CheckCaptcha(Request.Form["g-recaptcha-response"], IPAddress);
            if (result.OK)
            {
                model.UserID = CurrentUserID;
                result = _mailService.SendContactUsMail(model);
            }

            model = new ContactModel()
            {
                Result = result
            };
            return View(model);
        }

        public ActionResult Sss()
        {
            return View();
        }

        public ActionResult osman()
        {
            var url = "https://apis.garanti.com.tr/auth/oauth/v2/token";
            url += "?client_id=l7xxbef3f96a3f60401099bef251b89e9844&client_secret=95e5cd1b699b4b0d8ce6ec993ae2c9de&";
            url += "grant_type=client_credentials&redirect_uri=http://site.aware.com/home/demo";

            var result = WebRequester.makePostRequest(url,"");
            return Content(result);
        }

        [HttpPost]
        public ActionResult demo(string code, string error)
        {
            return Content(code + "-" + error);
        }

        public ActionResult MembershipAggreement()
        {
            return View();
        }

        public ActionResult SiteHeader()
        {
            var model = new SiteHeaderModel
            {
                IsLoggedIn = CurrentUserID > 0,
                UserInfo = CurrentUserID > 0 ? CurrentUser.Name : string.Empty,
                TopMenuItems = _categoryService.GetMainCategories(),
                FavoriteProducts = _favoriteService.GetUserFavorites(CurrentUserID),
                CurrentRegion = _addressService.GetRegion(UserRegionID),
                AllowProductCompare = _application.Site.AllowProductCompare,
                HasNotification = _sessionManager.HasNewNotifiction()
            };
            return PartialView("_SiteHeader", model);
        }

#if !DEBUG
        [OutputCache(Duration = Constants.DailyCacheDuration, VaryByCustom = "$Region")]
#endif
        public ActionResult MainCampaigns()
        {
            var model = new List<MultiSliderModel>();
            var oppotunityItems = _productSearchManager.GetOpportunityItems(UserRegionID);
            var newItems = oppotunityItems;
            var discountedItems = oppotunityItems;

            var categoryItems = _productSearchManager.GetHomeCategoryItems(UserRegionID);
            if (categoryItems != null && categoryItems.Any())
            {
                var categories = _categoryService.GetCategories(level: 1);
                foreach (var category in categories)
                {
                    var categoryIDs = _categoryService.GetRelatedCategoryIDs(new List<int>() { category.ID });
                    var subItems = categoryItems.Where(i => categoryIDs.Contains(i.CategoryID)).ToList();
                    if (subItems.Count() >= 4)
                    {
                        model.Add(new MultiSliderModel("CategoryItems_" + category.ID, string.Empty, 0)
                        {
                            Products = subItems.ToList(),
                            LazyLoad = true,
                            Title = string.Format("{0} Ürünleri", category.Name),
                            ItemCount = 4
                        });
                    }
                }
            }
            return PartialView("_MainCampaigns", model);
        }
    }
}