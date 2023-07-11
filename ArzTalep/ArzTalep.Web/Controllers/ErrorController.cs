using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArzTalep.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int code)
        {
            if(code == (int)HttpStatusCode.NotFound)
            {
                return View("NotFound");
            }
            return View();
        }
    }
}