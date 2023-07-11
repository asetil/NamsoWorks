using System;
using System.Collections.Generic;

namespace Aware.Util.Cache
{
    public interface IAwareCacher : IDisposable
    {
        bool Add(string key, object value, int cacheDuration);

        bool HasKey(string key);

        T Get<T>(string key);

        T Get<T>(string key, T defaultValue);

        bool Remove(string key);

        IEnumerable<string> GetAllKeys();
    }
}
