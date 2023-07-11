using System.Web.Mvc;
namespace WebMarket.Admin.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
    }
}
