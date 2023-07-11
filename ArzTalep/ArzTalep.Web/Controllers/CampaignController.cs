using Microsoft.AspNetCore.Mvc;

namespace ArzTalep.Web.Controllers
{
    public class CampaignController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}