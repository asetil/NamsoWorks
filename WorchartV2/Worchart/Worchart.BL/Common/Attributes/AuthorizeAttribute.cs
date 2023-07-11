//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Linq;
//using System.Net;
//using Worchart.BL.Enum;
//using Worchart.BL.Token;

//namespace Worchart.BL.Attributes
//{
//    public class WorchartAuthorizeAttribute : AuthorizeAttribute
//    {
//        private readonly CustomerRole _role;
//        private readonly AuthorityType[] _authorities;
//        private readonly string[] _excludeList;
//        private bool _isExcluded;

//        public WorchartAuthorizeAttribute(CustomerRole role = CustomerRole.Customer, AuthorityType[] authorities = null, string[] excludeList = null)
//        {
//            _role = role;
//            _authorities = authorities;
//            _excludeList = excludeList;
//        }

//        public WorchartAuthorizeAttribute(CustomerRole role = CustomerRole.Customer, string exclude = "")
//        {
//            _role = role;
//            if (exclude.Valid())
//            {
//                _excludeList = new[] { exclude };
//            }
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            if (_excludeList != null && _excludeList.Any())
//            {
//                //var controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
//                //var actionName = context.ActionDescriptor.DisplayName;

//                //var requestName = string.Format("{0}/{1}", controllerName, actionName);
//                //_isExcluded = _excludeList.Contains(requestName);

//                if (!_isExcluded)
//                {
//                    //context.Result = ;
//                    AuthorizeCore(context.HttpContext);
//                }
//            }
//        }


//        protected bool AuthorizeCore(HttpContext httpContext)
//        {
//            if (!_isExcluded)
//            {
//                var services = httpContext.RequestServices;
//                var tokenManager = (ITokenManager)services.GetService(typeof(ITokenManager));
//                var userToken = string.Empty;
//                var hasUserToken = httpContext.Request.Headers.ContainsKey("UserToken");
//                if (hasUserToken)
//                {
//                    userToken = httpContext.Request.Headers["UserToken"];
//                }

//                var userAuthorization = tokenManager.GetUserAuthorization(userToken);

//                var isAuthenticated = _role == CustomerRole.None || (userAuthorization != null && userAuthorization.ID > 0);
//                if (isAuthenticated && (_role == CustomerRole.User || _role == CustomerRole.Customer) && userAuthorization.FirmID > 0)
//                {
//                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    httpContext.Response.Redirect("UnAuthorizedRoute");
//                }
//                else if (isAuthenticated && (_role == CustomerRole.Firm || _role == CustomerRole.Customer) && userAuthorization.FirmID <= 0)
//                {
//                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    httpContext.Response.Redirect("UnAuthorizedRoute");
//                }
//                else if (isAuthenticated && _role == CustomerRole.SuperUser && userAuthorization.Role != CustomerRole.SuperUser)
//                {
//                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    httpContext.Response.Redirect("UnAuthorizedRoute");
//                }
//                return isAuthenticated;
//            }
//            return true;
//        }

//        public void OnActionExecuting(ActionExecutingContext context)
//        {
//            throw new NotImplementedException();
//        }

//        public void OnActionExecuted(ActionExecutedContext context)
//        {
//            throw new NotImplementedException();
//        }

//        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//        //{
//        //    try
//        //    {
//        //        var httpContext = filterContext.HttpContext;
//        //        var request = httpContext.Request;

//        //        if (request.IsAjaxRequest())
//        //        {
//        //            var response = httpContext.Response;
//        //            var user = httpContext.User;

//        //            if (user == null || user.Identity.IsAuthenticated == false)
//        //                response.StatusCode = (int)HttpStatusCode.Unauthorized;
//        //            else
//        //                response.StatusCode = (int)HttpStatusCode.Forbidden;

//        //            response.SuppressFormsAuthenticationRedirect = true;
//        //            //response.End();
//        //        }
//        //        else
//        //        {
//        //            //var loginUrl = string.Format("/uye-girisi?ReturnUrl={0}", request.Url.AbsolutePath);
//        //            //filterContext.Result = new RedirectResult(loginUrl);
//        //            //return;
//        //        }
//        //    }
//        //    catch (Exception)
//        //    {
//        //        // ignored
//        //    }
//        //    base.HandleUnauthorizedRequest(filterContext);
//        //}
//    }
//}