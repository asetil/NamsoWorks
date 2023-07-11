using System.Reflection;
using System.Web;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Aware.Dependency
{
    public class HybridScopeAccessor : IScopeAccessor
    {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridScopeAccessor(IScopeAccessor secondaryScopeAccessor)
        {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context)
        {
            if (HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized)
                return webRequestScopeAccessor.GetScope(context);
            return secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose()
        {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }
    }

    public class TransientScopeAccessor : IScopeAccessor
    {
        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context)
        {
            return new DefaultLifetimeScope();
        }

        public void Dispose()
        {
        }
    }

    public class HybridPerWebRequestPerThreadScopeAccessor : HybridScopeAccessor
    {
        public HybridPerWebRequestPerThreadScopeAccessor() :
            base(new ThreadScopeAccessor()) { }
    }

    public class HybridPerWebRequestTransientScopeAccessor : HybridScopeAccessor
    {
        public HybridPerWebRequestTransientScopeAccessor() :
            base(new TransientScopeAccessor()) { }
    }

    public class PerWebRequestLifestyleModuleUtils
    {
        // TODO make this public in Windsor
        private static readonly FieldInfo __initializedFieldInfo = typeof(PerWebRequestLifestyleModule).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

        public static bool IsInitialized
        {
            get
            {
                return (bool)__initializedFieldInfo.GetValue(null);
            }
        }
    }
}