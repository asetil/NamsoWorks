using System.Reflection;
using Aware.Dependency;
using CleanFramework.Business.Dependency;

namespace WebMarket.Infrastructure
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var dependencySetting = WindsorBootstrapper.Dependency()
                .SetCache(CacheMode.MemoryCacher)
                //.SetIntercepter()
                .SetORM(ORMType.Nhibernate)
                .SetAssembly(Assembly.GetAssembly(typeof(CleanFramework.Business.Model.Entry)));

            WindsorBootstrapper.Create(null);
            WindsorBootstrapper.WithInstaller(new CleanCodeInstaller());
            WindsorBootstrapper.WithInstaller(new AwareInstaller(dependencySetting));
            WindsorBootstrapper.WithInstaller(new WebsiteInstaller());
        }

        public static void Stop()
        {
            WindsorBootstrapper.Dispose();
        }
    }
}