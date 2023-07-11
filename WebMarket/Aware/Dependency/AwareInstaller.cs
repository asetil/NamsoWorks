using System.Reflection;
using Aware.Authenticate;
using Aware.Cache;
using Aware.Data;
using Aware.Data.Fake;
using Aware.Data.NHibernate.Configure;
using Aware.ECommerce;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Service;
using Aware.File;
using Aware.Mail;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Aware.Regional;
using Aware.Search.ElasticSearch;
using Aware.Search.ElasticSearch.Data;
using Aware.Task.Web;
using Aware.Util.Log;
using Aware.Util;

namespace Aware.Dependency
{
    public class AwareInstaller : IWindsorInstaller
    {
        private readonly DependencySetting _dependencySetting;

        public AwareInstaller(DependencySetting dependencySetting)
        {
            _dependencySetting = dependencySetting;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            BindConfiguration(container);
            container.Register(Component.For<IWindsorContainer>().Instance(container));
            container.Register(Component.For<ILogger>().ImplementedBy<NLogger>().LifestyleSingleton());

            if (_dependencySetting.OrmType == ORMType.Nhibernate)
            {
                var configurationProvider = new ConfigurationProvider().GetFluentConfiguration(_dependencySetting);
                container.Register(Component.For<ISessionFactory>().Instance(configurationProvider.BuildSessionFactory()));

                container.Register(Component.For<IUnitOfWork>().ImplementedBy<Data.NHibernate.UnitOfWork>().LifeStyle.HybridPerWebRequestPerThread());
                container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Data.NHibernate.NHibernateRepository<>)));
            }
            else if (_dependencySetting.OrmType == ORMType.Fake)
            {
                container.Register(Component.For<IFakeDataProvider>().ImplementedBy<FakeDataProvider>().LifestyleSingleton());
                container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(FakeRepository<>)));
            }
            else if (_dependencySetting.OrmType == ORMType.EntityFramework)
            {
                container.Register(Component.For<IUnitOfWork>().ImplementedBy<Data.EF.UnitOfWork>().LifeStyle.HybridPerWebRequestPerThread());
                container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Data.EF.EFRepository<>)));
            }
            else if (_dependencySetting.OrmType == ORMType.SQLConnection)
            {
                container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Data.SQL.SqlRepository<>)));
            }

            if (_dependencySetting.CacheMode == CacheMode.MemoryCacher)
            {
                container.Register(Component.For<ICacher>().ImplementedBy<MemoryCacher>().LifestyleSingleton());
            }
            else if (_dependencySetting.CacheMode == CacheMode.Empty)
            {
                container.Register(Component.For<ICacher>().ImplementedBy<EmptyCacher>().LifestyleSingleton());
            }
            else if (_dependencySetting.CacheMode == CacheMode.Partial)
            {
                container.Register(Component.For<ICacher>().ImplementedBy<PartialCacher>().LifestyleSingleton());
            }
            else if (_dependencySetting.CacheMode == CacheMode.HttpContextCacher)
            {
                container.Register(Component.For<ICacher>().ImplementedBy<HttpContextCacher>().LifestyleSingleton());
            }

            container.Register(Component.For<IWebHelper>().ImplementedBy<WebHelper>().LifestyleSingleton());
            container.Register(Component.For<IApplication>().ImplementedBy<Application>().LifestyleSingleton());
            container.Register(Component.For<IElasticRepository>().ImplementedBy<ElasticRepository>().LifestyleSingleton());
            container.Register(Component.For<IElasticService>().ImplementedBy<ElasticService>().LifestyleSingleton());
            container.Register(Component.For<IMailManager>().ImplementedBy<MailManager>().LifestyleSingleton());

            container.Register(Component.For<IAddressService>().ImplementedBy<AddressService>().LifestyleSingleton());
            container.Register(Component.For<IImageResizer>().ImplementedBy<ImageResizer>().LifestyleTransient());
            container.Register(Component.For<IFileService>().ImplementedBy<FileService>().LifestyleSingleton());
            container.Register(Component.For<IStoreService>().ImplementedBy<StoreService>().LifestyleSingleton());
            container.Register(Component.For<IMailService>().ImplementedBy<MailService>().LifestyleSingleton());
            container.Register(Component.For<ICategoryService>().ImplementedBy<CategoryService>().LifestyleSingleton());
            container.Register(Component.For<IUserService>().ImplementedBy<UserService>().LifestyleSingleton());
            container.Register(Component.For<ICampaignService>().ImplementedBy<CampaignService>().LifestyleSingleton());

            container.Register(Classes.FromThisAssembly().Where(i => i.Name.EndsWith("Manager")).WithService.DefaultInterfaces());
            container.Register(Classes.FromThisAssembly().Where(i => i.Name.EndsWith("Service")).WithService.DefaultInterfaces());

            container.Register(Classes.FromThisAssembly().BasedOn<ITask>());
        }

        private void BindConfiguration(IWindsorContainer container)
        {
            //container.Register(
            //  Component.For<IIndexConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IEvaluationTypeConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IMailConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IDatabaseConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IEuroMessageConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IPayuConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IUserProccessorConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IVConnectConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IMatchConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IDomainConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<ISocialApiKeyConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IIndexerConfiguration<ElAd>>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<ISmartBoxConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<ISmsConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IAdTemplateConfiguration>().UsingFactoryMethod(GetCurrentConfiguration),
            //  Component.For<IMobileNotificationPushConfiguration>().UsingFactoryMethod(GetCurrentConfiguration)
            //  );
        }
    }
}
