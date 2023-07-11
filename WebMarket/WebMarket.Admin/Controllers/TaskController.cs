using System.Web.Mvc;
using Aware.Task;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
    public class TaskController : BaseController
    {
        private readonly ITaskManager _taskManager;
        public TaskController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public ActionResult Index()
        {
            return View(_taskManager);
        }

        public ActionResult Detail(int id)
        {
            var model = _taskManager.GetTask(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult Refresh()
        {
            _taskManager.Refresh();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Stop()
        {
            _taskManager.Stop();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Start()
        {
            _taskManager.Start();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Execute(TaskType type, string executionParam)
        {
            _taskManager.RunImmediately(type, executionParam);
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
    }
}