using System;
using System.Security.Cryptography;
using System.Text;
using Worchart.BL.Cache;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Model;
using Worchart.BL.User;

namespace Worchart.BL.Token
{
    public class TokenManager : ITokenManager
    {
        private readonly IApplicationManager _applicationManager;
        private readonly ILogger _logger;
        private readonly ICacher _cacher;

        public TokenManager(IApplicationManager applicationManager, ILogger logger, ICacher cacher)
        {
            _logger = logger;
            _applicationManager = applicationManager;
            _cacher = cacher;
        }

        public OperationResult<TokenResponse> GetToken(TokenRequest request)
        {
            var result = new OperationResult<TokenResponse>(ErrorConstants.CheckParameters);

            try
            {
                if (request != null && request.IsValid)
                {

                    if (request.Type == TokenType.AccessToken && !request.IsPublic)
                    {
                        if (!_cacher.HasKey(TokenConstants.AuthorizeTokenKey.FormatWith(request.AuthorizeToken.Trim())))
                        {
                            throw new TokenException(ErrorConstants.Token.InvalidAuthorizeToken, null);
                        }
                    }

                    var application = _applicationManager.First(i => i.ClientID == request.ClientID && i.Status == Enum.StatusType.Active);
                    if (application == null)
                    {
                        throw new TokenException(ErrorConstants.Token.ApplicationNotFound, null);
                    }
                    else if (!application.IsIpAllowed(request.RequestIp))
                    {
                        throw new TokenException(ErrorConstants.Token.IpAddressNotAllowed, null);
                    }
                    else if (request.IsPublic && application.IsPublic == 0)
                    {
                        throw new TokenException(ErrorConstants.Token.InvalidHash, null);
                    }
                    else if (!request.IsPublic && application.IsPublic == 1)
                    {
                        throw new TokenException(ErrorConstants.Token.InvalidApiKey, null);
                    }
                    else if (request.IsPublic && application.ClientSecret != request.ApiKey)
                    {
                        throw new TokenException(ErrorConstants.Token.InvalidApiKey, null);
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
                        result.Code = ErrorConstants.OperationSuccess;
                        if (request.Type == TokenType.AuthorizeToken)
                        {
                            result.Value = new TokenResponse()
                            {
                                CreateDate = DateTime.Now,
                                Token = Guid.NewGuid().ToString(),
                                Duration = TokenConstants.AuthorizeTokenTime
                            };

                            _cacher.Add(request.Key.FormatWith(result.Value.Token), "1", result.Value.Duration);
                        }
                        else
                        {
                            var refreshToken = request.Type == TokenType.RefreshToken ? request.AuthorizeToken.Trim() : string.Empty;
                            result.Value = GetAccessToken(application.ID, refreshToken);
                        }
                    }
                    else
                    {
                        var message = string.Format("[HashCompare|{0}|{1}]", request.TokenHash, calculatedHash);
                        throw new TokenException(ErrorConstants.Token.InvalidHash, new Exception(message));
                    }
                }
            }
            catch (TokenException tex)
            {
                var requestStr = request != null ? request.GetDescription() : string.Empty;
                _logger.Error("TokenManager|GetToken", "RequestStr:{0}", tex, requestStr);
                result.Code = tex.Code;
            }
            catch (Exception ex)
            {
                var requestStr = request != null ? request.GetDescription() : string.Empty;
                _logger.Error("TokenManager|GetToken", "RequestStr:{0}", ex, requestStr);
                result.Code = ErrorConstants.OperationFailed;
            }
            return result;
        }

        public bool CheckToken(TokenRequest tokenRequest)
        {
            try
            {
                var result = new OperationResult<bool>(ErrorConstants.CheckParameters);
                if (tokenRequest != null && tokenRequest.AuthorizeToken.Valid())
                {
                    var key = tokenRequest.Key.FormatWith(tokenRequest.AuthorizeToken);
                    if (_cacher.HasKey(key))
                    {
                        if (tokenRequest.Type == TokenType.AuthorizeToken)
                        {
                            return true;
                        }
                        else
                        {
                            var applicationID = _cacher.Get<int>(key, 0);
                            var accessTokenKey = tokenRequest.Key.FormatWith(applicationID);
                            var cachedToken = _cacher.Get<TokenResponse>(accessTokenKey);

                            if (cachedToken != null)
                            {
                                if (!cachedToken.CanRefresh())
                                {
                                    _cacher.Remove(accessTokenKey);
                                    _cacher.Remove(key);
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
                _logger.Error("TokenManager|CheckToken", "apiToken:{0}", ex, tokenRequest.GetDescription());
            }
            return false;
        }

        private TokenResponse GetAccessToken(int applicationID, string refreshToken = "")
        {
            TokenResponse accessToken = null;
            var key = TokenConstants.AccessTokenKey.FormatWith(applicationID);
            if (_cacher.HasKey(key))
            {
                accessToken = _cacher.Get<TokenResponse>(key);
                if (accessToken != null)
                {
                    if (!accessToken.CanRefresh())
                    {
                        _cacher.Remove(key);
                        accessToken = null;
                    }
                    else if (refreshToken.Valid() && accessToken.RefreshToken != refreshToken)
                    {
                        throw new TokenException(ErrorConstants.Token.InvalidRefreshToken, new Exception(refreshToken));
                    }
                }
            }

            if (accessToken == null)
            {
                accessToken = new TokenResponse()
                {
                    CreateDate = DateTime.Now,
                    Token = Guid.NewGuid().ToString(),
                    RefreshToken = Guid.NewGuid().ToString(),
                    Duration = TokenConstants.AccessTokenTime
                };

                _cacher.Add(key, accessToken, accessToken.Duration + TokenConstants.RefreshDuration);
                _cacher.Add(TokenConstants.AccessTokenKey.FormatWith(accessToken.Token), applicationID, accessToken.Duration + TokenConstants.RefreshDuration);
            }
            return accessToken;
        }

        public AuthorizedCustomerModel GetUserAuthorization(string userToken)
        {
            try
            {
                var key = CommonConstants.UserSessionKey.FormatWith(userToken.Trim());
                if (userToken.Valid() && _cacher.HasKey(key))
                {
                    var result = _cacher.Get<AuthorizedCustomerModel>(key);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("TokenManager|GetUserAuthorization", "userToken:{0}", ex, userToken);
            }
            return null;
        }

        public OperationResult AuthorizeUser(UserModel user)
        {
            try
            {
                if (user != null)
                {
                    var sessionKey = string.Empty;
                    var key = CommonConstants.UserInfoKey.FormatWith(user.ID);
                    if (_cacher.HasKey(key))
                    {
                        sessionKey = _cacher.Get(key, "");
                        _cacher.Remove(sessionKey);
                        _cacher.Remove(key);
                    }

                    var token = GenerateUserToken(user.ID);
                    var authorizeModel = new AuthorizedCustomerModel()
                    {
                        ID = user.ID,
                        CustomerToken = token,
                        CompanyID = user.CompanyID,
                        Email = user.Email,
                        Name = user.Name,
                        Status = user.Status,
                        Role = user.Role,
                        ExpireTime = DateTime.Now.AddMinutes(CommonConstants.CustomerUserTokenTime)
                    };

                    sessionKey = CommonConstants.UserSessionKey.FormatWith(token);
                    _cacher.Add(sessionKey, authorizeModel, CommonConstants.CustomerUserTokenTime);
                    _cacher.Add(key, sessionKey, CommonConstants.CustomerUserTokenTime);

                    return new OperationResult(ErrorConstants.OperationSuccess) { Value = authorizeModel };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("TokenManager|AuthorizeUser", ex);
            }
            return new OperationResult(ErrorConstants.OperationFailed);
        }

        public OperationResult LogoffUser(string userToken)
        {
            var resultCode = ErrorConstants.OperationFailed;
            try
            {
                if (userToken.Valid())
                {
                    var key = CommonConstants.UserSessionKey.FormatWith(userToken.Trim());
                    if (_cacher.HasKey(key))
                    {
                        _cacher.Remove(key);
                        resultCode = "000";
                    }
                    else
                    {
                        resultCode = ErrorConstants.Login.InvalidAuthorizeToken;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("TokenManager|LogoffUser", "userToken:{0}", ex, userToken);
            }
            return new OperationResult(resultCode);
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
            _logger.Error("TokenManager|GetSHA1|", "{0}|{1}", null, data, result);
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

        public OperationResult<TokenResponse> Simulate()
        {
            var application = _applicationManager.First(i => i.ID == 1 && i.Status == Enum.StatusType.Active);
            var accessToken = GetAccessToken(application.ID, string.Empty);
            return new OperationResult<TokenResponse>(ErrorConstants.OperationSuccess)
            {
                Value = accessToken
            };
        }
    }
}
