using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Worchart.BL.Constants;
using Worchart.BL.Enum;
using Worchart.BL.Model;
using Worchart.BL.Token;

namespace Worchart.API.Middleware
{
    public class AuthorizeCustomerRequirement : IAuthorizationRequirement
    {
        public AuthorizeCustomerRequirement(CustomerRole role)
        {
            Role = role;
        }

        public CustomerRole Role { get; set; }
    }

    public class AuthorizeCustomerHandler : AuthorizationHandler<AuthorizeCustomerRequirement>
    {
        private readonly ITokenManager _tokenValidator;

        public AuthorizeCustomerHandler(ITokenManager tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeCustomerRequirement requirement)
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
                    var operationResult = new OperationResult(ErrorConstants.Token.UnAuthenticatedRequest);
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
            var userAuthorization = _tokenValidator.GetUserAuthorization(userToken);

            var isAuthenticated = userAuthorization != null && userAuthorization.ID > 0;
            if (isAuthenticated && (role == CustomerRole.User || role == CustomerRole.Customer) && userAuthorization.CompanyID > 0)
            {
                //httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //httpContext.Response.Redirect("UnAuthorizedRoute");
                return false;
            }
            else if (isAuthenticated && (role == CustomerRole.Company || role == CustomerRole.Customer) && userAuthorization.CompanyID <= 0)
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