using Microsoft.AspNetCore.Mvc;

namespace ArzTalep.Web.Controllers
{
    public class ProductController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}