using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Reflection;

namespace Aware.Dependency
{
    public static class Extensions
    {
        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, transient.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestTransient<S>(this LifestyleGroup<S> @group) where S : class
        {
            return @group.Scoped<HybridPerWebRequestTransientScopeAccessor>();
        }

        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, one per thread.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestPerThread<S>(this LifestyleGroup<S> @group) where S : class
        {
            return @group.Scoped<HybridPerWebRequestPerThreadScopeAccessor>();
        }
    }
}
