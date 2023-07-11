//using System;
//using System.Linq;
//using System.Configuration;
//using Worchart.BL.Enum;

//namespace Worchart.BL
//{
//    public static class Config
//    {
//        static Config()
//        {
//            Initialize();
//        }

//        public static bool IsAdmin { get; private set; }
//        public static bool RequireSsl { get; private set; }
//        public static bool CacheEnabled { get; private set; }
//        public static bool IsDebugMode { get; private set; }
//        public static string DomainUrl { get; private set; }
//        public static string SiteUrl { get; private set; }
//        public static LogMode LogMode { get; private set; }
//        public static DatabaseType DatabaseType { get; private set; }
//        public static string ImageRepository { get; private set; }
//        public static string CssTheme { get; private set; }
//        public static string RedisCacheServer { get; private set; }
//        public static string RedisCacheName { get; private set; }

//        public static string ConnectionString
//        {
//            get
//            {
//                var result = ConfigurationManager.ConnectionStrings[0].ConnectionString;
//                return result;
//            }
//        }

//        private static void Initialize()
//        {
//            DomainUrl = Value("DomainUrl");
//            SiteUrl = Value("SiteUrl");
//            IsAdmin = BoolValue("IsAdmin");
//            RequireSsl = BoolValue("RequireSSL");
//            CacheEnabled = BoolValue("CacheEnabled");
//            IsDebugMode = BoolValue("DebugMode"); //old way : HttpContext.Current!=null && HttpContext.Current.IsDebuggingEnabled;
//            LogMode = (LogMode)IntValue("LogMode", (int)LogMode.Disabled);
//            ImageRepository = Value("ImageRepository");
//            CssTheme = Value("theme");
//            RedisCacheServer = Value("RedisCacheServer");
//            RedisCacheName = Value("RedisCacheName");

//            var dbType = Value("DatabaseType");
//            if (!string.IsNullOrEmpty(dbType))
//            {
//                switch (dbType.ToLowerInvariant())
//                {
//                    case "mssql":
//                        DatabaseType = Dependency.DatabaseType.MsSQL;
//                        break;
//                    case "mysql":
//                        DatabaseType = Dependency.DatabaseType.MySQL;
//                        break;
//                    case "sqllite":
//                        DatabaseType = Dependency.DatabaseType.SQLLite;
//                        break;
//                    case "fake":
//                        DatabaseType = Dependency.DatabaseType.Fake;
//                        break;
//                }
//            }

//            if (DatabaseType == Dependency.DatabaseType.None)
//            {
//                throw new Exception("DatabaseType configuration not found!");
//            }
//        }

//        public static string Value(string key,string defaultValue="")
//        {
//            if (ConfigurationManager.AppSettings[key] != null)
//            {
//                return ConfigurationManager.AppSettings[key];
//            }
//            return defaultValue;
//        }

//        private static bool BoolValue(string key)
//        {
//            var value = Value(key);
//            return !string.IsNullOrEmpty(value) && value.ToLowerInvariant() == "true";
//        }

//        private static int IntValue(string key, int defaultValue = 0)
//        {
//            var value = Value(key);
//            return !string.IsNullOrEmpty(value) ? value.Int() : defaultValue;
//        }
//    }
//}