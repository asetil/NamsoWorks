using Aware.Regional;
using Aware.Regional.Model;
using Aware.Util.Enums;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMarket.Admin.Models;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(Aware.Util.Enums.AuthorizeLevel.SuperUser)]
    public class RegionController : BaseController
    {
        private readonly IAddressService _addressService;

        public RegionController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public ActionResult Index()
        {
            var cityList = _addressService.SearchRegions(string.Empty,null, RegionType.City);
            var model = new RegionDisplayModel
            {
                RegionList = cityList,
                Type = RegionType.City,
                ParentID = 0
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult GetSubRegions(int parentID, RegionType regionType)
        {
            var regionList = _addressService.SearchRegions(string.Empty,parentID, regionType);
            var model = new RegionDisplayModel
            {
                RegionList = regionList,
                Type = regionType,
                ParentID = parentID
            };

            var html = this.RenderPartialView("_RegionList", model);
            return Json(new { html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SearchRegions(string keyword)
        {
            var result = _addressService.SearchRegions(keyword, 20);
            if (result != null)
            {
                var data = result.Select(i => new { id = i.ID, value = i.Name });
                return Json(new { success = 1, data }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 0, data = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRegion(int id, int parentID, string name, RegionType regionType)
        {
            var result = _addressService.SaveRegion(new Region
            {
                ID = id,
                ParentID = parentID,
                Name = name,
                Type = regionType
            });
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult DeleteRegion(int regionID)
        {
            var result = _addressService.DeleteRegion(regionID);
            return ResultValue(result);
        }
    }
}