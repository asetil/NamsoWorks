using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace WebMarket.Admin.Infrastructure
{
    public class WebsiteInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient());

            //container.Register(Classes.FromThisAssembly().Where(item => item != typeof(IController) && item != typeof(SelectedCriteriaContainer)).WithService.DefaultInterfaces());
        }
    }
}