using System.Web.Mvc;
using WebMarket.Helper;
using Aware.Authenticate;
using Aware.Dependency;

namespace WebMarket.Filters
{
    public class RegionSelectionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
            var activeRegion = sessionManager.GetCurrentRegion();

            if (activeRegion == 0) //Semt seçili değilse semt seçimine yönlendir
            {
                filterContext.Result = new RedirectToRouteResult(RouteNames.SelectRegionRoute,null);
            }
        }
    }
}
