using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace Aware.Dependency
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException();

            _container = container;
        }

        public override void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            _container.Kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                if (!typeof (IController).IsAssignableFrom(controllerType))
                {
                    throw new ArgumentException();
                }

                if (_container.Kernel.HasComponent(controllerType))
                {
                    IController controller = _container.Kernel.Resolve(controllerType) as IController;
                    if (controller != null)
                    {
                        return controller;
                    }
                }
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerComponentName = controllerName + "Controller";

            try
            {
                var controller = _container.Kernel.Resolve<IController>(controllerComponentName);
                if (controller != null)
                {
                    return controller;
                }
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                //make sure that windsor doesn't blow up... return to default factory if it does.
            }

            return base.CreateController(requestContext, controllerName);
        }
    }
}