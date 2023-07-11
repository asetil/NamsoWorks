using Aware.Dependency;

namespace WebMarket.Infrastructure
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var dependencySetting = WindsorBootstrapper.Dependency()
                .SetCache(CacheMode.MemoryCacher)
                .SetORM(ORMType.Nhibernate)
                .SetIntercepter();

            WindsorBootstrapper.Create(null);
            WindsorBootstrapper.WithInstaller(new AwareInstaller(dependencySetting));
            WindsorBootstrapper.WithInstaller(new WebsiteInstaller());
        }

        public static void Stop()
        {
            WindsorBootstrapper.Dispose();
        }
    }
}