using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Worchart.BL.Cache;
using Worchart.BL.Company;
using Worchart.BL.Encryption;
using Worchart.BL.Log;
using Worchart.BL.Lookup;
using Worchart.BL.Manager;
using Worchart.BL.Project;
using Worchart.BL.Token;
using Worchart.BL.User;
using Worchart.Data;
using Worchart.Data.NHibernate;

namespace Worchart.BL.Dependency
{
    public class LibraryInstaller
    {
        public void Install(ref IServiceCollection services, DependencySetting dependencySetting)
        {
            services.AddSingleton<BL.Log.ILogger, BL.Log.WorchartLogger>();
            services.AddSingleton<ICacher, RedisCacher>();

            var configurationProvider = new Data.NHibernate.Configure.ConfigurationProvider(new WorchartLogger()).GetFluentConfiguration(dependencySetting);
            //container.Register(Component.For<ISessionFactory>().Instance());

            services.AddSingleton<ISessionFactory>(t => configurationProvider.BuildSessionFactory());
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IEncryptManager, EncryptManager>();

            //services.AddSingleton<Data.IRepository<>, NHibernateRepository<>>();
            services.AddSingleton(typeof(Data.IRepository<>), typeof(NHibernateRepository<>));
            services.AddSingleton<IApplicationManager, ApplicationManager>();
            services.AddSingleton<ILookupManager, LookupManager>();
            services.AddSingleton<ITokenManager, TokenManager>();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<IProjectManager, ProjectManager>();
            services.AddSingleton<ITeamManager, TeamManager>();
            services.AddSingleton<ITeamUserManager, TeamUserManager>();
            services.AddSingleton<ICompanyManager, CompanyManager>();
            services.AddSingleton<ICommonManager, CommonManager>();
            services.AddSingleton<IResourceManager, ResourceManager>();
        }
    }
}
