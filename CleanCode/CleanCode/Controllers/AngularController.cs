using System.Web.Mvc;
using CleanCode.Helper;

namespace CleanCode.Controllers
{
    public class AngularController : Controller
    {
        public ActionResult Index(string name)
        {
            if (!string.IsNullOrEmpty(name) && this.ViewExists(name.Replace("-", "")))
            {
                return View(name.Replace("-", ""));
            }
            return View();
        }
    }
}