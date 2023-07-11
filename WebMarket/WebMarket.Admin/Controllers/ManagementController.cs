using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aware.Language;
using Aware.Language.Model;
using Aware.Util.Enums;
using Aware.Util.Model;
using Aware.Util.Slider;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
    public class ManagementController : BaseController
    {
        private readonly ILanguageService _languageService;
        private readonly ISliderManager _sliderManager;

        public ManagementController(ILanguageService languageService, ISliderManager sliderManager)
        {
            _languageService = languageService;
            _sliderManager = sliderManager;
        }

        #region  SliderManagement

        public ActionResult SliderItems(SliderType type = SliderType.Main)
        {
            var model = _sliderManager.GetSliderManagementModel(type);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetSlider(int id, SliderType type)
        {
            var model = id > 0 ? _sliderManager.GetSliderItem(id, type) : new SliderItem { Type = type };
            if (model != null)
            {
                var html = this.RenderPartialView("_SliderPreview", model);
                return Json(new { success = !string.IsNullOrEmpty(html), html }, JsonRequestBehavior.DenyGet);
            }
            return ResultValue(Result.Error());
        }

        [HttpPost]
        public JsonResult SaveSlider(SliderItem model)
        {
            var result = _sliderManager.Save(model);
            if (result.OK)
            {
                SaveMyFiles((int)result.Value,(int)RelationTypes.Slider);
            }
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult RemoveSlider(int sliderID)
        {
            var result = _sliderManager.Delete(sliderID);
            return ResultValue(result);
        }

        #endregion

        #region LanguageManagement

        public ActionResult Language()
        {
            return View();
        }

        public ActionResult LanguageValue(int relationID, int relationType)
        {
            var model = _languageService.GetDisplayModel(relationID, relationType);
            return PartialView(model);
        }


        [HttpPost]
        public JsonResult GetLanguageList()
        {
            var model = _languageService.GetLanguages();
            return Json(new { model }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveLanguage(Language model)
        {
            var result = _languageService.Save(model);
            return Json(new { success = result.IsSuccess, message = result.Message, langID = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveLanguageValue(int relationID, int relationType, List<LanguageValue> languageValue)
        {
            var result = _languageService.SaveValues(relationID, relationType, languageValue);
            return Json(new { success = result.IsSuccess, message = result.Message, langID = result.Value }, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult SetAsDefaultLanguage(int languageID)
        {
            var result = _languageService.SetAsDefault(languageID);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult DeleteLanguage(int languageID)
        {
            var result = _languageService.DeleteLanguage(languageID);
            return ResultValue(result);
        }

        #endregion
    }
}
