using ArzTalep.Web.Helper;
using ArzTalep.Web.Models;
using Aware.Manager;
using Aware.Model;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Aware.BL.Model;

namespace ArzTalep.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserManager _userManager;
        private readonly ISessionHelper _sessionHelper;
        private readonly IEncryptManager _encryptManager;

        public AccountController(IUserManager userManager, IEncryptManager encryptManager, ISessionHelper sessionHelper)
        {
            _userManager = userManager;
            _encryptManager = encryptManager;
            _sessionHelper = sessionHelper;
        }

        public IActionResult Index(string returnUrl)
        {
            if (_sessionHelper.CurrentUserID > 0)
            {
                returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return Redirect(returnUrl);
            }
            return View();
        }


        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Login(string userName, string password, bool remember = false)
        {
            if (ModelState.IsValid)
            {
                var result = _userManager.Login(userName, password);
                if (result.Ok && result.Value.IsValid)
                {
                    _sessionHelper.Set(WebConstants.AuthenticationSessionKey, result.Value.SessionKey);

                    if (remember)
                    {
                        var authorizatonCookie = _encryptManager.Encrypt(result.Value.SessionKey);
                        Response.Cookies.Append(WebConstants.AuthorizationCookieKey, authorizatonCookie, new CookieOptions()
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddDays(7),
                            SameSite = SameSiteMode.None,
                        });
                    }

                    //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, invite.Id.ToString()));

                    //var principal = new ClaimsPrincipal(identity);
                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                }
                return Json(result);
            }
            return Json(OperationResult<SessionDataModel>.Error(ResultCodes.Error.Login.InvalidUsernamePassword));
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            _userManager.Logoff(_sessionHelper.CurrentUserID);
            _sessionHelper.Remove(WebConstants.AuthenticationSessionKey);

            if (Request.Cookies.ContainsKey(WebConstants.AuthorizationCookieKey))
            {
                Response.Cookies.Append(WebConstants.AuthorizationCookieKey, string.Empty, new CookieOptions()
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(-1),
                    SameSite = SameSiteMode.Lax,
                });
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (_sessionHelper.CurrentUserID > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new UserViewModel()
            {
                User = new User()
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            var result = OperationResult<User>.Error(ResultCodes.Error.CheckParameters);
            if (ModelState.IsValid)
            {
                result = _userManager.Register(model);
                if (result.Ok)
                {
                    var activationResult = OperationResult<ActivationResultType>.Success(ActivationResultType.WaitingActivation);
                    return View("Activation", activationResult);
                }
            }

            var resultModel = new Models.UserViewModel
            {
                User = model,
                SaveResult = result
            };
            return View(resultModel);
        }

        [Authorize]
        public ActionResult Detail()
        {
            var model = new Models.UserViewModel
            {
                User = _userManager.Get(_sessionHelper.CurrentUserID)
            };
            return View("UserDetail", model);
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(User model)
        {
            var result = Failed<User>();
            if (ModelState.IsValid)
            {
                model.ID = _sessionHelper.CurrentUserID;
                result = _userManager.Save(model);
            }

            var resultModel = new Models.UserViewModel
            {
                User = model,
                SaveResult = result
            };
            return View("UserDetail", resultModel);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult ForgotPassword(string emailOrPhoneNumber)
        {
            var result = _userManager.SendNewActivation(emailOrPhoneNumber, true);
            return Json(result);
        }

        public ActionResult ChangePassword(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var result = _userManager.CheckActivationData(data);
                if (result.Ok)
                {
                    ViewBag.RequestData = data;
                }
                else
                {
                    ViewBag.ErrorCode = result.Code;
                }
                return View();
            }
            else if (_sessionHelper.CurrentUserID > 0)
            {
                //TODO : //Sosyal ağlardan bağlanan kullanıcılar şifre tanımlasınlar diye
                //var user = _userManager.Get(_sessionHelper.CurrentUserID);
                //if (string.IsNullOrEmpty(user.Password)) //Sosyal ağlardan bağlanan kullanıcılar şifre tanımlasınlar diye
                //{
                //    var activationData = string.Format("{0}#{1}#{2}", user.ID, user.Email, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //    ViewBag.RequestData = Encryptor.Encrypt(activationData);
                //}
                return View();
            }
            return RedirectToAction("Login", new { returnUrl = "sifre-degistir" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword(string currentPassword, string password, string data = "")
        {
            var userID = string.IsNullOrEmpty(data) ? _sessionHelper.CurrentUserID : 0;
            var result = _userManager.ChangePassword(userID, currentPassword, password, data);
            return Json(result);
        }

        public ActionResult Activation(string data)
        {
            if (_sessionHelper.CurrentUserID > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = _userManager.ActivateUser(data);
            return View(result);
        }

        [HttpPost]
        public JsonResult SendActivation(string email)
        {
            var result = _userManager.SendNewActivation(email, false);
            return Json(result);
        }
    }
}