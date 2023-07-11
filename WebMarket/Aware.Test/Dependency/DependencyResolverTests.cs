using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Aware.Dependency;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.Windsor.Diagnostics;
using NUnit.Framework;
using FluentAssertions;

namespace Aware.Test.Dependency
{
    [TestFixture]
    public class DependencyResolvingTests
    {
        [SetUp]
        public void Init()
        {
            WindsorBootstrapper.Create(null);
            WindsorBootstrapper.Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;

            var dependencySetting = WindsorBootstrapper.Dependency()
              .SetCache(CacheMode.Empty)
              .SetORM(ORMType.Fake).SetIntercepter();

            WindsorBootstrapper.WithInstaller(new AwareInstaller(dependencySetting));
        }

        void Kernel_ComponentModelCreated(ComponentModel model)
        {
            if (model.LifestyleType == LifestyleType.PerWebRequest)
                model.LifestyleType = LifestyleType.Transient;
        }

        [TearDown]
        public void Cleanup()
        {
            WindsorBootstrapper.Dispose();
        }

        //[Test]
        //public void ResolveAllControllersTest()
        //{
        //    IEnumerable<Type> controllerTypes = typeof(HomeController).Assembly.GetTypes().Where(item => typeof(IController).IsAssignableFrom(item) && !item.IsAbstract);

        //    foreach (Type controllerType in controllerTypes)
        //    {
        //        DependencyResolver.Current.GetService(controllerType);
        //    }
        //}

        [Test]
        public void ShouldNotHaveAnyMisconfiguredComponents()
        {
            var diagnostic = new PotentiallyMisconfiguredComponentsDiagnostic(WindsorBootstrapper.Kernel);
            List<IHandler> handlers = diagnostic.Inspect().Where(NotContains).Select(x => x).ToList();
            if (handlers.Any())
            {
                var builder = new StringBuilder();
                builder.AppendFormat("Misconfigured components ({0})\r\n", handlers.Count);
                foreach (IHandler handler in handlers)
                {
                    var info = (IExposeDependencyInfo)handler;
                    var inspector = new DependencyInspector(builder);
                    info.ObtainDependencyDetails(inspector);
                }
                Assert.Fail(builder.ToString());
            }
        }

        [Test]
        public void ShouldHaveAllServicesProperlyRegisterred()
        {
            IEnumerable<IHandler> handlers = new AllServicesDiagnostic(WindsorBootstrapper.Kernel).Inspect()
                .SelectMany(x => x).Where(NotContains).Select(x => x).ToList();
            foreach (var handler in handlers)
            {
                handler.CurrentState.Should().Be(HandlerState.Valid, handler.ComponentModel.Name);
            }
        }

        [Test]
        public void ShouldHaveAllComponentsProperlyRegisterred()
        {
            List<IHandler> handlers = new AllComponentsDiagnostic(WindsorBootstrapper.Kernel).Inspect().Where(NotContains).Select(x => x).ToList();

            foreach (var handler in handlers)
            {
                handler.CurrentState.Should().Be(HandlerState.Valid, handler.ComponentModel.Name);
            }
        }

        [Test]
        public void ShouldHaveAllComponentsWithProperLifestyle()
        {
            var handlers = new PotentialLifestyleMismatchesDiagnostic(WindsorBootstrapper.Kernel).Inspect();
            handlers.Should().BeEmpty();
        }

        [Test]
        public void ShouldHaveAllComponentsCorrectlyConfigured()
        {
            var handlers = new PotentiallyMisconfiguredComponentsDiagnostic(WindsorBootstrapper.Kernel).Inspect().Where(NotContains).Select(x => x).ToList();
            handlers.Should().BeEmpty();
        }

        [Test]
        public void ShouldHaveAllNotDuplicatedDependenciesProperly()
        {
            var duplicatedDependenciesDiagnostic = new DuplicatedDependenciesDiagnostic(WindsorBootstrapper.Kernel);

            var handlers = duplicatedDependenciesDiagnostic.Inspect();
            List<KeyValuePair<string, string>> detailList = new List<KeyValuePair<string, string>>();

            foreach (Pair<IHandler, DependencyDuplicate[]> handler in handlers)
            {
                var handlerName = handler.First.ComponentModel.Name;
                if (handlerName != "Castle.Windsor.WindsorContainer")
                {
                    string details = handler.Second.Aggregate(string.Empty, (current, dependencyDuplicate) => current + duplicatedDependenciesDiagnostic.GetDetails(dependencyDuplicate));
                    detailList.Add(new KeyValuePair<string, string>(handlerName, details));
                }
            }

            detailList.Should().BeEmpty();
        }

        [Test]
        public void ResolveAllTest()
        {
            IHandler[] handlers = new AllComponentsDiagnostic(WindsorBootstrapper.Kernel).Inspect();

            var creationContext = CreationContext.CreateEmpty();

            var builder = new StringBuilder();

            foreach (var handler in handlers)
            {
                try
                {
                    if (!handler.ComponentModel.RequiresGenericArguments)
                    {
                        handler.Resolve(creationContext);
                    }
                }
                catch (Exception exception)
                {
                    builder.AppendFormat("Cannot Resolve components ({0}, {1})\r\n", handler.ComponentModel.ComponentName, exception.Message);
                }
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                Assert.Fail(builder.ToString());
            }
        }

        private static bool NotContains(IHandler x)
        {
            return !x.ComponentModel.ComponentName.ToString().Contains("AnonymousType");
        }
    }
}
