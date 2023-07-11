using Aware.Authenticate.Model;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aware.Authenticate
{
    public class SessionManager : ISessionManager
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IWebHelper _contextHelper;
        private const int MONTH_TIME = 60 * 24 * 30;

        public SessionManager(IUserService userService, IWebHelper contextHelper, ILogger logger)
        {
            _userService = userService;
            _contextHelper = contextHelper;
            _logger = logger;
        }

        public Result Authorize(User user)
        {
            var authorizeResult = _userService.IsAuthorized(user);
            if (authorizeResult.OK)
            {
                var persistedUser = authorizeResult.ValueAs<User>();
                var userInfoStr = string.Format("{0};{1};{2};{3}", persistedUser.ID, persistedUser.Name,(int)persistedUser.Role, persistedUser.CustomerID);
                _contextHelper.SetSession("UserInfo", userInfoStr);

                if (user.RememberMe)
                {
                    var principal = new CustomPrincipal()
                    {
                        ID = persistedUser.ID,
                        Name = persistedUser.Name,
                        Role = persistedUser.Role,
                        CustomerID = persistedUser.CustomerID
                    };

                    var cookieValue = Encryptor.Encrypt(Common.Serialize(principal));
                    _contextHelper.AddCookie(Constants.UserInfoCookie, cookieValue, MONTH_TIME);
                }
            }
            return authorizeResult;
        }

        public Result AuthorizeSocialUser(User user)
        {
            try
            {
                if (user != null)
                {
                    var persistUser = _userService.GetUserByEmail(user.Email);
                    if (persistUser == null)
                    {
                        var registerResult = _userService.Register(user, true);
                        persistUser = registerResult.OK ? registerResult.ValueAs<User>() : null;
                    }

                    if (persistUser != null)
                    {
                        Authorize(persistUser);
                        return Result.Success(persistUser, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                var email = user != null ? user.Email : "null email";
                _logger.Error(string.Format("UserService > ProcessSocialUser - failed : {0}", email), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public void Authenticate(ref CustomPrincipal principal)
        {
            try
            {
                if (principal == null || principal.ID <= 0)
                {
                    var userInfo = _contextHelper.SessionValue("UserInfo").Replace(";;", ";").Split(';');
                    if (userInfo.Length > 1)
                    {
                        principal = new CustomPrincipal()
                        {
                            ID = userInfo[0].Int(),
                            Name = userInfo[1],
                            Role = (UserRole)userInfo[2].Int(),
                            CustomerID = userInfo[3].Int()
                        };
                    }

                    if (principal == null)
                    {
                        var userCookie = _contextHelper.GetCookie(Constants.UserInfoCookie);
                        if (userCookie != null && !string.IsNullOrEmpty(userCookie.Value))
                        {
                            var value = Encryptor.Decrypt(userCookie.Value);
                            principal = Common.DeSerialize<CustomPrincipal>(value);

                            if (principal != null)
                            {
                                var userInfoStr = string.Format("{0};{1};{2};{3}", principal.ID, principal.Name,(int)principal.Role,principal.CustomerID);
                                _contextHelper.SetSession("UserInfo", userInfoStr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserService > Authenticate - failed", ex);
            }
        }

        public void LoginAs(int userID)
        {
            if (userID > 0)
            {
                var demoUser = Config.IsAdmin ? _userService.GetAdminUser(userID) : _userService.GetUser(userID);
                Authorize(demoUser);
            }
        }

        public void Logout()
        {
            _contextHelper.RemoveSession("UserInfo");
            _contextHelper.RemoveCookie(Constants.UserInfoCookie);
        }

        public int GetCurrentRegion()
        {
            var result = GetLanguageRegion();
            return result.Item2;
        }

        public string GetCurrentLanguage()
        {
            var language = _contextHelper.SessionValue(Constants.LangSessionKey);
            if (string.IsNullOrEmpty(language))
            {
                var result = GetLanguageRegion();
                language = string.IsNullOrEmpty(result.Item1) ? Constants.DefaultLanguage : result.Item1;
                _contextHelper.SetSession(Constants.LangSessionKey, language);
            }
            return language;
        }

        public bool SetCurrentLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                _contextHelper.SetSession(Constants.LangSessionKey, language);
                SetLanguageRegionCookie();
                return true;
            }
            return false;
        }

        public bool SetCurrentRegion(int regionID)
        {
            if (regionID > 0)
            {
                SetLanguageRegionCookie(regionID);
                return true;
            }
            return false;
        }

        public List<int> GetDisplayedNotifications()
        {
            var result = new List<int>();
            var cookie = _contextHelper.GetCookie("NotificationList");
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                result = cookie.Value.Trim().Split(',').Select(i => i.Int()).ToList();
            }
            return result;
        }

        public void SetNotificationCookie(List<int> displayedList, bool hasNotification)
        {
            if (displayedList != null)
            {
                _contextHelper.SetSession("HasNotification", hasNotification ? 1 : 0);

                var value = string.Join(",", displayedList);
                _contextHelper.AddCookie("NotificationList", value, MONTH_TIME);
            }
        }

        public bool HasNewNotifiction()
        {
            var hasNotification = _contextHelper.SessionValue("HasNotification", "1") == "1";
            return hasNotification;
        }

        private Tuple<string, int> GetLanguageRegion()
        {
            var cookie = _contextHelper.GetCookie(Constants.LocalizationCookie);
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                var values = cookie.Value.Split('|');
                var language = values[0];
                var regionID = values.Length > 1 ? values[1].Int() : 0;
                return new Tuple<string, int>(language, regionID);
            }
            return new Tuple<string, int>(string.Empty, 0);
        }

        private void SetLanguageRegionCookie(int regionID = 0)
        {
            regionID = regionID > 0 ? regionID : GetCurrentRegion();
            var language = GetCurrentLanguage();

            var cookieValue = string.Format("{0}|{1}", language, regionID);
            _contextHelper.AddCookie(Constants.LocalizationCookie, cookieValue, MONTH_TIME);
        }
    }
}
