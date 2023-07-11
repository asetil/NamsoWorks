//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using Aware.Util;
//using Aware.Util.Log;

//namespace Aware.Cache
//{
//    public class RedisCacher : ICacher
//    {
//        private ConnectionMultiplexer _connectionMultiplexer;
//        private readonly IWebHelper _webHelper;
//        private readonly ILogger _logger;

//        public RedisCacher(ILogger logger, IWebHelper webHelper)
//        {
//            _logger = logger;
//            _webHelper = webHelper;
//        }
        
//        #region Methods
       
//        public bool Add(string key, object value, int cacheTime = 1440)
//        {
//            if (value != null)
//            {
//                Database.StringSetAsync(GetLocalizedKey(key), _webHelper.Serialize(value), TimeSpan.FromMinutes(cacheTime));
//            }
//            return false;
//        }

//        public T Get<T>(string key)
//        {
//            return Get<T>(key,default(T));
//        }

//        public T Get<T>(string key, T defaultValue)
//        {
//            var redisValue = Database.StringGet(GetLocalizedKey(key));
//            return redisValue.HasValue ? _webHelper.Deserialize<T>(redisValue) : defaultValue;
//        }

//        public T Get<T>(string key, Func<T> acquire, int cacheMinute = 1440, bool skipCache = false)
//        {
//            if (!skipCache && IsSet(key))
//            {
//                return Get<T>(key);
//            }

//            var result = acquire();
//            Add(key, result, cacheMinute);
//            return result;
//        }

//        public bool Remove(string key)
//        {
//            if (IsSet(key))
//            {
//                Database.KeyDelete(GetLocalizedKey(key));
//                return true;
//            }
//            return false;
//        }

//        public void Clear(string parentKey)
//        {
//            Database.KeyDeleteWithPrefix(GetLocalizedKey($"{parentKey}*"));
//        }

//        public void ClearAll()
//        {
//            Database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
//        }


//        public IEnumerable<string> GetAllKeys()
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            if (_connectionMultiplexer != null)
//                _connectionMultiplexer.Dispose();
//        }

//        public bool IsSet(string key)
//        {
//            return Database.KeyExists(key);
//        }

//        #endregion

//        #region Osman

//        private string GetLocalizedKey(string key)
//        {
//            return string.Format("{0}:cache:{1}", Config.RedisCacheName, key);
//        }

//        private IDatabase Database
//        {
//            get
//            {
//                return GetConnection().GetDatabase();
//            }
//        }

//        public ConnectionMultiplexer GetConnection()
//        {
//            if (string.IsNullOrEmpty(Config.RedisCacheServer))
//            {
//                throw new ConfigurationErrorsException("A connection string is expected for Redis");
//            }

//            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
//            {
//                _connectionMultiplexer = ConnectionMultiplexer.Connect(Config.RedisCacheServer);
//                _connectionMultiplexer.ErrorMessage += RedisErrorMessage;
//            }
//            return _connectionMultiplexer;
//        }

//        private void RedisErrorMessage(object sender, RedisErrorEventArgs e)
//        {
//            _logger.Error("{0} : {1}", e.EndPoint, e.Message);
//        }
//        #endregion
//    }
//}