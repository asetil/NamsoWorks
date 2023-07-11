using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Util.Enums;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser, new[] { "Brand/Index" })]
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult Index(int page = 1)
        {
            if (!IsSuper(false))
            {
                var model = _brandService.GetBrands(page);
                return View("BrandList", model);
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetBrands(int page = 1)
        {
            var model = _brandService.GetBrands(page, 25);
            return Json(new { model }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Save(Brand model)
        {
            var old = model != null && model.ID > 0 ? _brandService.Get(model.ID) : null;
            var result = _brandService.Save(model);
            if (result.OK && old != null)
            {
                _brandService.RefreshProductBrand(string.Empty, old.Name);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Delete(int brandID)
        {
            var old = _brandService.Get(brandID);
            var result = _brandService.Delete(brandID);
            if (result.OK && old != null)
            {
                _brandService.RefreshProductBrand(string.Empty, old.Name);
            }
            return ResultValue(result);
        }
    }
}