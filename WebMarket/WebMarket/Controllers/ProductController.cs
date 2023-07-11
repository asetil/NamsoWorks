using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Manager;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Model;
using WebMarket.Helper;
using WebMarket.Models;
using Aware.Util.Enums;
using WebMarket.Filters;
using WebMarket.Models.ModelBinder;

namespace WebMarket.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductSearchManager _productSearchManager;
        private readonly ICategoryService _categoryService;
        private readonly IPropertyService _propertyService;
        private readonly IFavoriteService _favoriteService;
        private readonly IVariantService _variantService;
        private readonly IApplication _application;
        private readonly IStoreService _storeService;

        private readonly object _lockThis = new object();

        public ProductController(IProductSearchManager productSearchManager, IStoreService storeService, ICategoryService categoryService, IPropertyService propertyService,
            IFavoriteService favoriteService, IApplication application, IVariantService variantService)
        {
            _productSearchManager = productSearchManager;
            _storeService = storeService;
            _categoryService = categoryService;
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _application = application;
            _variantService = variantService;
        }

        [RegionSelection]
        public ActionResult Index([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams)
        {
            return Search(searchParams, true);
        }

        public ActionResult Search([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams, bool asView = false)
        {
            searchParams.SetPaging(0, 36).WithAggs().WithCount();
            var searchResult = _productSearchManager.Search(searchParams, _storeService.GetRegionStores());

            var model = new ProductSearchViewModel()
            {
                SearchResult = searchResult,
                FavoriteProducts = _favoriteService.GetUserFavorites(CurrentUserID),
                ForStoreDetail = !asView,
                FilterDirection = asView && searchResult.HasAggregation ? LayoutDirection.Vertical : LayoutDirection.Horizantal,
                AllowCompare = _application.Site.AllowProductCompare
            };

            if (asView)
            {
                return View("Index", model);
            }
            return PartialView("_ProductListView", model);
        }

        public JsonResult LoadNextPage([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams)
        {
            searchParams.WithCount();
            var searchResult = _productSearchManager.Search(searchParams, _storeService.GetRegionStores());

            if (searchResult != null && searchResult.Success)
            {
                var completed = searchResult.HasMore ? 0 : 1;
                var model = new ProductSearchViewModel()
                {
                    SearchResult = searchResult,
                    AllowCompare = _application.Site.AllowProductCompare
                };

                var html = this.RenderPartialView("_ProductListView2", model);
                return Json(new { success = 1, html, completed }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        [RegionSelection]
        public ActionResult Detail(int id, int? storeID)
        {
            var model = new ProductViewModel
            {
                Product = _productSearchManager.GetProductDetail(id, storeID),
                FavoriteProducts = _favoriteService.GetUserFavorites(CurrentUserID),
                DisplayComments = _application.Site.DisplayComments,
                AllowCompare = _application.Site.AllowProductCompare,
                AllowSoialShare = _application.Site.AllowSocialShare,
                ShowInstallments = _application.Site.ShowProductInstallments && _application.Order.PosList != null && _application.Order.PosList.Any()
            };

            if (model.Product != null && model.Product.Properties != null && model.Product.Properties.Any())
            {
                model.ArrangeProperties(_propertyService.GetAllCachedProperties());
            }
            return View("Detail", model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult MyFavorites([ModelBinder(typeof(ItemSearchBinder))] ItemSearchParams searchParams)
        {
            searchParams.OnlyFavorites = CurrentUserID > 0;
            searchParams.UserID = CurrentUserID;
            return Search(searchParams, true);
        }

        public ActionResult Comments(int id, int page = 1)
        {
            var model = _propertyService.GetProductCommentStats(id, CurrentUserID, page);
            model.IsPartial = false;
            return View("ProductComments", model);
        }

        public JsonResult SearchProducts(string keyword)
        {
            lock (_lockThis)
            {
                var searchParams = new ItemSearchParams(keyword, 0, 10) { SearchCategories = true };
                var model = _productSearchManager.Search(searchParams, _storeService.GetRegionStores());
                var html = this.RenderPartialView("_SearchResultPreview", model);
                return Json(new { html }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCategoryHierarchy(int parentID)
        {
            var hierarchy = _categoryService.GetCategoryHierarchy(parentID);
            var html = Util.DrawProductHierarchicalCategories(hierarchy);
            return Json(new { success = 1, html }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSimilarItems(int storeID, int categoryID, int productID)
        {
            var searchParams = new ItemSearchParams(string.Empty, 0, 16);
            searchParams.WithStore(storeID).WithCategory(categoryID);

            var result = _productSearchManager.Search(searchParams, _storeService.GetRegionStores());
            if (!result.Success || !result.HasResult) return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            var products = result.Results.Where(i => i.ID != productID).ToList();
            if (!products.Any()) return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            var slider = new MultiSliderModel("similarItems")
            {
                Products = products,
                ItemCount = 4,
                Title = "Benzer Ürünler"
            };

            var html = this.RenderPartialView("_MultiSlider", slider);
            return Json(new { success = 1, html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductCommentStats(int productID, string productName)
        {
            var model = _propertyService.GetProductCommentStats(productID, CurrentUserID);
            if (model != null)
            {
                model.Title = productName;
                model.IsPartial = true;

                var html = this.RenderPartialView("_ProductCommentView", model);
                var success = !string.IsNullOrEmpty(html);
                return Json(new { success, html, cc = model.CommentCount, avg = model.RatingAverage }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveComment(int productID, string title, string value, int rating)
        {
            var model = new Comment
            {
                Title = title,
                Value = value,
                Rating = rating,
                RelationType = (int)RelationTypes.Product,
                RelationID = productID,
                OwnerID = CurrentUserID,
                OwnerName = CurrentUser.Name.AsCommentOwner()
            };

            var result = _propertyService.SaveComment(model);
            return ResultValue(result);
        }

        [RegionSelection]
        public ActionResult CompareProducts()
        {
            ProductSearchViewModel model = null;
            var compareCookie = _application.WebHelper.GetCookie("compare-list");
            if (compareCookie != null && !string.IsNullOrEmpty(compareCookie.Value))
            {
                var productIDs = compareCookie.Value.Split(",").Select(i => i.Int()).ToList();
                var searchParams = new ItemSearchParams(string.Empty, 0, productIDs.Count) { IDs = productIDs };
                model = new ProductSearchViewModel()
                {
                    SearchResult = _productSearchManager.Search(searchParams, _storeService.GetRegionStores())
                };
                ViewBag.PropertyData = _propertyService.GetAllCachedProperties();
            }
            return View("CompareProducts", model);
        }

        [HttpPost]
        public JsonResult GetComparedProductsInfo(string productIDs)
        {
            if (!string.IsNullOrEmpty(productIDs))
            {
                var ids = productIDs.Trim().Split(",").Select(i => i.Int()).ToList();
                var searchParams = new ItemSearchParams(string.Empty, 0) { IDs = ids };
                var searchResult = _productSearchManager.Search(searchParams, _storeService.GetRegionStores());

                if (searchResult != null && searchResult.HasResult)
                {
                    var items = searchResult.Results.Select(i => new Item(i.ID, i.Name, string.Empty, i.DefaultImage.Path)).ToList();
                    return Json(new { success = 1, items }, JsonRequestBehavior.DenyGet);
                }
            }
            return Json(new { success = 0 }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult GetItemVariants(int itemID)
        {
            var model = _variantService.GetVariantRelations(itemID, (int)RelationTypes.StoreItem, true);
            if (model == null) return Json(new { success = false }, JsonRequestBehavior.DenyGet);

            var html = this.RenderPartialView("_ItemVariantsView", model);
            var success = !string.IsNullOrEmpty(html);
            return Json(new { success, html, data = model }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AddToFavorite(int productID)
        {
            var result = _favoriteService.AddToFavorite(CurrentUserID, productID);
            return Json(new { success = result.IsSuccess, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveFavorite(string productIDs)
        {
            var result = _favoriteService.RemoveFavorites(CurrentUserID, productIDs);
            return Json(new { success = result.IsSuccess, message = result.Message, ids = result.Value }, JsonRequestBehavior.AllowGet);
        }
    }
}