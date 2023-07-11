using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace Aware.Dependency
{
    public class WindsorDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        public readonly IKernel Kernel;

        public WindsorDependencyResolver(IKernel kernel)
        {
            Kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return Kernel.HasComponent(serviceType) ? Kernel.Resolve(serviceType) : null;
        }

        public TService GetService<TService>()
        {
            return (TService) GetService(typeof (TService));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.HasComponent(serviceType) ? Kernel.ResolveAll(serviceType).Cast<object>() : new object[] {};
        }

        public IEnumerable<TService> GetServices<TService>()
        {
            return (IEnumerable<TService>) GetServices(typeof (TService));
        }

        public object GetService(string typeName)
        {
            var type = Type.GetType(typeName);
            return !string.IsNullOrEmpty(typeName) && Kernel.HasComponent(typeName) ? Kernel.Resolve(type) : null;
        }
    }
}