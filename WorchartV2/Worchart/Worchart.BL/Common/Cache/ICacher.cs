using System;
using System.Collections.Generic;

namespace Worchart.BL.Cache
{
    public interface ICacher : IDisposable
    {
        /// <summary>
        /// Add value to cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDuration">duration in seconds</param>
        /// <returns></returns>
        bool Add(string key, object value, int cacheDuration);

        bool HasKey(string key);

        T Get<T>(string key);

        T Get<T>(string key, T defaultValue);

        bool Remove(string key);

        IEnumerable<string> GetAllKeys();
    }
}
