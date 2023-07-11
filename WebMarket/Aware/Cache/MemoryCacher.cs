using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Aware.Cache
{
    public class MemoryCacher : ICacher
    {
        protected ObjectCache Instance
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        public virtual bool Add(string key, object value, int cacheTime = 1440)
        {
            try
            {
                if (Instance != null && !string.IsNullOrEmpty(key) && value != null)
                {
                    cacheTime = cacheTime <= 0 ? 1440 : cacheTime;
                    Remove(key);
                    var policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
                    return Instance.Add(new CacheItem(key, value), policy);
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool IsSet(string key)
        {
            return Instance.Contains(key);
        }

        public virtual T Get<T>(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    return (T)Instance.Get(key);
                }
            }
            catch (Exception)
            {

            }
            return default(T);
        }

        public virtual T Get<T>(string key, T defaultValue)
        {
            try
            {
                return (T)Instance.Get(key);
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }

        public virtual bool Remove(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    if (IsSet(key))
                    {
                        Instance.Remove(key);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Instance)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        public void Clear()
        {
            foreach (var item in Instance)
                Remove(item.Key);
        }

        public IEnumerable<string> GetAllKeys()
        {
            return Instance.Select(i => i.Key);
        }

        public void Dispose()
        {

        }
    }
}
