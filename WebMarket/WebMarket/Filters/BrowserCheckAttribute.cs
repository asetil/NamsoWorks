using System.Web.Mvc;

namespace WebMarket.Filters
{
    public class BrowserCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null && !filterContext.IsChildAction)
            {
                var request = filterContext.RequestContext.HttpContext.Request;
                var aa = request.Browser;
            }
        }
    }
}
