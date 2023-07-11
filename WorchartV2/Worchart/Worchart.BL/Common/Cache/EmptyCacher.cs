using System.Collections.Generic;
using System.Linq;

namespace Worchart.BL.Cache
{
    /// <summary>
    /// Use for unit test purpose or in admin panel scenarious
    /// </summary>
    public class EmptyCacher : ICacher
    {
        public void Dispose()
        {

        }

        public bool Add(string key, object value, int cacheTime = 1440)
        {
            return true;
        }

        public T Get<T>(string key)
        {
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public bool Remove(string key)
        {
            return true;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return Enumerable.Empty<string>();
        }

        public bool HasKey(string key)
        {
            return false;
        }
    }
}
