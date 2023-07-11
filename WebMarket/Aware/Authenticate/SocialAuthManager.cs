using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.ECommerce;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Facebook;
using Aware.Util.Enums;

namespace Aware.Authenticate
{
    public class SocialAuthManager : ISocialAuthManager
    {
        private readonly ISessionManager _sessionManager;
        private readonly IApplication _application;
        private readonly ILogger _logger;

        private const string FB_SCOPES = "public_profile,email";
        private const string FB_REQUESTED_FIELDS = "first_name,last_name,id,gender,email,birthday";
        private const string FB_LOGIN_URL = "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}";
        private const string FB_TOKEN_URL = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}";

        private const string GP_SCOPES = "profile email";
        private const string GP_LOGIN_URL="https://accounts.google.com/o/oauth2/v2/auth?client_id={0}&redirect_uri={1}&scope={2}&response_type=code";
        private const string GP_TOKEN_URL = "https://www.googleapis.com/oauth2/v4/token";
        private const string GP_USERINFO_URL = "https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}";

        public SocialAuthManager(ISessionManager sessionManager, ILogger logger, IApplication application)
        {
            _sessionManager = sessionManager;
            _logger = logger;
            _application = application;
        }

        public string GetFacebookLoginUrl(string redirectUrl, List<string> scopeList = null)
        {
            var scopeInfo = scopeList != null && scopeList.Any() ? string.Join(",", scopeList) : FB_SCOPES;
            var url = string.Format(FB_LOGIN_URL, _application.Site.FacebookApiKey, GetRedirectUrl(redirectUrl), scopeInfo);
            return url;
        }

        public string GetGoogleLoginUrl(string redirectUrl, List<string> scopeList = null)
        {
            var scopeInfo = scopeList != null && scopeList.Any() ? string.Join(",", scopeList) : GP_SCOPES;
            var url = string.Format(GP_LOGIN_URL, _application.Site.GoogleApiKey, GetRedirectUrl(redirectUrl), scopeInfo);
            return url;
        }

        public Result ProcessFacebookLogin(string accessCode, string redirectUrl, List<string> scopeList = null)
        {
            try
            {
                var scopeInfo = scopeList != null && scopeList.Any() ? string.Join(",", scopeList) : FB_SCOPES;
                var uri = string.Format(FB_TOKEN_URL, _application.Site.FacebookApiKey, GetRedirectUrl(redirectUrl), scopeInfo, accessCode, _application.Site.FacebookApiSecret);
                var response = WebRequester.DoRequest<Dictionary<string, string>>(uri, false);

                //string[] vals = response.Split('&');
                //foreach (var token in vals)
                //{
                //    var index = token.IndexOf("=", StringComparison.Ordinal);
                //    tokens.Add(token.Substring(0, index), token.Substring(index + 1, token.Length - index - 1));
                //}

                if (response != null && !string.IsNullOrEmpty(response["access_token"]))
                {
                    var result = new FacebookClient(response["access_token"])
                        .Get<FacebookProfileModel>("me", new { fields = FB_REQUESTED_FIELDS });

                    var user = ToUser(result);
                    return _sessionManager.AuthorizeSocialUser(user);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SocialAuthManager > GetFacebookUserData - failed", ex);
            }
            return Result.Error("İşlem başarısız!");
        }

        public Result ProcessGoogleLogin(string accessCode, string redirectUrl)
        {
            try
            {
                var requestParams = new NameValueCollection
                {
                    {"client_id",  _application.Site.GoogleApiKey},
                    {"redirect_uri", GetRedirectUrl(redirectUrl)},
                    {"code", accessCode},
                    {"client_secret",  _application.Site.GoogleApiSecret},
                    {"grant_type", "authorization_code"}
                };

                var response = WebRequester.DoRequest<Dictionary<string, string>>(GP_TOKEN_URL, true, requestParams);
                if (response != null && !string.IsNullOrEmpty(response["access_token"]))
                {
                    var userInfoUrl = string.Format(GP_USERINFO_URL, response["access_token"]);
                    var userInfo = WebRequester.DoRequest<Dictionary<string, string>>(userInfoUrl, false);

                    if (userInfo != null)
                    {
                        var genderInfo = userInfo.ContainsKey("gender") ? userInfo["gender"] : string.Empty;
                        var gender = genderInfo == "male" ? GenderType.Male : (genderInfo == "female" ? GenderType.Female : GenderType.None);

                        var user = new User
                        {
                            Name = userInfo.ContainsKey("name") ? userInfo["name"] : string.Empty,
                            Email = userInfo.ContainsKey("email") ? userInfo["email"] : string.Empty,
                            Gender = gender
                        };
                        return _sessionManager.AuthorizeSocialUser(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SocialAuthManager > GetGoogleUserData - failed", ex);
            }
            return Result.Error("İşlem başarısız!");
        }

        public Result HandleSocialClient(FacebookProfileModel model)
        {
            var user = ToUser(model);
            return _sessionManager.AuthorizeSocialUser(user);
        }

        private string GetRedirectUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && url.StartsWith("/"))
            {
                return string.Format("{0}{1}", Config.DomainUrl, url);
            }
            return url;
        }

        public static string Fields
        {
            get
            {
                List<string> fieldList = new List<string>
                {
                    "first_name","last_name","id","gender",
                    "email",
                    "birthday"
                };

                return string.Join(",", fieldList);
            }
        }

        private User ToUser(FacebookProfileModel facebookUser)
        {
            if (facebookUser != null)
            {
                var user = new User()
                {
                    Name = string.Format("{0} {1}", facebookUser.first_name, facebookUser.last_name),
                    Email = facebookUser.email
                };

                switch (facebookUser.gender)
                {
                    case "female":
                        user.Gender = GenderType.Female;
                        break;
                    case "male":
                        user.Gender = GenderType.Male;
                        break;
                }

                if (!string.IsNullOrEmpty(facebookUser.birthday))
                {
                    user.BirthDate = DateTime.Parse(facebookUser.birthday);
                }
                return user;
            }
            return null;
        }
    }
}
