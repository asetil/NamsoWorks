using System.Linq;
using System.Web.Mvc;
using Aware.Cache;
using Aware.Util.Enums;
using CleanFramework.Business.Service;
using CleanFramework.Business.Model;
using Aware.Util.View;

namespace CleanCode.Controllers
{
    public class HomeController : AwareController
    {
        private readonly IEntryService _entryService;
        private readonly ICacher _cacher;

        public HomeController(IEntryService entryService, ICacher cacher)
        {
            _entryService = entryService;
            _cacher = cacher;
        }

        public ActionResult Index(int categoryID = 0, string q = "", string tag = "", int page = 1)
        {
            TempData["ActiveMenu"] = categoryID;
            var searchParams = new EntrySearchParams(categoryID, q, tag) { Status = Statuses.Active };
            searchParams.SetPaging(page, 8).WithCount().SortBy(i => i.DateCreated, true);

            var model = _entryService.Search(searchParams, true, true);
            return View(model);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult CacheManagement()
        {
            var model = _cacher.GetAllKeys().Where(i => !i.StartsWith("MetadataPrototypes"));
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public JsonResult ClearCache(string cacheKey)
        {
            var isSuccess = _cacher.Remove(cacheKey) ? 1 : 0;
            return Json(new { isSuccess }, JsonRequestBehavior.DenyGet);
        }
    }
}