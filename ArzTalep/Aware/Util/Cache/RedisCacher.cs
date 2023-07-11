using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Aware.Util;
using Aware.Util.Exceptions;
using Aware.Util.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace Aware.Util.Cache
{
    public class RedisCacher : IAwareCacher
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IConfiguration _configuration;
        private IAwareLogger _logger;

        public RedisCacher(IConfiguration configuration, IAwareLogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool Add(string key, object value, int cacheDuration = 1440)
        {

            if (value != null)
            {
                string jsonString = JsonConvert.SerializeObject(value);
                RedisDatabase.StringSetAsync(key, jsonString, TimeSpan.FromMinutes(cacheDuration));
            }
            return false;
        }

        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            var redisValue = RedisDatabase.StringGet(key);
            return redisValue.HasValue ? JsonConvert.DeserializeObject<T>(redisValue) : defaultValue;
        }

        public T Get<T>(string key, Func<T> acquire, int cacheMinute = 1440, bool skipCache = false)
        {
            if (!skipCache && HasKey(key))
            {
                return Get<T>(key);
            }

            var result = acquire();
            Add(key, result, cacheMinute);
            return result;
        }

        public bool Remove(string key)
        {
            if (HasKey(key))
            {
                RedisDatabase.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }

        public bool HasKey(string key)
        {
            return RedisDatabase.KeyExists(key);
        }

        public IEnumerable<string> GetAllKeys()
        {
            var redisServer = _configuration.GetValue("RedisServer");
            return GetConnection().GetServer(redisServer).Keys().Select(i => i.ToString());
        }

        public void Dispose()
        {
            if (_connectionMultiplexer != null)
                _connectionMultiplexer.Dispose();
        }

        private IDatabase RedisDatabase
        {
            get
            {
                return GetConnection().GetDatabase();
            }
        }

        public ConnectionMultiplexer GetConnection()
        {


            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
            {
                var redisServer = _configuration.GetValue("RedisServer");
                if (!redisServer.Valid())
                {
                    throw new AwareException("A connection string is expected for Redis");
                }

                _connectionMultiplexer = ConnectionMultiplexer.Connect(redisServer);
                _connectionMultiplexer.ErrorMessage += RedisErrorMessage;
            }
            return _connectionMultiplexer;
        }

        private void RedisErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger.Error("RedisCacher|Error", "Endpoint:{0}, Message:{1}", null, e.EndPoint, e.Message);
        }
    }
}
