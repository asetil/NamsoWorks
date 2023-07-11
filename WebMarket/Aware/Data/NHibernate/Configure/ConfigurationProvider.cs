using System;
using System.Reflection;
using Aware.Dependency;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using Aware.ECommerce.Util;

namespace Aware.Data.NHibernate.Configure
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public Configuration GetConfiguration(Assembly assembly)
        {
            Func<IPersistenceConfigurer> dbConfig = MsSqlConfiguration.MsSql2012
                .ConnectionString(builder => builder.FromConnectionStringWithKey(Constants.ConnectionString))
                .ShowSql()
                .Dialect<MsSql2012Dialect>;

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
            var dbType = Aware.Util.Config.DatabaseType;
            Func<IPersistenceConfigurer> dbConfig = null;

            switch (dbType)
            {
                case DatabaseType.MsSQL:
                    dbConfig = MsSqlConfiguration.MsSql2012
                        .ConnectionString(builder => builder.FromConnectionStringWithKey(Constants.ConnectionString))
                        .ShowSql().Dialect<MsSql2012Dialect>; break;
                case DatabaseType.MySQL:
                    dbConfig = MySQLConfiguration.Standard
                        .ConnectionString(builder => builder.FromConnectionStringWithKey(Constants.ConnectionString))
                        .ShowSql().Dialect<MySQL55Dialect>; break;
                case DatabaseType.SQLLite:
                    dbConfig = SQLiteConfiguration.Standard
                        .ConnectionString(builder => builder.FromConnectionStringWithKey(Constants.ConnectionString))
                        .ShowSql().Dialect<SQLiteDialect>; break;
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
            if (dependency.UseIntercepter && Aware.Util.Config.IsDebugMode)
            {
                IInterceptor interceptor = new NhSqlInterceptor();
                fluentConfiguration.BuildConfiguration().SetInterceptor(interceptor);
            }
            return fluentConfiguration;
        }
    }
}
