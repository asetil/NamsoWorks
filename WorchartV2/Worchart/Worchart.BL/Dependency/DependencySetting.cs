using System.Reflection;
using Worchart.BL.Enum;

namespace Worchart.BL.Dependency
{
    public class DependencySetting
    {
        public CacheMode CacheMode { get; set; }

        public bool UseIntercepter { get; set; }

        public Assembly Assembly { get; set; }

        public string ConnectionString { get; set; }

        public DatabaseType DbType { get; set; }

        public ORMType OrmType { get; set; }

    }
}