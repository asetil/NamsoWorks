using System;
using System.Collections.Generic;

namespace Aware.Cache
{
    public interface ICacher : IDisposable
    {
        bool Add(string key, object value, int cacheTime = 1440);
        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);
        bool Remove(string key);
        IEnumerable<string> GetAllKeys();
    }
}
