using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using Aware.Dependency;
using Aware.Authenticate;

namespace Aware.Util.Filter
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
            var language = sessionManager.GetCurrentLanguage();

            if (Thread.CurrentThread.CurrentCulture.Name != language)
            {
                CultureInfo culture = new CultureInfo(language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
    }
}