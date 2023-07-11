using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Worchart.UI.Util;

namespace Worchart.UI.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly WebRequester _webRequester;
        public TokenController()
        {
            _webRequester = new WebRequester(Constants.ApiBaseUrl);
        }

        [HttpGet("refresh")]
        public OperationResult<TokenResponse> RefreshAccessToken()
        {
            HttpContext.Session.Remove("AccessToken");
            Response.Cookies.Delete("AccessToken");

            return GetAccessToken();
        }

        [HttpGet("accesstoken")]
        public OperationResult<TokenResponse> GetAccessToken()
        {
            var response = new OperationResult<TokenResponse>();

            var accessToken = HttpContext.Session.Keys.Any(i => i == "AccessToken") ? HttpContext.Session.GetString("AccessToken") : string.Empty;
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                }
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                response = GetAuthorizeToken();
                if (response != null && response.Success)
                {
                    var requestTime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss fff");
                    var tokenResponse = (response.Value as TokenResponse);
                    var authorizeToken = tokenResponse != null ? tokenResponse.Token : string.Empty;

                    var hashString = string.Format("{0}:{1}:{2}:{3}", authorizeToken, Constants.ClientID, Constants.ClientSecret, requestTime);
                    var calculatedHash = GetSHA1(hashString);

                    var tokenParams = new
                    {
                        clientID = Constants.ClientID,
                        authorizeToken,
                        requestTime,
                        tokenHash = calculatedHash
                    };

                    response = _webRequester.PostJson<OperationResult<TokenResponse>>("/token/accesstoken", tokenParams);
                    if (response != null && response.Success)
                    {
                        tokenResponse = (response.Value as TokenResponse);
                        accessToken = tokenResponse.Token;
                        HttpContext.Session.SetString("AccessToken", accessToken);

                        Response.Cookies.Append("AccessToken", accessToken, new CookieOptions()
                        {
                            Expires = DateTime.Now.AddSeconds(tokenResponse.ValidityPeriod),
                            HttpOnly = true
                        });
                    }
                }
            }
            else
            {
                response.Value = new TokenResponse()
                {
                    Token = accessToken
                };
            }
            return response;
        }

        private OperationResult<TokenResponse> GetAuthorizeToken()
        {
            var requestTime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss fff");
            var hashString = string.Format("{0}:{1}:{2}", Constants.ClientID, Constants.ClientSecret, requestTime);
            var calculatedHash = GetSHA1(hashString);

            var authorizeParams = new
            {
                clientID = Constants.ClientID,
                requestTime,
                tokenHash = calculatedHash
            };

            var result = _webRequester.PostJson<OperationResult<TokenResponse>>("/token/authorize", authorizeParams);
            return result;
        }

        private string GetSHA1(string data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes).ToUpperInvariant();
        }

        private string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(string.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }
        public int ValidityPeriod { get; set; }
        public string RefreshToken { get; set; }
    }

    public class OperationResult<T>
    {
        public bool Success { get { return Code == "000"; } }
        public string Code { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
    }
}
