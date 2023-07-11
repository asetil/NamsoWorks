using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Notification;
using Aware.Mail;
using Aware.Authenticate;
using Aware.Language;

namespace WebMarket.Controllers
{
    public class HelperController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IMailService _mailService;
        private readonly ISessionManager _sessionManager;
        private readonly ILanguageService _languageService;

        public HelperController(INotificationService notificationService, IMailService mailService,ISessionManager sessionManager, ILanguageService languageService)
        {
            _notificationService = notificationService;
            _mailService = mailService;
            _languageService = languageService;
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public JsonResult CheckNotification()
        {
            var notification = _notificationService.GetUserNotification(CurrentUserID);
            var success = notification != null ? 1 : 0;
            return Json(new { success, notification }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult SelectRegion()
        {
            return View();
        }

        public ActionResult DynamicPage(int pageID)
        {
            var model = new Tuple<int, string, string>(pageID, "Deneme", "Deneme İçerik");
            return View(model);
        }

        public JsonResult MailTest()
        {
            _mailService.SendWelcomeMail("osman.sokuoglu@gmail.com", "Osman Sokuoğlu", "#");
            return Json("Gönderildi", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LanguageList()
        {
            var languageList = _languageService.GetCachedLanguages();
            if (languageList != null && languageList.Any())
            {
                var defaultLanguage = _sessionManager.GetCurrentLanguage();
                languageList = languageList.Where(i => i.Abbreviate != defaultLanguage).ToList();
            }
            return PartialView(languageList);
        }

        public ActionResult ChangeLanguage(string lang)
        {
            _sessionManager.SetCurrentLanguage(lang);
            var returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "/";
            return Redirect(returnUrl);
        }

        [HttpPost]
        public JsonResult RefreshRegion(int regionID)
        {
            var success = _sessionManager.SetCurrentRegion(regionID) ? 1 : 0;
            return Json(new { success }, JsonRequestBehavior.DenyGet);
        }
    }
}
