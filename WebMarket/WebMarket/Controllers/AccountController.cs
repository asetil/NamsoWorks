using System.Web.Mvc;
using System.Web.Security;
using Aware;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.ECommerce.Interface;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using System;

namespace WebMarket.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        public AccountController(ISessionManager sessionManager, IUserService userService, ICommonService commonService)
        {
            _sessionManager = sessionManager;
            _userService = userService;
            _commonService = commonService;
        }

        public ActionResult Index(string returnUrl)
        {
            if (CurrentUserID > 0)
            {
                returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return Redirect(returnUrl);
            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Login(User model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Email))
            {
                result = _sessionManager.Authorize(model);
                if (result.OK)
                {
                    model = result.ValueAs<User>();
                    FormsAuthentication.SetAuthCookie(model.ID + ";;;;" + model.Name, false);
                }
            }
            return ResultValue(result);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            _sessionManager.Logout();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (CurrentUserID > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new Models.UserViewModel
            {
                User = new User(),
                PermissionList = _commonService.GetCachedSimpleItems(ItemType.UserPermissions)
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                result = _userService.SaveUser(model);
                if (result.OK)
                {
                    return RedirectToAction("Activation", new { id = 1 });
                }
            }

            var resultModel = new Models.UserViewModel
            {
                User = model,
                PermissionList = _commonService.GetCachedSimpleItems(ItemType.UserPermissions),
                Result = result
            };
            return View(resultModel);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult Detail()
        {
            var model = new Models.UserViewModel
            {
                User = _userService.GetUser(CurrentUserID),
                PermissionList = _commonService.GetCachedSimpleItems(ItemType.UserPermissions),
            };
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Detail(User model)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                model.ID = CurrentUserID;
                result = _userService.SaveUser(model);
            }

            var resultModel = new Models.UserViewModel
            {
                User = model,
                PermissionList = _commonService.GetCachedSimpleItems(ItemType.UserPermissions),
                Result = result
            };
            return View(resultModel);
        }

        public ActionResult ChangePassword(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var result = _userService.CheckActivationData(data);
                if (result.OK)
                {
                    ViewBag.RequestData = data;
                }
                return View();
            }
            else if (CurrentUserID > 0)
            {
                var user = _userService.GetUser(CurrentUserID);
                if (string.IsNullOrEmpty(user.Password)) //Sosyal ağlardan bağlanan kullanıcılar şifre tanımlasınlar diye
                {
                    var activationData = string.Format("{0}#{1}#{2}", user.ID, user.Email, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    ViewBag.RequestData = Encryptor.Encrypt(activationData);
                }
                return View();
            }
            return RedirectToRoute(Helper.RouteNames.LoginRoute, new { returnUrl = "sifre-degistir" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword(string currentPassword, string password, string data = "")
        {
            var userID = string.IsNullOrEmpty(data) ? CurrentUserID : 0;
            var result = _userService.ChangePassword(userID, currentPassword, password, data);
            return ResultValue(result);
        }

        public ActionResult Activation(string data, int id = 0)
        {
            if (CurrentUserID > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id > 0)
            {
                return View(Result.Error(id, string.Empty));
            }

            var result = _userService.ActivateUser(data);
            return View(result);
        }

        [HttpPost]
        public JsonResult SendActivation(string email)
        {
            var result = _userService.SendAuthenticationMail(email, AuthenticationMailType.ActivationMail);
            return Json(result.ResultCode, result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult ForgotPassword(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var result = _userService.SendAuthenticationMail(username, AuthenticationMailType.ForgotPasswordMail);
                return Json(result.IsSuccess, result.Message);
            }
            return Json(0, Resource.User_EmailIsNotValid);
        }
    }
}
