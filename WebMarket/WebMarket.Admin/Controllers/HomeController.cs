using System.Linq;
using System.Web.Mvc;
using Aware.Authenticate.Model;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Util.Lookup;
using WebMarket.Admin.Helper;
using WebMarket.Admin.Models;

namespace WebMarket.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IOrderService _orderService;
        private readonly ILookupManager _lookupManager;

        public HomeController(IStoreService storeService, IOrderService orderService, ILookupManager lookupManager)
        {
            _storeService = storeService;
            _orderService = orderService;
            _lookupManager = lookupManager;
        }

        public ActionResult Index()
        {
            if (CurrentUser.IsSuper)
            {
                return RedirectToRoute(RouteNames.CustomerSearchRoute);
            }

            var model = new DashboardStatisticModel();
            var storeList = _storeService.GetCustomerStores(CurrentUser.CustomerID);
            var searchResult = _orderService.SearchOrders(null, CurrentUser.CustomerID);

            if (searchResult!=null && searchResult.Success && storeList != null)
            {
                var storeStatistics = _storeService.GetStoreStatistics(CurrentUser.CustomerID);
                var stores = storeList.Select(i =>
                {
                    i.Statistic = storeStatistics.FirstOrDefault(s => s.StoreID == i.ID) ?? new StoreStatisticModel();
                    return i;
                }).ToList();

                model.Orders = searchResult.Results;
                model.Stores = stores;
            }

            model.CalculateStatistics();
            return View(model);
        }
        
        public ActionResult Header()
        {
            var model = CurrentUser ?? new CustomPrincipal();
            return PartialView("_Header", model);
        }

        public ActionResult SideBar()
        {
            var user = CurrentUser ?? new CustomPrincipal();
            var model = new SideBarModel
            {
                User = user,
                //Stores = null//_storeService.GetStoreStatistics(AdminUserID),
                RoleList = _lookupManager.GetLookups(LookupType.UserRoles)
            };
            return PartialView("_SideBar", model);
        }
    }
}
