using ArzTalep.Web.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ArzTalep.Web.Helper
{
    public class AuthenticateCustomerHandlerOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }

    public class AuthenticateCustomerHandler : AuthenticationHandler<AuthenticateCustomerHandlerOptions>
    {
        private readonly ISessionHelper _sessionHelper;
        public AuthenticateCustomerHandler(ISessionHelper sessionHelper, IOptionsMonitor<AuthenticateCustomerHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _sessionHelper = sessionHelper;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var isPathAllowed = IsPathAllowed(Request.Path.Value);
            if (isPathAllowed)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            //if (!Request.Headers.ContainsKey(CommonConstants.CustomerUserToken))
            //{
            //    return Task.FromResult(AuthenticateResult.Fail("Request contains no customer user token!"));
            //}
            //else
            //{
            //    var userToken = Request.Headers[CommonConstants.CustomerUserToken];
            //    var userAuthorization = _tokenValidator.GetUserAuthorization(userToken);
            //    if (userAuthorization == null)
            //    {
            //        return Task.FromResult(AuthenticateResult.Fail("User is not authenticated!"));
            //    }
            //}

            if (_sessionHelper.CurrentUserID <= 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("User is not authenticated!"));
            }

            var user = _sessionHelper.GetCurrentUser();
            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, ((int)user.Role).ToString()),
                new Claim("UserId", user.SessionKey)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        //protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
        //    await base.HandleChallengeAsync(properties);
        //}

        private bool IsPathAllowed(string path)
        {
            var allowedPaths = new List<string>() { "/hesap/uye-girisi", "/account" };
            return allowedPaths.Contains(path.ToLowerInvariant());
        }
    }
}
