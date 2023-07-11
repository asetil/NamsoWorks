namespace Aware.Dependency
{
    public enum CacheMode
    {
        Empty = 0,
        Partial = 1,
        MemoryCacher = 2,
        HttpContextCacher = 3,
    }

    public enum ORMType
    {
        Fake = 0,
        Nhibernate = 1,
        EntityFramework = 2,
        SQLConnection = 3
    }

    public enum DatabaseType
    {
        None=0,
        MsSQL = 1,
        MySQL = 2,
        SQLLite = 3,
        Fake = 4,
    }
}
