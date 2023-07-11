using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Dependency;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.Util.Filter
{
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly AuthorizeLevel _level;
        private readonly string[] _excludeList;
        private bool _isExcluded;

        public AuthorizeAttribute(AuthorizeLevel level = AuthorizeLevel.Authenticated, string[] excludeList = null)
        {
            _level = level;
            _excludeList = excludeList;
        }

        public AuthorizeAttribute(string exclude, AuthorizeLevel level = AuthorizeLevel.Authenticated)
        {
            _level = level;
            _excludeList = new[] { exclude };
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (_excludeList != null && _excludeList.Any())
            {
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;

                var requestName = string.Format("{0}/{1}", controllerName, actionName);
                _isExcluded = _excludeList.Contains(requestName);
            }
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!_isExcluded)
            {
                var principal = httpContext.User as CustomPrincipal;
                if (principal == null)
                {
                    var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
                    sessionManager.Authenticate(ref principal);

                    if (principal != null)
                    {
                        principal.Identity = httpContext.User.Identity;
                        httpContext.User = principal;
                    }
                }

                var isAuthenticated = _level == AuthorizeLevel.None || (principal != null && principal.ID > 0);
                if (_level == AuthorizeLevel.Admin && isAuthenticated && !principal.IsAdmin)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    httpContext.Response.RedirectToRoute("UnAuthorizedRoute");
                }

                if (_level == AuthorizeLevel.SuperUser && isAuthenticated && principal.Role != UserRole.SuperUser)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    httpContext.Response.RedirectToRoute("UnAuthorizedRoute");
                }
                return isAuthenticated;
            }
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {
                var httpContext = filterContext.HttpContext;
                var request = httpContext.Request;

                if (request.IsAjaxRequest())
                {
                    var response = httpContext.Response;
                    var user = httpContext.User;

                    if (user == null || user.Identity.IsAuthenticated == false)
                        response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    else
                        response.StatusCode = (int) HttpStatusCode.Forbidden;

                    response.SuppressFormsAuthenticationRedirect = true;
                    //response.End();
                }
                else
                {
                    //var loginUrl = string.Format("/uye-girisi?ReturnUrl={0}", request.Url.AbsolutePath);
                    //filterContext.Result = new RedirectResult(loginUrl);
                    //return;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}