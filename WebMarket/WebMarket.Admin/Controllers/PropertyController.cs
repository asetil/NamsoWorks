using System.Web.Mvc;
using System.Linq;
using Aware;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.Search;
using WebMarket.Admin.Models;
using Aware.Util;
using Aware.Util.Model;
using WebMarket.Admin.Helper;
using WebMarket.Admin.Models.ModelBinder;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Controllers
{
    public class PropertyController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IPropertyService _propertyService;
        private readonly ILookupManager _lookupManager;
        public PropertyController(IPropertyService propertyService, IProductService productService, ILookupManager lookupManager)
        {
            _propertyService = propertyService;
            _productService = productService;
            _lookupManager = lookupManager;
        }

        public ActionResult Index()
        {
            IsSuper();
            ViewBag.PropertyTypes = _lookupManager.GetLookups(LookupType.PropertyTypes);
            var model = _propertyService.GetProperties();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            if (!IsSuper(false) && id == 0)
            {
                return RedirectToAction("Index");
            }

            return Detail(id, null);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult Detail(PropertyValue model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid)
            {
                var isNew = model != null && model.ID == 0;
                result = _propertyService.SaveProperty(model);
                if (result.OK && isNew)
                {
                    var id = (int)result.Value;
                    return RedirectToRoute(RouteNames.PropertyDetailRoute, new { name = model.Name.ToSeoUrl(), id });
                }
            }
            return Detail(model.ID, result);
        }

        public ActionResult CommentManagement([ModelBinder(typeof(CommentSearchBinder))] CommentSearchParams searchParams)
        {
            searchParams.WithCount();
            var result = _propertyService.GetComments(searchParams);
            var productIDs = result.Results.Select(i => i.RelationID).Distinct();

            var productSearchParams = new ProductSearchParams { IDs = productIDs};
            var productSearchResult = _productService.SearchProducts(productSearchParams) ?? new SearchResult<Product>();

            var model = new CommentListModel
            {
                SearchResult = result,
                ItemList = productSearchResult.Results.Select(p => new Item(p.ID, p.Name) { Url = p.DefaultImage.Path }),
                RaitingStarList = _lookupManager.GetLookups(LookupType.RaitingStars),
                CommentStatusList = _lookupManager.GetLookups(LookupType.CommentStatus),
                AllowEdit = IsSuper(false)
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult DrawProductRelations(int productID)
        {
            var model = _propertyService.GetProductRelationProperties(productID);
            if (model != null)
            {
                model.ViewMode = GetViewMode();
                var html = this.RenderPartialView("_ProductRelationsView", model);
                var success = !string.IsNullOrEmpty(html);
                return Json(new { success, html }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { success = 0 }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SavePropertyOption(int id, int parentID, string name, string sortOrder, bool isVariant)
        {
            var model = new PropertyValue()
            {
                ID = id,
                ParentID = parentID,
                Name = name,
                SortOrder = sortOrder
            };

            var result = _propertyService.SavePropertyOption(model, isVariant);
            var itemID = result.OK ? result.ValueAs<PropertyValue>().ID : model.ID;
            return Json(new { success = result.IsSuccess, message = result.Message, itemID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteProperty(int id)
        {
            var result = _propertyService.DeleteProperty(id, false);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult SavePropertyRelation(int productID, int propertyID, string value, string sortOrder)
        {
            var result = _propertyService.SavePropertyRelation(propertyID, value, sortOrder, productID, (int)RelationTypes.Product);
            return Json(new { success = result.IsSuccess, message = result.Message, relationID = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteProductRelation(int productID, int propertyID)
        {
            var result = _propertyService.DeletePropertyRelation(propertyID, productID, (int)RelationTypes.Product);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult ChangeCommentStatus(int commentID, CommentStatus status)
        {
            var result = _propertyService.ChangeCommentStatus(commentID, status, CurrentUserID);
            return ResultValue(result);
        }

        private ActionResult Detail(int id, Result result)
        {
            var model = _propertyService.GetPropertyDetail(id);
            model.AllowEdit = IsSuper(false);
            model.Result = result;
            return View(model);
        }
    }
}