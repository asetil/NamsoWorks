namespace Worchart.BL.Enum
{
    public enum StatusType
    {
        None = -1,
        Passive = 0,
        Active = 1,
        Deleted = 2
    }

    public enum ProjectRequirementStatus
    {
        None = 0,
        Created = 1,
        Analysing = 2,
        Developing = 3,
        Testing = 4,
        Completed = 5,
        Cancelled = 6,
        Deleted = 7,
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
    }

    public enum ORMType
    {
        Fake = 0,
        Nhibernate = 1,
        SQLConnection = 2
    }

    public enum DatabaseType
    {
        None = 0,
        MsSQL = 1,
        MySQL = 2,
        SQLLite = 3,
        Fake = 4,
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
}
