using System.Web.Mvc;
using Aware.Notification;
using WebMarket.Admin.Helper;
using Aware.Util.Enums;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            var model = _notificationService.GetAllNotifications();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetNotification(int notificationID)
        {
            var html = string.Empty;
            var model = _notificationService.GetNotification(notificationID);
            if (model != null)
            {
                html = this.RenderPartialView("_NotificationView", model);
            }
            return Json(new { success = !string.IsNullOrEmpty(html), html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveNotification(Notification model)
        {
            var result = _notificationService.Save(model);
            return ResultValue(result);
        }
    }
}