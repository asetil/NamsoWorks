using Aware.Data.Fake;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace CleanFramework.Business.Dependency
{
    public class CleanCodeInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IFakeDataProvider>().ImplementedBy<Data.Repository.FakeDataProvider>().LifestyleSingleton());
            container.Register(Classes.FromThisAssembly().Where(i => i.Name.EndsWith("Service")).WithService.DefaultInterfaces());
        }
    }
}
