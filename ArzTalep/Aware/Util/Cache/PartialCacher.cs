//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Worchart.BL.Cache
//{
//    public class PartialCacher : MemoryCacher
//    {
//        private readonly List<string> _allowedKeys;

//        public PartialCacher()
//        {
//            _allowedKeys = GetAllowedKeys();
//        }

//        public override T Get<T>(string key)
//        {
//            if (IsKeyAllowed(key))
//            {
//                return base.Get<T>(key);
//            }
//            return default(T);
//        }

//        public override T Get<T>(string key, T defaultValue)
//        {
//            if (IsKeyAllowed(key))
//            {
//                return base.Get(key, defaultValue);
//            }
//            return default(T);
//        }

//        public override bool Add(string key, object value, int cacheTime = 1440)
//        {
//            if (IsKeyAllowed(key))
//            {
//                return base.Add(key, value, cacheTime);
//            }
//            return false;
//        }

//        public override bool Remove(string key)
//        {
//            if (IsKeyAllowed(key))
//            {
//                return base.Remove(key);
//            }
//            return false;
//        }

//        private bool IsKeyAllowed(string key)
//        {
//            return !string.IsNullOrEmpty(key) && _allowedKeys.Any(i => key.IndexOf(i, StringComparison.Ordinal) > -1);
//        }

//        private List<string> GetAllowedKeys()
//        {
//            var result = new List<string>()
//            {
//                //Constants.CK_Lookups
//            };
//            return result;
//        }
//    }
//}
