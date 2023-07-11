using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Aware.Dependency;
using Aware.Util.Log;

namespace Aware.Util.Filter
{
    public class ExceptionLogFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var httpContext = context.HttpContext.ApplicationInstance.Context;
            LogException(context.Exception, httpContext, null);
        }

        public static void LogException(Exception exception, HttpContext httpContext, IController errorController)
        {
            var httpException = exception as HttpException;
            var logger = WindsorBootstrapper.Resolve<ILogger>();

            if (httpException!=null && httpContext.Response.StatusCode != 401)
            {
                var logException = true;
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        logException = false;
                        break;
                    case 401:
                        logException = false;
                        break;
                }

                if (logException)
                {
                    logger.Error("URL:{0}, REFERRER:{1},User Agent : {5}{2}Stack Trace: {3}{2}InnerException: {4}{2}",
                        exception, httpContext.Request.Url,
                        httpContext.Request.UrlReferrer, Environment.NewLine, exception.StackTrace,
                        exception.InnerException, httpContext.Request.UserAgent);
                }
                ResponseError(httpContext, exception, errorController); 
            }
            else
            {
                logger.Error("Stack Trace: {0}{1}InnerException: {2}", exception, Environment.NewLine, exception.StackTrace, exception.InnerException);
            }
        }

        private static void ResponseError(HttpContext httpContext, Exception exception, IController errorController)
        {
            if (errorController != null)
            {
                string action = "Index";
                if (exception is HttpException)
                {
                    var httpException = exception as HttpException;
                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            action = "NotFound";
                            break;
                        case 401:
                            action = "AccessDenied";
                            break;
                    }
                }

                var routeData = new RouteData();
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = action;

                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = exception is HttpException ? ((HttpException)exception).GetHttpCode() : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;

                ((IController)errorController).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }
    }
}