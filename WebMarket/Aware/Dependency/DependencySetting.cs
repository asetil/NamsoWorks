using System.Reflection;

namespace Aware.Dependency
{
    public class DependencySetting
    {
        public CacheMode CacheMode { get; private set; }
        public ORMType OrmType { get; private set; }
        public bool UseIntercepter { get; private set; }
        public Assembly Assembly { get; private set; }

        public DependencySetting SetCache(CacheMode cacheMode)
        {
            CacheMode = cacheMode;
            return this;
        }

        public DependencySetting SetORM(ORMType ormType)
        {
            OrmType = ormType;
            return this;
        }

        public DependencySetting SetIntercepter(bool use = true)
        {
            UseIntercepter = use;
            return this;
        }

        public DependencySetting SetAssembly(Assembly assembly)
        {
            Assembly = assembly;
            return this;
        }
    }
}