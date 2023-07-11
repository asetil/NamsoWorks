using Aware.Authenticate.Model;
using Aware.Util.Model;
using System;
using System.Collections.Generic;

namespace Aware.Authenticate
{
    public interface ISessionManager
    {
        Result Authorize(User user);
        Result AuthorizeSocialUser(User user);
        void Authenticate(ref CustomPrincipal principal);
        void Logout();
        void LoginAs(int userID);

        int GetCurrentRegion();
        string GetCurrentLanguage();
        bool SetCurrentLanguage(string language);
        bool SetCurrentRegion(int regionID);
        List<int> GetDisplayedNotifications();
        void SetNotificationCookie(List<int> displayedList, bool hasNotification);
        bool HasNewNotifiction();
    }
}
