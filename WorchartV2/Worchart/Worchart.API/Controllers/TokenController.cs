using Microsoft.AspNetCore.Mvc;
using Worchart.BL.Model;
using Worchart.BL.Token;
using Worchart.BL.User;

namespace Worchart.API.Controllers
{
    public class TokenController : BaseController
    {
        private readonly ITokenManager _tokenManager;
        public TokenController(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        [HttpPost("authorize")]
        public ActionResult<OperationResult<TokenResponse>> GetAuthorizeToken(TokenRequest request)
        {
            request.RequestIp = CurrentIpAddress;
            request.Type = TokenType.AuthorizeToken;
            return _tokenManager.GetToken(request);
        }

        [HttpPost("accesstoken")]
        public ActionResult<OperationResult<TokenResponse>> GetAccessToken(TokenRequest request)
        {
            request.RequestIp = CurrentIpAddress;
            request.Type = TokenType.AccessToken;
            return _tokenManager.GetToken(request);
        }

        [HttpPost("refresh")]
        public ActionResult<OperationResult<TokenResponse>> RefreshAccessToken(TokenRequest request)
        {
            request.RequestIp = CurrentIpAddress;
            request.Type = TokenType.RefreshToken;
            return _tokenManager.GetToken(request);
        }

        [HttpPost("simulate")]
        public ActionResult<OperationResult<TokenResponse>> Simulate()
        {
            var accessToken = _tokenManager.Simulate();
            if (accessToken.Success)
            {
                var services = HttpContext.RequestServices;
                var userManager = (IUserManager)services.GetService(typeof(IUserManager));
                var loginResult = userManager.Login(new LoginRequestModel()
                {
                    Email = "osman.sokuoglu@gmail.com",
                    Password = "sngrlu19"
                });

                accessToken.Value.RefreshToken = loginResult.ValueAs<string>();

            };
            return accessToken;
        }
    }
}
