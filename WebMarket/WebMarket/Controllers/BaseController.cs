using System.Web.Mvc;
using Aware;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Dependency;
using Aware.Util.Model;
using Aware.Util.Filter;

namespace WebMarket.Controllers
{
    [Localization]
    public class BaseController : Controller
    {
        private CustomPrincipal _currentUser;
        protected CustomPrincipal CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = HttpContext.User as CustomPrincipal;
                    if (_currentUser == null)
                    {
                        var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
                        sessionManager.Authenticate(ref _currentUser);

                        if (_currentUser != null)
                        {
                            _currentUser.Identity = HttpContext.User.Identity;
                            HttpContext.User = _currentUser;
                        }
                    }
                }
                return _currentUser;
            }
        }

        private int _currentUserID = -1;
        protected int CurrentUserID
        {
            get
            {
                if (_currentUserID == -1)
                {
                    _currentUserID = (CurrentUser != null ? CurrentUser.ID : 0);
                }
                return _currentUserID == -1 ? 0 : _currentUserID;
            }
        }

        public JsonResult Json(int success, string message)
        {
            return Json(new { success, message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResultValue(Result result, JsonRequestBehavior behaviour = JsonRequestBehavior.DenyGet, bool includeCode = false)
        {
            result = result ?? Result.Error(Resource.General_Error);
            if (includeCode)
            {
                return Json(new { success = result.IsSuccess, message = result.Message, resultCode = result.ResultCode }, behaviour);
            }
            return Json(new { success = result.IsSuccess, message = result.Message }, behaviour);
        }

        public int UserRegionID
        {
            get
            {
                var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
                var activeRegion = sessionManager.GetCurrentRegion();
                return activeRegion;
            }
        }

        public string IPAddress
        {
            get
            {
                var ipAddress = Request.ServerVariables["http_client_ip"] ?? Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) { ipAddress = Request.ServerVariables["REMOTE_ADDR"]; }
                return ipAddress ?? "UNKNOWN";
            }
        }
    }
}