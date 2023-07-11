using System.Web.Mvc;
using Aware.Util.Model;

namespace WebMarket.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Maintenance()
        {
            return View();
        }

        public ActionResult BrowserNotSupported()
        {
            return View();
        }

        public ActionResult Message(string source)
        {
            var model = Result.Error(source);   
            return View(model);
        }
    }
}
