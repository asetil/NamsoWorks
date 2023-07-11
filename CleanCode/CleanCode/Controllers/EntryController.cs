using System.Web.Mvc;
using Aware.ECommerce.Enums;
using Aware.Util.Model;
using Aware.Util;
using CleanCode.Helper;
using CleanFramework.Business.Model;
using CleanFramework.Business.Service;
using Aware.Util.View;

namespace CleanCode.Controllers
{
    public class EntryController : AwareController
    {
        private readonly IEntryService _entryService;

        public EntryController(IEntryService entryService)
        {
            _entryService = entryService;
        }

        public ActionResult Detail(int id)
        {
            var model = _entryService.GetEntryDisplayModel(id);
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult ManageList(EntrySearchParams searchParams = null, int page = 1)
        {
            searchParams = searchParams ?? new EntrySearchParams();
            searchParams.SetPaging(page, 25);
            if (CurrentUser.Role != UserRole.SuperUser)
            {
                searchParams.UserID = CurrentUserID;
            }

            var model = _entryService.GetEntryList(searchParams);
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult ManageDetail(int id)
        {
            var model = _entryService.GetEntryDetail(id,CurrentUserID);
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public ActionResult ManageDetail(Entry entry)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                var isNew = entry.ID == 0;
                result = _entryService.Save(CurrentUserID, entry);
                if (result.OK && isNew)
                {
                    entry = result.ValueAs<Entry>();
                    return RedirectToRoute(RouteNames.EntryManageDetail, new { name = entry.Name.ToSeoUrl(), id = entry.ID });
                }
            }

            ViewBag.OperationResult = result;
            var model = _entryService.GetEntryDetail(entry.ID,CurrentUserID);
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public JsonResult Delete(int entryID)
        {
            var result = _entryService.Delete(CurrentUserID, entryID);
            return Json(new { success = result.IsSuccess, message = result.Message }, JsonRequestBehavior.DenyGet);
        }
    }
}
