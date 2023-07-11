using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMarket.Controllers;
using Aware.Dependency;
using Aware.Util.Filter;
using WebMarket.Infrastructure;
using Aware.Authenticate;

namespace WebMarket
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            Bootstrapper.Initialise();

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            //ModelBinders.Binders.Add(typeof(DateTime), new ModelBinderForDateTime());
            //ModelBinders.Binders.Add(typeof(DateTime?), new ModelBinderForDateTime());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Authorization,DNT,X-Mx-ReqToken,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type");
        }
    
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            ExceptionLogFilter.LogException(exception, HttpContext.Current, new ErrorController());
        }

        protected void Application_End()
        {
            Bootstrapper.Stop();
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            var pars = arg.Split(';');
            if (pars.Length == 0) return string.Empty;

            var res = new System.Text.StringBuilder();
            foreach (var s in pars)
            {
                switch (s)
                {
                    case "$Region":
                        var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
                        var regionInfo = string.Format("region_{0}", sessionManager.GetCurrentRegion());
                        res.Append(regionInfo);
                        break;
                    default:
                        var par = context.Request[s];
                        if (par != null)
                            res.AppendFormat(par);
                        break;
                }
            }
            return res.ToString();
        }
    }
}