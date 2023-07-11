using ArzTalep.BL.Manager;
using Microsoft.AspNetCore.Mvc;

namespace ArzTalep.Web.ViewComponents
{
    public class MainCampaigns : ViewComponent
    {
        private readonly ICampaignManager _campaignManager;

        public MainCampaigns(ICampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public IViewComponentResult Invoke()
        {
            var model = _campaignManager.SearchBy(i => i.ID > 0);
            return View("MainCampaigns", model);
        }
    }
}
