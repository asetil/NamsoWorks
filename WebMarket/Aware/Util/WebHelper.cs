using System;
using System.Web;
using Newtonsoft.Json;

namespace Aware.Util
{
    public class WebHelper : IWebHelper
    {
        public string SessionValue(string key, string defaultValue = "")
        {
            var context = CurrentContext;
            if (!string.IsNullOrEmpty(key) && context != null && context.Session[key] != null)
            {
                return context.Session[key].ToString();
            }
            return defaultValue;
        }

        public void SetSession(string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                CurrentContext.Session[key] = value;
            }
        }

        public void RemoveSession(string key)
        {
            var context = CurrentContext;
            if (!string.IsNullOrEmpty(key) && context.Session[key] != null)
            {
                context.Session.Remove(key);
            }
        }

        public HttpCookie AddCookie(string name, string value, int expireTime = -1)
        {
            var cookie = new HttpCookie(name);
            cookie.Value = value;

            if (expireTime > 0)
            {
                cookie.Expires = DateTime.Now.AddMinutes(expireTime);
            }

            CurrentContext.Response.Cookies.Add(cookie);
            return cookie;
        }

        public HttpCookie GetCookie(string name)
        {
            return CurrentContext.Request.Cookies.Get(name);
        }

        public void RemoveCookie(string name)
        {
            var context = CurrentContext;
            var cookie = context.Request.Cookies.Get(name);
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                context.Response.Cookies.Add(cookie);
                context.Request.Cookies.Remove(name);
            }
        }

        public string Serialize(object item)
        {
            try
            {
                if (item!=null)
                {
                    var result = JsonConvert.SerializeObject(item);
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public T Deserialize<T>(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var deserializedResult = JsonConvert.DeserializeObject<T>(input);
                    return deserializedResult;
                }
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        public string Encode(string value)
        {
            return System.Web.HttpUtility.HtmlEncode(value);
        }

        public string Decode(string value)
        {
            return System.Web.HttpUtility.HtmlDecode(value);
        }

        public string UrlEncode(string value)
        {
            return System.Web.HttpUtility.UrlEncode(value);
        }

        private HttpContext CurrentContext
        {
            get
            {
                try
                {
                    var httpContext = HttpContext.Current;
                    if (httpContext != null && httpContext.Request != null)
                    {
                        return httpContext;
                    }
                }
                catch (Exception)
                {

                }
                return null;
            }
        }
    }
}
