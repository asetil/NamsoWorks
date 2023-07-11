using System.Reflection;
using Aware.Dependency;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;

namespace Aware.Data.NHibernate.Configure
{
    public interface IConfigurationProvider
    {
        Configuration GetConfiguration(Assembly assembly);
        FluentConfiguration GetFluentConfiguration(DependencySetting dependencySetting);
    }
}