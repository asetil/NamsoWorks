using ArzTalep.Web.Helper;
using Microsoft.AspNetCore.Mvc;

namespace ArzTalep.Web.ViewComponents
{
    public class UserPanel : ViewComponent
    {
        private readonly ISessionHelper _sessionHelper;

        public UserPanel(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        public IViewComponentResult Invoke()
        {
            var user = _sessionHelper.GetCurrentUser();
            return View("UserPanel", user);
        }
    }
}
