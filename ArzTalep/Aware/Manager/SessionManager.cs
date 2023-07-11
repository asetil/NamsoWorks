using Aware.Data;
using Aware.Model;
using Aware.Util;
using Aware.Util.Cache;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Exceptions;
using Aware.Util.Log;
using System;
using System.Security.Cryptography;
using System.Text;
using Aware.BL.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aware.Manager
{
    public class SessionManager : BaseManager<User>, ISessionManager
    {
        private readonly IConfiguration _configuration;

        public SessionManager(IConfiguration configuration, IRepository<User> repository, IAwareLogger logger, IAwareCacher cacher) : base(repository, logger, cacher)
        {
            _configuration = configuration;
        }

        public OperationResult<TokenResponse> GetAuthenticationToken(User user, int expireMinutes = CommonConstants.JwtTokenExpire)
        {
            try
            {
                var tokenKey = Encoding.UTF8.GetBytes(_configuration.GetValue("Jwt.SecretKey"));
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]{
                           new Claim(ClaimTypes.Name, user.Name),
                           new Claim(ClaimTypes.Role, ((int)user.Role).ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Success(new TokenResponse()
                {
                    Expires = tokenDescriptor.Expires.Value,
                    Token = tokenHandler.WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|GetAuthenticationToken", "user:{0}", ex, user.ID);
                throw ex;
            }
        }

        public OperationResult<TokenResponse> GetApiToken(TokenRequest request, ApplicationModel application)
        {
            var result = OperationResult<TokenResponse>.Error(ResultCodes.Error.CheckParameters);

            try
            {
                if (request != null && request.IsValid)
                {

                    if (request.Type == TokenType.AccessToken && !request.IsPublic)
                    {
                        if (!Cacher.HasKey(TokenConstants.AuthorizeTokenKey.FormatWith(request.AuthorizeToken.Trim())))
                        {
                            throw new TokenException(ResultCodes.Error.Session.InvalidAuthorizeToken, null);
                        }
                    }

                    //var application = _applicationManager.First(i => i.ClientID == request.ClientID && i.Status == StatusType.Active);
                    if (application == null)
                    {
                        throw new TokenException(ResultCodes.Error.Session.ApplicationNotFound, null);
                    }
                    else if (!application.IsIpAllowed(request.RequestIp))
                    {
                        throw new TokenException(ResultCodes.Error.Session.IpAddressNotAllowed, null);
                    }
                    else if (request.IsPublic && application.IsPublic == 0)
                    {
                        throw new TokenException(ResultCodes.Error.Session.InvalidHash, null);
                    }
                    else if (!request.IsPublic && application.IsPublic == 1)
                    {
                        throw new TokenException(ResultCodes.Error.Session.InvalidApiKey, null);
                    }
                    else if (request.IsPublic && application.ClientSecret != request.ApiKey)
                    {
                        throw new TokenException(ResultCodes.Error.Session.InvalidApiKey, null);
                    }

                    var calculatedHash = string.Empty;
                    if (!request.IsPublic)
                    {
                        var hashString = string.Format("{0}:{1}:{2}", application.ClientID, application.ClientSecret.ToLowerInvariant(), request.RequestTime);
                        if (request.Type != TokenType.AuthorizeToken)
                        {
                            var authorizeToken = request.AuthorizeToken.Valid() ? request.AuthorizeToken.Trim() : string.Empty;
                            hashString = string.Format("{0}:{1}", authorizeToken, hashString);
                        }

                        calculatedHash = GetSHA1(hashString);
                    }

                    if (request.IsPublic || calculatedHash == request.TokenHash)
                    {
                        result = OperationResult<TokenResponse>.Success(null);
                        if (request.Type == TokenType.AuthorizeToken)
                        {
                            result.SetValue(new TokenResponse()
                            {
                                Expires = DateTime.UtcNow.AddSeconds(TokenConstants.AuthorizeTokenTime),
                                Token = Guid.NewGuid().ToString(),
                            });

                            Cacher.Add(request.Key.FormatWith(result.Value.Token), "1", TokenConstants.AuthorizeTokenTime);
                        }
                        else
                        {
                            var refreshToken = request.Type == TokenType.RefreshToken ? request.AuthorizeToken.Trim() : string.Empty;
                            var accessToken = GetAccessToken(application.ID, refreshToken);
                            result.SetValue(accessToken);
                        }
                    }
                    else
                    {
                        var message = string.Format("[HashCompare|{0}|{1}]", request.TokenHash, calculatedHash);
                        throw new TokenException(ResultCodes.Error.Session.InvalidHash, new Exception(message));
                    }
                }
            }
            catch (TokenException tex)
            {
                var requestStr = request != null ? request.GetDescription() : string.Empty;
                Logger.Error("SessionManager|GetToken", "RequestStr:{0}", tex, requestStr);
                return OperationResult<TokenResponse>.Error(tex.Code);
            }
            catch (Exception ex)
            {
                var requestStr = request != null ? request.GetDescription() : string.Empty;
                Logger.Error("SessionManager|GetToken", "RequestStr:{0}", ex, requestStr);
                return OperationResult<TokenResponse>.Error(ResultCodes.Error.OperationFailed);
            }
            return result;
        }

        public bool CheckApiToken(TokenRequest tokenRequest)
        {
            try
            {
                if (tokenRequest != null && tokenRequest.AuthorizeToken.Valid())
                {
                    var key = tokenRequest.Key.FormatWith(tokenRequest.AuthorizeToken);
                    if (Cacher.HasKey(key))
                    {
                        if (tokenRequest.Type == TokenType.AuthorizeToken)
                        {
                            return true;
                        }
                        else
                        {
                            var applicationID = Cacher.Get<int>(key, 0);
                            var accessTokenKey = tokenRequest.Key.FormatWith(applicationID);
                            var cachedToken = Cacher.Get<TokenResponse>(accessTokenKey);

                            if (cachedToken != null)
                            {
                                if (!cachedToken.CanRefresh())
                                {
                                    Cacher.Remove(accessTokenKey);
                                    Cacher.Remove(key);
                                    return false;
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|CheckToken", "apiToken:{0}", ex, tokenRequest.GetDescription());
            }
            return false;
        }

        private TokenResponse GetAccessToken(int applicationID, string refreshToken = "")
        {
            TokenResponse accessToken = null;
            var key = TokenConstants.AccessTokenKey.FormatWith(applicationID);
            if (Cacher.HasKey(key))
            {
                accessToken = Cacher.Get<TokenResponse>(key);
                if (accessToken != null)
                {
                    if (!accessToken.CanRefresh())
                    {
                        Cacher.Remove(key);
                        accessToken = null;
                    }
                    else if (refreshToken.Valid() && accessToken.RefreshToken != refreshToken)
                    {
                        throw new TokenException(ResultCodes.Error.Session.InvalidRefreshToken, new Exception(refreshToken));
                    }
                }
            }

            if (accessToken == null)
            {
                accessToken = new TokenResponse()
                {
                    Expires = DateTime.UtcNow.AddSeconds(TokenConstants.AccessTokenTime),
                    Token = Guid.NewGuid().ToString(),
                    RefreshToken = Guid.NewGuid().ToString(),
                };

                var duration = TokenConstants.AccessTokenTime + TokenConstants.RefreshDuration;
                Cacher.Add(key, accessToken, duration);
                Cacher.Add(TokenConstants.AccessTokenKey.FormatWith(accessToken.Token), applicationID, duration);
            }
            return accessToken;
        }

        public bool Open(SessionDataModel data)
        {
            try
            {
                if (data != null)
                {
                    var sessionKey = CommonConstants.ApplicationSessionKey.FormatWith(data.SessionKey);
                    Cacher.Add(sessionKey, data, CommonConstants.CustomerUserTokenTime);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|Open", ex);
            }
            return false;
        }

        public SessionDataModel GetSessionData(string sessionKey)
        {
            try
            {
                var key = CommonConstants.ApplicationSessionKey.FormatWith(sessionKey);
                if (Cacher.HasKey(key))
                {
                    var result = Cacher.Get<SessionDataModel>(key);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|GetSessionData", "sessionKey:{0}", ex, sessionKey);
                throw ex;
            }
        }

        public SessionDataModel Authenticate(string sessionKey)
        {
            try
            {
                if (sessionKey.Valid() && sessionKey.ToInt() > 0)
                {
                    var user = Get(sessionKey.ToInt());
                    if (user != null && user.Status == StatusType.Active)
                    {
                        var result = new SessionDataModel()
                        {
                            SessionKey = user.ID.ToString(),
                            Name = user.Name,
                            Role = user.Role
                        };

                        if (Open(result))
                        {
                            return result;
                        }
                        return null;
                    }
                }
                throw new AwareException(ResultCodes.Error.Login.UserNotFound);
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|AuthorizeUser", "sessionKey:{0}", ex, sessionKey);
            }
            return null;
        }

        public bool Terminate(string sessionKey)
        {
            try
            {
                if (sessionKey.Valid())
                {
                    var key = CommonConstants.ApplicationSessionKey.FormatWith(sessionKey);
                    if (Cacher.HasKey(key))
                    {
                        Cacher.Remove(key);
                        return true;
                    }
                    else
                    {
                        throw new AwareException(ResultCodes.Error.Session.InvalidKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SessionManager|Logoff", "sessionKey:{0}", ex, sessionKey);
                throw ex;
            }
            return false;
        }

        private string GenerateUserToken(int userID)
        {
            return Guid.NewGuid().ToString();
        }

        private string GetSHA1(string data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);

            var result = GetHexaDecimal(inputbytes).ToUpperInvariant();
            Logger.Error("SessionManager|GetSHA1|", "{0}|{1}", null, data, result);
            return result;
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

        public OperationResult<TokenResponse> Simulate(ApplicationModel application)
        {
            //var application = _applicationManager.First(i => i.ID == 1 && i.Status == StatusType.Active);
            var accessToken = GetAccessToken(application.ID, string.Empty);
            return OperationResult<TokenResponse>.Success(accessToken);
        }

        protected override OperationResult<User> OnBeforeUpdate(ref User existing, User model)
        {
            throw new NotImplementedException();
        }
    }
}
