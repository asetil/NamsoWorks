using Aware.BL.Model;
using Aware.Manager;
using Aware.Util;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArzTalep.Web.Helper
{
    public class CustomAuthorizeRequirement : IAuthorizationRequirement
    {
        public CustomAuthorizeRequirement(CustomerRole role)
        {
            Role = role;
        }

        public CustomerRole Role { get; set; }
    }

    public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizeRequirement>
    {
        private readonly ISessionManager _sessionManager;

        public CustomAuthorizationHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizeRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                var isAuthenticated = IsAuthenticated(mvcContext.HttpContext.Request, requirement.Role);
                if (isAuthenticated)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var operationResult = OperationResult<bool>.Error(ResultCodes.Error.Session.UnAuthenticatedRequest);
                    mvcContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    mvcContext.HttpContext.Response.ContentType = "application/json";
                    string jsonString = JsonConvert.SerializeObject(operationResult);
                    mvcContext.HttpContext.Response.WriteAsync(jsonString, Encoding.UTF8);

                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }

        public bool IsAuthenticated(HttpRequest request, CustomerRole role)
        {
            if (role == CustomerRole.Public)
            {
                return true;
            }
            else if (!request.Headers.ContainsKey(CommonConstants.CustomerUserToken))
            {
                return false;
            }

            var userToken = request.Headers[CommonConstants.CustomerUserToken];
            var userAuthorization = _sessionManager.GetSessionData(userToken);

            var isAuthenticated = userAuthorization != null && userAuthorization.SessionKey.Valid();
            if (isAuthenticated && (role == CustomerRole.User || role == CustomerRole.Customer))
            {
                //httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //httpContext.Response.Redirect("UnAuthorizedRoute");
                return false;
            }
            else if (isAuthenticated && (role == CustomerRole.Company || role == CustomerRole.Customer))
            {
                return false;
            }
            else if (isAuthenticated && role == CustomerRole.SuperUser && userAuthorization.Role != CustomerRole.SuperUser)
            {
                return false;
            }
            return isAuthenticated;
        }
    }
}