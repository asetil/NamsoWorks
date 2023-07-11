using Aware.ECommerce.Interface;
using System.Web.Mvc;
using Aware.File;
using Aware.Util.Enums;
using CleanCode.Helper;

namespace CleanCode.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        public CategoryController(ICategoryService categoryService, IFileService fileService)
        {
            _categoryService = categoryService;
            _fileService = fileService;
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult Index()
        {
            var model = _categoryService.GetCategories();
            return View(model);
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        [HttpPost]
        public JsonResult GetCategory(int categoryID)
        {
            var model = _categoryService.GetCategoryViewModel(categoryID, ViewTypes.Editable);
            if (model != null)
            {
                var html = this.RenderPartialView("_Detail", model);
                return Json(new {success= !string.IsNullOrEmpty(html) ? 1 : 0,html}, JsonRequestBehavior.DenyGet);
            }
            return Json(new { success = 0 }, JsonRequestBehavior.DenyGet);
        }

        [ChildActionOnly]
#if !DEBUG
        [OutputCache(Duration = 3600)]
#endif
        public ActionResult TopMenu()
        {
            var categoryList = _categoryService.GetMainCategories();
            return PartialView("_TopMenu",categoryList);
        }
    }
}
