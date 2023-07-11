using System.Linq;
using System.Web.Mvc;
using Aware;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Regional;
using Aware.Regional.Model;
using Aware.Util;
using Aware.Util.Model;
using WebMarket.Helper;
using Resource = Resources.Resource;
using Aware.Util.Enums;

namespace WebMarket.Controllers
{
    public class AddressController : BaseController
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult MyAddresses(byte drawMode = 0)
        {
            ViewBag.DrawMode = drawMode;
            var model = _addressService.GetUserAddresses(CurrentUserID, true);
            return PartialView("_AddressSelection", model);
        }

        [HttpPost]
        public JsonResult LoadDistricts(string keyword)
        {
            var data = _addressService.SearchRegions(string.Empty,null, RegionType.District)
                                        .Where(i => i.Name.Contain(keyword))
                                        .Select(i => new { id = i.ID, value = i.Name });
            return Json(new { success = 1, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDistricts()
        {
            var districts = _addressService.SearchRegions(string.Empty, null, RegionType.District);
            return Json(new { districts }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public JsonResult DeleteAddress(int id)
        {
            var result = _addressService.DeleteAddress(id, CurrentUserID);
            return ResultValue(result);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public JsonResult EditAddress(Address model)
        {
            var html = string.Empty;
            var result = Result.Error(Resource.Address_RegionCannotBeRefreshed);

            if (ModelState.IsValid)
            {
                var isNew = model != null && model.ID == 0;
                result = _addressService.SaveAddress(model, CurrentUserID);
                if (result.OK)
                {
                    var address = result.ValueAs<Address>();
                    _addressService.LoadAddressRecipe(ref address);
                    html = isNew ? this.RenderPartialView("_AddressListItem", address) : address.DisplayText;
                }
            }
            return Json(new { success = result.IsSuccess, message = result.Message, html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public JsonResult LoadAddress(int id)
        {
            var html = string.Empty;
            var result = _addressService.GetUserAddress(id, CurrentUserID);
            if (result != null)
            {
                html = this.RenderPartialView("_AddressEditView", result);
            }
            return Json(new { success = !string.IsNullOrEmpty(html), html, addressID = id }, JsonRequestBehavior.AllowGet);
        }
    }
}