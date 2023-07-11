using System.Web.Mvc;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using WebMarket.Filters;

namespace WebMarket.Controllers
{
    [RegionSelection]
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public ActionResult Index()
        {
            var model = _storeService.GetRegionStores();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = _storeService.GetRegionStore(id,true);
            if (model == null || !model.HasRegion(UserRegionID) || model.ServiceRegions == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}