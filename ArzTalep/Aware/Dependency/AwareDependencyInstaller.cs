using Aware.Data;
using Aware.Data.EF;
using Aware.Data.Fake;
using Aware.File;
using Aware.Mail;
using Aware.Manager;
using Aware.Util.Cache;
using Aware.Util.Enum;
using Aware.Util.Log;
using Aware.Util.Lookup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aware.Dependency
{
    public class AwareDependencyInstaller
    {
        public virtual void Install(ref IServiceCollection services, DependencySetting dependencySetting)
        {
            services.AddSingleton<IAwareLogger, AwareLogger>();
            AddCacher(ref services, dependencySetting.CacheMode);
            //services.AddSingleton<IUnitOfWork, UnitOfWork>();

            ConfigureRepository(ref services, dependencySetting.OrmType);

            InstallManagers(ref services, dependencySetting.IsLite);
        }

        public virtual void InstallWithEF<TContext>(ref IServiceCollection services, DependencySetting dependencySetting) where TContext : DbContext
        {
            services.AddSingleton<IAwareLogger, AwareLogger>();
            AddCacher(ref services, dependencySetting.CacheMode);

            services.AddSingleton(typeof(IDbContextFactory), typeof(DbContextFactory<TContext>));
            services.AddSingleton(typeof(IRepository<>), typeof(EFRepository<>));
            //services.AddScoped(typeof(Data.IRepository<>), typeof(EFRepository<>));

            InstallManagers(ref services, dependencySetting.IsLite);
        }

        private void InstallManagers(ref IServiceCollection services, bool isLite)
        {
            services.AddSingleton<IEncryptManager, EncryptManager>();

            if (isLite)
            {
                services.AddSingleton<ILookupManager, LookupManager>();
            }
            else
            {
                services.AddSingleton<IApplicationManager, ApplicationManager>();
                services.AddSingleton<ILookupManager, LookupManager>();
                services.AddSingleton<IMailManager, MailManager>();
                services.AddSingleton<IFileManager, FileManager>();
            }

            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddSingleton<IUserManager, UserManager>();
        }

        private void AddCacher(ref IServiceCollection services, CacheMode cacheMode)
        {
            switch (cacheMode)
            {
                case Util.Enum.CacheMode.Partial:
                    //services.AddSingleton<IAwareCacher, PartialCacher>();
                    break;
                case Util.Enum.CacheMode.MemoryCacher:
                    //services.AddSingleton<IAwareCacher, PartialCacher>();
                    break;
                case Util.Enum.CacheMode.HttpContextCacher:
                    //services.AddSingleton<IAwareCacher, PartialCacher>();
                    break;
                case Util.Enum.CacheMode.RedisCacher:
                    services.AddSingleton<IAwareCacher, RedisCacher>();
                    break;
                default:
                    services.AddSingleton<IAwareCacher, EmptyCacher>();
                    break;
            }
        }

        private void ConfigureRepository(ref IServiceCollection services, ORMType ormType)
        {
            switch (ormType)
            {
                case ORMType.Fake:
                    services.AddSingleton(typeof(Data.IRepository<>), typeof(FakeRepository<>));
                    break;
                case ORMType.EntityFramework:
                    throw new Exception("Use InstallWithEF() method instead of Install() method!");
                case ORMType.Nhibernate:
                    //var configurationProvider = new Data.NHibernate.Configure.ConfigurationProvider(new WorchartLogger()).GetFluentConfiguration(dependencySetting);
                    //container.Register(Component.For<ISessionFactory>().Instance());

                    //services.AddSingleton<ISessionFactory>(t => configurationProvider.BuildSessionFactory());
                    //services.AddSingleton<Data.IRepository<>, NHibernateRepository<>>();
                    break;
                case ORMType.SQLConnection:
                    break;
            }
        }
    }
}