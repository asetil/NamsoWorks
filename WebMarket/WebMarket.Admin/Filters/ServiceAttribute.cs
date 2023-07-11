using System.Web.Mvc;

namespace WebMarket.Admin.Filters
{
    public class ServiceAttribute : ActionFilterAttribute
    {
        private const string TICKET = "ticket";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey(TICKET))
            {
                var ticket = filterContext.ActionParameters[TICKET] as string;
            }
        } 
    }
}