using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Worchart.API
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class NmsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BL.Log.ILogger _logger;

        public NmsMiddleware(RequestDelegate next, BL.Log.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var sw = new Stopwatch();
            sw.Start();
            _next(httpContext);
            sw.Stop();

            //httpContext.Response.Headers.Add("nms_version", "v1.2.0");
            _logger.Warn("NmsMiddleware|ResponseDuration", "Url:{0}\nDuration:{1}", false, httpContext.Request.Path, sw.ElapsedMilliseconds);
            return Task.CompletedTask;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class NmsMiddlewareExtensions
    {
        public static IApplicationBuilder UseNmsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NmsMiddleware>();
        }
    }
}
