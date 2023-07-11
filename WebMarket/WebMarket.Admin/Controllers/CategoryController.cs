using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using WebMarket.Admin.Helper;
using Aware.Util.Enums;

namespace WebMarket.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var model = _categoryService.GetCategories();
            foreach (var category in model)
            {
                var subList = model.Where(i => i.ParentID == category.ID).ToList();
                category.SetSubCategories(subList);
            }
            model = model.Where(i => i.Level == 1).ToList();

            IsSuper();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetCategoryDetail(int categoryID)
        {
            var html = string.Empty;
            var model = _categoryService.GetCategoryViewModel(categoryID,GetViewMode());
            if (model != null)
            {
                html = this.RenderPartialView("_CategoryView", model);
            }
            return Json(new { html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Save(int id, int parentID, string name, int status)
        {
            var success = 0;
            Category category = null;

            if (IsSuper(false) && ModelState.IsValid)
            {
                var model = new Category()
                {
                    ID = id,
                    ParentID = parentID,
                    Name = name,
                    Status = (Statuses)status
                };

                category = _categoryService.Save(model);
                success = category != null && category.ID > 0 ? 1 : 0;
            }
            return Json(new { success, category }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Delete(int categoryID)
        {
            var result = _categoryService.Delete(categoryID);
            return Json(new { success = result.IsSuccess, message = result.Message }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult RefreshHierarchy(int categoryID, int direction)
        {
            var result = _categoryService.RefreshHierarchy(categoryID, direction);
            return Json(new { success = result ? 1 : 0 }, JsonRequestBehavior.DenyGet);
        }
    }
}
