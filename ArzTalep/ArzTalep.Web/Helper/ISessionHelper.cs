using Aware.Model;

namespace ArzTalep.Web.Helper
{
    public interface ISessionHelper
    {
        int CurrentUserID { get; }

        SessionDataModel GetCurrentUser();

        void Set(string key, string value);

        void SetInt(string key, int value);

        void Remove(string key);
    }
}