namespace Aware.Util.Enum
{
    public enum StatusType
    {
        None = -1,
        Passive = 0,
        Active = 1,
        Deleted = 2,
        WaitingActivation = 3
    }

    public enum ManagerCacheMode
    {
        NoCache = 0,
        UseCache = 1,
        UseResponsiveCache = 2 //After Save + Delete ops cache updated!
    }

    public enum JoinMode
    {
        None = -666,
        InnerJoin = 0,
        LeftOuterJoin = 1,
        RightOuterJoin = 2,
        FullJoin = 4
    }

    public enum CacheMode
    {
        Empty = 0,
        Partial = 1,
        MemoryCacher = 2,
        HttpContextCacher = 3,
        RedisCacher = 4
    }

    public enum ORMType
    {
        Fake = 0,
        EntityFramework = 1,
        Nhibernate = 2,
        SQLConnection = 3
    }

    public enum DatabaseType
    {
        None = 0,
        MsSQL = 1,
        MySQL = 2,
        SQLLite = 3,
        Fake = 4,
        PLSQL = 5,
        Oracle = 6
    }

    public enum GenderType
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public enum CustomerRole
    {
        Public = 0,
        SuperUser = 1,
        Customer = 2, //firm or user
        Company = 3,
        User = 4
    }

    public enum AuthorityType
    {
        None = 0,
    }

    public enum ResourceScope
    {
        None = 0,
        Script = 1
    }

    public enum ActivationResultType
    {
        None = 0,
        WaitingActivation = 1,
        ActivationSuccessfull = 2,
        ActivationSend = 3
    }

    public enum FileType
    {
        None = 0,
        File = 1,
        Image = 2
    }

    public enum ViewTypes
    {
        None = 0,
        Editable = 1,
        Read = 2,
        UnAuthorized = 3
    }
}
