using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

namespace Aware.Dependency
{
    public static class WindsorBootstrapper
    {
        public static WindsorDependencyResolver Resolver;
        private static IWindsorContainer _container;

        public static void Create(IEnumerable<string> assemblyNameList)
        {
            SetContainer(assemblyNameList);
            DependencyResolver.SetResolver(new WindsorDependencyResolver(_container.Kernel));
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(_container));
        }

        public static void CreateForWcf(IEnumerable<string> assemblyNameList)
        {
            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel));

            if (assemblyNameList != null)
            {
                foreach (var assemblyName in assemblyNameList)
                {
                    if (!string.IsNullOrEmpty(assemblyName)) Install(assemblyName);
                }
            }

            Resolver = new WindsorDependencyResolver(_container.Kernel);
        }

        public static DependencySetting Dependency()
        {
            return new DependencySetting();
        }

        public static void WithInstaller(IWindsorInstaller installer)
        {
            if (installer != null)
            {
                _container.Install(installer);
            }
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static void InstallFor(IWindsorInstaller installer)
        {
            if (installer != null)
            {
                _container.Install(installer);
            }
        }

        public static IKernel Kernel
        {
            get { return _container.Kernel; }
        }

        public static void Dispose()
        {
            _container.Dispose();
        }

        private static void SetContainer(IEnumerable<string> assemblyNameList)
        {
            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel));

            if (assemblyNameList != null)
            {
                foreach (var assemblyName in assemblyNameList)
                {
                    if (!string.IsNullOrEmpty(assemblyName)) Install(assemblyName);
                }
            }

            Resolver = new WindsorDependencyResolver(_container.Kernel);
        }

        private static void Install(string assemblyName)
        {
            var assembly = GetAssemblyByName(assemblyName);
            if (assembly != null)
            {
                var installerType = typeof(IWindsorInstaller);
                var installlerTypes = assembly.GetTypes().Where(x => x.IsClass && installerType.IsAssignableFrom(x));

                foreach (var type in installlerTypes)
                {
                    var windsorInstaller = Activator.CreateInstance(type) as IWindsorInstaller;
                    _container.Install(windsorInstaller);
                }
            }
            else
            {
                throw new Exception(string.Format(@"cannot find assembly {0}\{1}.dll",
                    AppDomain.CurrentDomain.RelativeSearchPath, assemblyName));
            }
        }

        private static Assembly GetAssemblyByName(string assemblyName)
        {
            AppDomain.CurrentDomain.Load(assemblyName);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
        }
    }
}
