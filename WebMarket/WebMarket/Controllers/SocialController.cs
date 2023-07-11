using System.Collections.Specialized;
using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Util;

namespace WebMarket.Controllers
{
    public class SocialController : BaseController
    {
        private readonly ISocialAuthManager _socialAuthManager;
        private const string FB_ReturnUrl = "/Social/FacebookLogin";
        private const string GP_ReturnUrl = "/Social/ProcessGoogleLogin";

        public SocialController(ISocialAuthManager socialAuthManager)
        {
            _socialAuthManager = socialAuthManager;
        }

        public ActionResult FacebookLogin(string code, string error)
        {
            var success = false;
            if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(code))
            {
                var result = _socialAuthManager.ProcessFacebookLogin(code, FB_ReturnUrl);
                success = result.OK;
            }

            if (!success)
            {
                return RedirectToAction("Message", "Error", new { source = "facebook_login" });
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ProcessGoogleLogin(string code, string error)
        {
            var success = false;
            if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(code))
            {
                var result = _socialAuthManager.ProcessGoogleLogin(code, GP_ReturnUrl);
                success = result.OK;
            }

            if (!success)
            {
                return RedirectToAction("Message", "Error", new { source = "google_login" });
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult TestFacebookLogin()
        {
            return View();
        }

        public ActionResult TestGoogleLogin()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult GetFacebookLoginUrl()
        {
            var result = _socialAuthManager.GetFacebookLoginUrl(FB_ReturnUrl);
            return Json(new { success = 1, url = result }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult GetGoogleLoginUrl()
        {
            var result = _socialAuthManager.GetGoogleLoginUrl(GP_ReturnUrl);
            return Json(new { success = 1, url = result }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult HandleSocialClient(FacebookProfileModel model)
        {
            var result = _socialAuthManager.HandleSocialClient(model);
            return Json(new { success = result.IsSuccess }, JsonRequestBehavior.DenyGet);
        }
    }
}