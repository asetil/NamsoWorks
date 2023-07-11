using System;
using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Manager;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.Util.Model;
using WebMarket.Admin.Helper;
using WebMarket.Admin.Models.ModelBinder;
using Aware.File;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Lookup;
using WebMarket.Admin.Models;

namespace WebMarket.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        private readonly ICommonService _commonService;
        private readonly ILookupManager _lookupManager;
        private readonly IBrandService _brandService;

        public ProductController(IProductService productService, ICategoryService categoryService, IFileService fileService, ICommonService commonService, ILookupManager lookupManager, IBrandService brandService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
            _commonService = commonService;
            _lookupManager = lookupManager;
            _brandService = brandService;
        }

        public ActionResult Index([ModelBinder(typeof(ProductSearchBinder))] ProductSearchParams searchParams)
        {
            var model = _productService.GetProductListModel(searchParams);
            IsSuper();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var isSuper = IsSuper(false);
            if (!isSuper && id == 0) { return View("Index"); }

            var settings = _commonService.GetSiteSettings();
            var model = new ProductViewModel
            {
                Product = _productService.Get(id),
                CategoryList = _categoryService.GetCategories(),
                BrandList = _brandService.GetBrands(1, int.MaxValue),
                HasMultiLanguage = settings.UseMultiLanguage,
                StatusList = _lookupManager.GetLookups(LookupType.Status),
                MeasureUnits = _lookupManager.GetLookups(LookupType.MeasureUnits)
            };
            return View(isSuper ? "Detail" : "ProductView", model);
        }

        [HttpPost]
        public ActionResult Detail(ProductViewModel model)
        {
            var result = Result.Error();
            var brandList = _brandService.GetBrands(1, int.MaxValue);
            if (ModelState.IsValid)
            {
                var isNew = model.Product.ID == 0;
                var brand = brandList.FirstOrDefault(i => i.ID == model.Product.Brand.Int());
                if (brand != null) { model.Product.Brand = brand.Name; }

                result = _productService.SaveProduct(model.Product);
                if (result.OK)
                {
                    model.Product = result.ValueAs<Product>();
                    if (isNew)
                    {
                        return RedirectToRoute(Helper.RouteNames.ProductDetailRoute, new { name = model.Product.Name.ToSeoUrl(), id = model.Product.ID });
                    }
                }
            }

            IsSuper();
            model.CategoryList = _categoryService.GetCategories();
            model.BrandList = brandList;
            ViewBag.SaveResult = result;
            return View(model);
        }

        [HttpPost]
        public JsonResult GetUnit(int productID)
        {
            var product = _productService.Get(productID) ?? new Product();
            return Json(new { value = product.UnitDescription }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult GetProductImages(int productID)
        {
            var html = string.Empty;
            var gallery = _fileService.GetGallery(productID, (int)RelationTypes.Product, 10);
            if (gallery != null)
            {
                gallery.ViewMode = GetViewMode();
                html = this.RenderPartialView("_FileGallery", gallery);
            }
            return Json(new { success = !string.IsNullOrEmpty(html), html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchProducts(string keyword)
        {
            var searchParams = new ProductSearchParams(keyword, 0, 12);
            var result = _productService.SearchProducts(searchParams);

            if (result != null && result.Success && result.Results != null)
            {
                var data = result.Results.Select(i => new { id = i.ID, value = i.Name });
                return Json(new { success = 1, data }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 0, data = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}