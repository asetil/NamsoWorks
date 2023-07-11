using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using Worchart.BL.Common;
using Worchart.BL.Dependency;
using Worchart.BL.Enum;
using Worchart.BL.Log;

namespace Worchart.Data.NHibernate.Configure
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly ILogger _logger;

        public ConfigurationProvider(ILogger logger)
        {
            _logger = logger;
        }

        public Configuration GetConfiguration(Assembly assembly, DependencySetting settings)
        {
            Func<IPersistenceConfigurer> dbConfig = MsSqlConfiguration.MsSql2012.ConnectionString(settings.ConnectionString).ShowSql().Dialect<MsSql2012Dialect>;
            Action<MappingConfiguration> mappingConfig = config =>
            {
                config.HbmMappings.AddFromAssemblyOf<ConfigurationProvider>();
                config.FluentMappings.AddFromAssemblyOf<ConfigurationProvider>();

                if (assembly != null)
                {
                    config.HbmMappings.AddFromAssembly(assembly);
                    config.FluentMappings.AddFromAssembly(assembly);
                }
            };

            return Fluently.Configure().Database(dbConfig).Mappings(mappingConfig).BuildConfiguration();
        }

        public FluentConfiguration GetFluentConfiguration(DependencySetting dependency)
        {
            Func<IPersistenceConfigurer> dbConfig = null;
            SiteSettings.DatabaseType = dependency.DbType;

            switch (dependency.DbType)
            {
                case DatabaseType.MsSQL:
                    dbConfig = MsSqlConfiguration.MsSql2012.ConnectionString(dependency.ConnectionString).ShowSql().Dialect<MsSql2012Dialect>;break;
                case DatabaseType.MySQL:
                    dbConfig = MySQLConfiguration.Standard.ConnectionString(dependency.ConnectionString).ShowSql().Dialect<MySQL55Dialect>; break;
                case DatabaseType.SQLLite:
                    dbConfig = SQLiteConfiguration.Standard.ConnectionString(dependency.ConnectionString).ShowSql().Dialect<SQLiteDialect>; break;
            }
            
            if (dbConfig == null) { throw new Exception("Database configuration failed! Check database type."); }

            Action<MappingConfiguration> mappingConfig = config =>
            {
                config.HbmMappings.AddFromAssemblyOf<ConfigurationProvider>();
                config.FluentMappings.AddFromAssemblyOf<ConfigurationProvider>();

                if (dependency.Assembly != null)
                {
                    config.HbmMappings.AddFromAssembly(dependency.Assembly);
                    config.FluentMappings.AddFromAssembly(dependency.Assembly);
                }
            };

            var fluentConfiguration = Fluently.Configure().Database(dbConfig).Mappings(mappingConfig);
            if (dependency.UseIntercepter)
            {
                IInterceptor interceptor = new NhSqlInterceptor(_logger);
                fluentConfiguration.BuildConfiguration().SetInterceptor(interceptor);
            }
            return fluentConfiguration;
        }
    }
}
