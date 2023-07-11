using System.Web.Mvc;
using Aware;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace WebMarket.Admin.Controllers
{
    public class VariantController : BaseController
    {
        private readonly IVariantService _variantService;
        public VariantController(IVariantService variantService)
        {
            _variantService = variantService;
        }

        public ActionResult Index()
        {
            IsSuper();
            var model = _variantService.GetVariantListModel();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var isSuper = IsSuper(false);
            if (!isSuper && id == 0)
            {
                return RedirectToAction("Index");
            }

            var model = _variantService.GetVariantDetail(id);
            if (model != null)
            {
                model.AllowEdit = isSuper;
                model.SaveResult = (Result)TempData["SaveResult"];
            }
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult Detail(VariantProperty model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid)
            {
                var isNew = model != null && model.ID == 0;
                result = _variantService.SaveVariantProperty(model);
                if (result.OK && isNew)
                {
                    var id = (int)result.Value;
                    return RedirectToRoute(Helper.RouteNames.VariantDetailRoute, new { name = model.Name.ToSeoUrl(), id });
                }
            }

            TempData["SaveResult"] = result;
            return Detail(model.ID);
        }

        [HttpPost]
        public JsonResult GetVariantRelations(int relationID, int relationType)
        {
            var model = _variantService.GetVariantRelations(relationID, relationType);
            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult DeleteVariantProperty(int id)
        {
            var result = _variantService.DeleteVariantProperty(id);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult SaveVariantRelation(VariantRelation relation)
        {
            relation.Status = Statuses.Active;
            var result = _variantService.SaveVariantRelation(relation);
            return Json(new { success = result.IsSuccess, id = (int)result.Value });
        }

        [HttpPost]
        public JsonResult DeleteVariantRelation(int id)
        {
            var result = _variantService.DeleteVariantRelation(id);
            return Json(new { success = result.IsSuccess });
        }

        [HttpPost]
        public JsonResult SaveVariantSelection(VariantSelection selection)
        {
            var result = _variantService.SaveVariantSelection(selection);
            return Json(new { success = result.IsSuccess, id = (int)result.Value });
        }

        [HttpPost]
        public JsonResult DeleteVariantSelection(int selectionID)
        {
            var result = _variantService.DeleteVariantSelection(selectionID);
            return Json(new { success = result.IsSuccess });
        }
    }
}