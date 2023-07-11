using System.Web.Mvc;
using Aware.ECommerce.Interface;

namespace WebMarket.Controllers
{
    public class CampaignController : BaseController
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public ActionResult Index()
        {
            var model = _campaignService.GetActiveCampaigns();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = _campaignService.GetActiveCampaign(id);
            return View(model);
        }
    }
}
