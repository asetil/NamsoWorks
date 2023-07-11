using System.Web;

namespace Aware.Util
{
    public interface IWebHelper
    {
        HttpCookie AddCookie(string name, string value, int expireTime = -1);
        HttpCookie GetCookie(string name);
        void RemoveCookie(string name);
        void RemoveSession(string key);
        string SessionValue(string key, string defaultValue = "");
        void SetSession(string key, object value);
        string Serialize(object item);
        T Deserialize<T>(string input);
        string UrlEncode(string value);
        string Encode(string value);
        string Decode(string value);
    }
}
