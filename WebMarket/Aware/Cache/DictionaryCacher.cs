using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Aware.Cache
{
    public class DictionaryCacher<T>
    {
        private readonly ReaderWriterLockSlim _addRemoveLock;
        private const int DefaultExpireHour = 24;
        
        public DictionaryCacher()
        {
            DefaultExpiration = new TimeSpan(0, DefaultExpireHour, 0, 0);
            _addRemoveLock = new ReaderWriterLockSlim();
        }

        #region Fields and Properties

        private readonly Dictionary<T, CacheItem> _mItems = new Dictionary<T, CacheItem>();
        public dynamic this[T key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value, DefaultExpiration); }
        }

        private TimeSpan DefaultExpiration { get; set; }

        #endregion

        #region Methods

        public bool ContainsKey(T key)
        {
            return _mItems.ContainsKey(key) && !_mItems[key].HasExpired;
        }

        public void SetValue(T key, dynamic value)
        {
            SetValue(key, value, DefaultExpiration);
        }

        private void SetValue(T key, dynamic value, TimeSpan expiration)
        {
            _addRemoveLock.EnterReadLock();
            try
            {
                if (_mItems.ContainsKey(key))
                {
                    _mItems.Remove(key);
                }

                if (value != null)
                {
                    _mItems.Add(key, new CacheItem(key, value, DateTime.Now.Add(expiration)));
                }
            }
            catch (Exception ex)
            {
            }
            _addRemoveLock.ExitReadLock();
        }

        public void ClearValue(T key)
        {
            SetValue(key, null);
        }

        public dynamic GetValue(T key)
        {
            dynamic result = null;

            _addRemoveLock.EnterReadLock();

            try
            {
                if (_mItems.ContainsKey(key))
                {
                    var item = _mItems[key];
                    if (!item.HasExpired)
                    {
                        result = item.Value;
                    }
                }
            }
            catch (Exception)
            {
            }

            _addRemoveLock.ExitReadLock();

            return result;
        }

        public void RemoveKey(T key)
        {
            _addRemoveLock.EnterReadLock();

            try
            {
                if (_mItems.ContainsKey(key))
                {
                    _mItems.Remove(key);
                }
            }
            catch (Exception)
            {
            }

            _addRemoveLock.ExitReadLock();
        }

        public T2 GetValue<T2>(T key, T2 defaultVal)
        {
            var value = GetValue(key);
            return value == null ? defaultVal : (T2)value;
        }

        public void PurgeCache()
        {
            _addRemoveLock.EnterReadLock();
            try
            {
                var items = _mItems.Values;
                foreach (var item in items.Where(item => item.HasExpired))
                {
                    _mItems.Remove(item.Key);
                }
            }
            catch (Exception)
            {
            }
            _addRemoveLock.ExitReadLock();
        }

        public void ClearCache()
        {
            _addRemoveLock.EnterReadLock();
            try
            {
                _mItems.Clear();
            }
            catch (Exception)
            {
            }
            _addRemoveLock.ExitReadLock();
        }

        #endregion

        #region CacheItem

        private class CacheItem
        {
            public CacheItem(T key, dynamic value, DateTime expirationDate)
            {
                Key = key;
                ExpirationDate = expirationDate;
                _mValRef = value;
            }

            private readonly dynamic _mValRef;

            public T Key { get; private set; }

            public dynamic Value
            {
                get
                {
                    return _mValRef;
                }
            }

            private DateTime ExpirationDate { get; set; }

            public bool HasExpired
            {
                get
                {
                    return !(DateTime.Now < ExpirationDate);
                }
            }
        }

        #endregion

        public int GetCount()
        {
            return _mItems.Count();
        }

        public T GetLastKey()
        {
            T result = default(T);

            _addRemoveLock.EnterReadLock();

            try
            {
                if (_mItems.Count > 0)
                {
                    result = _mItems.Keys.Last();
                }
            }
            catch (Exception)
            {
            }

            _addRemoveLock.ExitReadLock();

            return result;
        }

    }
}