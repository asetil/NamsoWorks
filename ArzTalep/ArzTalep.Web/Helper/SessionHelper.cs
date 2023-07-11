using ArzTalep.Web.Models;
using Aware.Manager;
using Aware.Model;
using Aware.Util;
using Microsoft.AspNetCore.Http;

namespace ArzTalep.Web.Helper
{
    public class SessionHelper : ISessionHelper
    {
        private readonly IUserManager _userManager;
        private readonly ISessionManager _sessionManager;
        private readonly IEncryptManager _encryptManager;
        public readonly HttpContext HttpContext;

        private int _currentUserID = -1;
        private SessionDataModel _currentUser;

        public SessionHelper(ISessionManager sessionManager, IUserManager userManager, IEncryptManager encryptManager, IHttpContextAccessor contextAccessor)
        {
            _sessionManager = sessionManager;
            _userManager = userManager;
            _encryptManager = encryptManager;
            HttpContext = contextAccessor.HttpContext;
        }

        public int CurrentUserID
        {
            get
            {
                if (_currentUserID == -1)
                {
                    var user = GetCurrentUser();
                    _currentUserID = user != null ? user.SessionKey.ToInt() : 0;
                }
                return _currentUserID == -1 ? 0 : _currentUserID;
            }
        }

        public SessionDataModel GetCurrentUser()
        {
            if (_currentUser == null)
            {
                var sessionKey = HttpContext.Session.GetString(WebConstants.AuthenticationSessionKey);
                if (sessionKey.Valid())
                {
                    _currentUser = _sessionManager.GetSessionData(sessionKey);
                    if (_currentUser == null)
                    {
                        _currentUser = _sessionManager.Authenticate(sessionKey);
                    }
                }

                if (_currentUser == null && HttpContext.Request.Cookies.ContainsKey(WebConstants.AuthorizationCookieKey))
                {
                    var cookie = HttpContext.Request.Cookies.TryGetValue(WebConstants.AuthorizationCookieKey, out string userIDEncrypted);
                    if (userIDEncrypted.Valid())
                    {
                        //var services = HttpContext.RequestServices;
                        //var encryptManager = (IEncryptManager)services.GetService(typeof(IEncryptManager));
                        sessionKey = _encryptManager.Decrypt(userIDEncrypted);
                        _currentUser = _sessionManager.Authenticate(sessionKey);

                        if (_currentUser != null)
                        {
                            Set(WebConstants.AuthenticationSessionKey, _currentUser.SessionKey);
                        }
                    }
                }
            }
            return _currentUser;
        }

        public void Set(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        public void SetInt(string key, int value)
        {
            HttpContext.Session.SetInt32(key, value);
        }

        public void Remove(string key)
        {
            HttpContext.Session.Remove(key);
        }
    }
}
