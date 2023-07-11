using System.Linq;
using System.Web.Mvc;
using Aware;
using Aware.Authenticate;
using Aware.Cache;
using Aware.ECommerce.Util;
using Aware.Dependency;
using Aware.ECommerce;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace WebMarket.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICacher _cacher;
        public ServiceController(IUserService userService, ICacher cacher)
        {
            _userService = userService;
            _cacher = cacher;
        }

        [HttpPost]
        public JsonResult GetTicket(string email, string password)
        {
            var result = _userService.GetAuthorizeTicket(email, password, 3);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetCacheKeys(string ticket)
        {
            var result = _userService.IsTicketAuthorized(ticket);
            if (result.OK)
            {
                var keyList = _cacher.GetAllKeys().Where(i => !i.StartsWith("MetadataPrototypes"));
                result = Result.Success(keyList);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult ClearCache(string ticket, string cacheKey)
        {
            var result = _userService.IsTicketAuthorized(ticket);
            if (result.OK)
            {
                var success = true;
                var application = WindsorBootstrapper.Resolve<IApplication>();

                if (cacheKey == Constants.CK_SiteModel)
                {
                    application.ClearCache(ItemType.SiteSettings);
                }
                else if (cacheKey == Constants.CK_OrderSettings)
                {
                    application.ClearCache(ItemType.OrderSettings);
                }
                else
                {
                    success = _cacher.Remove(cacheKey);
                }
                result = success ? Result.Success() : Result.Error(Resource.General_Error);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}