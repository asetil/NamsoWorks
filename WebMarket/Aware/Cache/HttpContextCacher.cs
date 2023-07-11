using System;
using System.Collections.Generic;
using System.Web;

namespace Aware.Cache
{
    public class HttpContextCacher : ICacher
    {
        public System.Web.Caching.Cache Instance
        {
            get
            {
                return HttpContext.Current.Cache;
            }
        }

        public bool Add(string key, object value, int cacheTime = 1440)
        {
            try
            {
                if (cacheTime <= 0)
                {
                    Instance.Insert(key, value);
                }
                else
                {
                    Instance.Insert(key, value, null, DateTime.UtcNow.AddMinutes(cacheTime), TimeSpan.Zero);
                }
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public T Get<T>(string key)
        {
            try
            {
                var value = Instance.Get(key);
                if (value != null)
                {
                    return (T)value;
                }
            }
            catch (Exception)
            {

            }
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            try
            {
                var value = Instance.Get(key);
                if (value != null)
                {
                    return (T)value;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }

        public bool Remove(string key)
        {
            try
            {
                Instance.Remove(key);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public IEnumerable<string> GetAllKeys()
        {
            var enumerator = Instance.GetEnumerator();
            var keyList=new List<string>();
            while (enumerator.MoveNext())
            {
                keyList.Add(enumerator.Key.ToString());
            }
            return keyList;
        }

        public void Dispose()
        {
            
        }
    }
}