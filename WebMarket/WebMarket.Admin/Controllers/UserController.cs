using System.Web.Mvc;
using System.Web.Security;
using Aware;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Crm;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;
using Aware.Util.Lookup;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.None)]
    public class UserController : BaseController
    {
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;
        private readonly ILookupManager _lookupManager;
        private readonly ICustomerService _customerService;

        public UserController(ISessionManager sessionManager, IUserService userService, ILookupManager lookupManager, ICustomerService customerService)
        {
            _sessionManager = sessionManager;
            _userService = userService;
            _lookupManager = lookupManager;
            _customerService = customerService;
        }

        public ActionResult Index(string returnUrl)
        {
            if (IsLoggedIn)
            {
                returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return Redirect(returnUrl);
            }
            return View();
        }

        [HttpPost]
        public JsonResult Login(User model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid)
            {
                result = _sessionManager.Authorize(model);
                if (result.OK)
                {
                    model = result.ValueAs<User>();
                    var cookikeValue = string.Format("{0};;;;{1};;;;{2};;;;{3}", model.ID, model.Name, (int)model.Status, model.CustomerID);
                    FormsAuthentication.SetAuthCookie(cookikeValue, false);
                }
            }
            return ResultValue(result);
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.Admin)]
        public ActionResult UserList(int customerID = 0)
        {
            customerID = CurrentUser.IsSuper ? customerID : CurrentUser.CustomerID;
            var managerList = _userService.GetCustomerUsers(customerID);
            var model = new ManagerListModel
            {
                ManagerList = managerList,
                IsSuper = IsSuper(false),
                CustomerID = customerID,
                TitleList = _lookupManager.GetLookups(LookupType.UserTitles)
            };
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult UserDetail(int userID, int customerID = 0)
        {
            customerID = CurrentUser.IsSuper ? customerID : CurrentUser.CustomerID;
            if (!IsValidOperation(customerID))
            {
                return RedirectToRoute(RouteNames.UnauthorizedRoute);
            }

            var model = GetAdminUser(userID, customerID);
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult MyInfo()
        {
            var model = GetAdminUser(CurrentUserID, CurrentUser.CustomerID);
            return View("UserDetail", model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public ActionResult UserDetail(User model)
        {
            if (!IsValidOperation(model.CustomerID))
            {
                return RedirectToRoute(RouteNames.UnauthorizedRoute);
            }

            var result = Result.Error();
            if (ModelState.IsValid)
            {
                var isNew = model.ID == 0;
                if (!CurrentUser.IsSuper && CurrentUser.CustomerID > 0)
                {
                    model.Role = UserRole.AdminUser;
                    model.CustomerID = CurrentUser.CustomerID;
                }

                result = _userService.SaveUser(model);
                if (result.OK && isNew)
                {
                    return RedirectToRoute(Helper.RouteNames.UserDetail, new { userID = result.ValueAs<User>().ID });
                }
            }

            ViewBag.SaveResult = result;
            return UserDetail(model.ID);
        }

        public ActionResult ChangePassword(string data)
        {
            if (!IsLoggedIn && string.IsNullOrEmpty(data))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrEmpty(data))
            {
                var result = _userService.CheckActivationData(data);
                if (result.OK)
                {
                    ViewBag.RequestData = data;
                }
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword(string currentPassword, string password, string data = "")
        {
            var userID = string.IsNullOrEmpty(data) ? CurrentUserID : 0;
            var result = _userService.ChangePassword(userID, currentPassword, password);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult ForgotPassword(string email)
        {
            var result = _userService.SendAuthenticationMail(email, AuthenticationMailType.ForgotPasswordMail);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult SendActivation(string email)
        {
            var result = _userService.SendAuthenticationMail(email, AuthenticationMailType.ActivationMail);
            return ResultValue(result);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize(AuthorizeLevel.Admin)]
        public JsonResult DeleteUser(int userID)
        {
            var result = _userService.DeleteUser(userID, CurrentUserID);
            return ResultValue(result);
        }

        public ActionResult Activation(string data)
        {
            if (IsLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = _userService.ActivateUser(data);
            return View(result);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult Logout(int? newUserID)
        {
            var role = CurrentUser != null ? CurrentUser.Role : UserRole.AdminUser;
            _sessionManager.Logout();
            FormsAuthentication.SignOut();

            if (newUserID.HasValue && role == UserRole.SuperUser)
            {
                _sessionManager.LoginAs(newUserID.Value);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NoAccess()
        {
            return View();
        }

        private UserDetailModel GetAdminUser(int userID, int customerID = 0)
        {
            var user = new User { CustomerID = customerID, Role = UserRole.AdminUser };
            if (userID > 0)
            {
                user = _userService.GetAdminUser(userID);
                if (user != null && !CurrentUser.IsSuper && user.CustomerID != CurrentUser.CustomerID)
                {
                    user = null;
                }
            }

            if (user != null)
            {
                var result = new UserDetailModel
                {
                    User = user,
                    Customer = customerID > 0 ? _customerService.Get(customerID) : null,
                    AllowPasswordChange = user.ID == CurrentUserID,
                    IsSuper = CurrentUser.IsSuper,
                    UserRoleList = _lookupManager.GetLookups(LookupType.UserRoles),
                    TitleList = _lookupManager.GetLookups(LookupType.UserTitles),
                    StatusList = _lookupManager.GetLookups(LookupType.Status)
                };
                return result;
            }
            return null;
        }
    }
}
