using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Worchart.BL;
using Worchart.BL.Constants;
using Worchart.BL.Model;
using Worchart.BL.Token;

namespace Worchart.API.Middleware
{
    public class TokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (IsPathAllowed(httpContext.Request.Path.Value))
            {
                return _next(httpContext);
            }

            var errorCode = string.Empty;
            var hasApiToken = httpContext.Request.Headers.ContainsKey(CommonConstants.AccessToken) || httpContext.Request.Headers.ContainsKey(CommonConstants.AccessToken.ToLowerInvariant());
            var services = httpContext.RequestServices;

            if (!hasApiToken)
            {
                errorCode = ErrorConstants.Token.NoAccessToken;
            }
            else
            {
                var accessToken = httpContext.Request.Headers[CommonConstants.AccessToken].ToString();
                if (!accessToken.Valid())
                {
                    errorCode = ErrorConstants.Token.InvalidAccessToken;
                }
                else
                {
                    var tokenManager = (ITokenManager)services.GetService(typeof(ITokenManager));
                    var valid = tokenManager.CheckToken(new TokenRequest()
                    {
                        AuthorizeToken = accessToken,
                        Type = TokenType.AccessToken
                    });

                    if (!valid)
                    {
                        errorCode = ErrorConstants.Token.InvalidAccessToken;
                    }
                }
            }

            var configuration = (IConfiguration)services.GetService(typeof(IConfiguration));

            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { configuration.GetValue("CorsOrigin") });
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept, AccessToken, CustomerToken, get" });
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
            httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });


            if (!string.IsNullOrEmpty(errorCode))
            {
                var operationResult = new OperationResult(errorCode);
                httpContext.Response.StatusCode = httpContext.Request.Method == "OPTIONS" ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Unauthorized;
                httpContext.Response.ContentType = "application/json";
                string jsonString = JsonConvert.SerializeObject(operationResult);
                httpContext.Response.WriteAsync(jsonString, Encoding.UTF8);
               
                return Task.CompletedTask;
            }

            return _next(httpContext);
        }

        private bool IsPathAllowed(string path)
        {
            var allowedPaths = new List<string>() { "/api/token/authorize", "/api/token/accesstoken", "/api/token/refresh", "/api/token/simulate", "/api/values" };
            return allowedPaths.Contains(path.ToLowerInvariant());
        }
    }
}
