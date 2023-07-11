namespace Worchart.Data.NHibernate.Mapping
{
    //public class AddressMap : TableMapper<Address>
    //{
    //    public AddressMap() : base("Address")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Name);
    //        Map(x => x.Description);
    //        Map(x => x.CityID);
    //        Map(x => x.CountyID);
    //        Map(x => x.DistrictID);
    //        Map(x => x.Phone);
    //        Map(x => x.UserID);
    //        Map(x => x.Status).CustomType<Statuses>();
    //    }
    //}

    //public class UserMap : TableMapper<UserModel>
    //{
    //    public UserMap() : base("User")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.CompanyID);
    //        Map(x => x.Name);
    //        Map(x => x.Email);
    //        Map(x => x.Password);
    //        //Map(x => x.BirthDate);
    //        //Map(x => x.PhotoPath);
    //        Map(x => x.LastVisit);
    //        Map(x => x.DateCreated);
    //        Map(x => x.DateModified);
    //        Map(x => x.Gender).CustomType<GenderType>();
    //        Map(x => x.Status).CustomType<StatusType>();
    //        //Map(x => x.Role).CustomType<UserRole>();
    //    }
    //}

    //public class SliderItemMap : TableMapper<SliderItem>
    //{
    //    public SliderItemMap() : base("SliderItem")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Title);
    //        Map(x => x.Description);
    //        Map(x => x.Subtitle);
    //        Map(x => x.ImagePath);
    //        Map(x => x.ActionUrl);
    //        Map(x => x.SortOrder);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    //public class ApplicationModelMap : TableMapper<ApplicationModel>
    //{
    //    public ApplicationModelMap()
    //        : base("Application")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Name);
    //        Map(x => x.ClientID);
    //        Map(x => x.ClientSecret);
    //        Map(x => x.DateCreated);
    //        Map(x => x.DateModified);
    //        Map(x => x.AllowedIps);
    //        Map(x => x.IsPublic);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    //public class ProjectModelMap : TableMapper<ProjectModel>
    //{
    //    public ProjectModelMap()
    //        : base("Project")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Name);
    //        Map(x => x.Description);
    //        Map(x => x.CreatedBy);
    //        Map(x => x.FirmID);
    //        Map(x => x.Reference);
    //        Map(x => x.DateCreated);
    //        Map(x => x.DateModified);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    //public class ProjectRequirementModelMap : TableMapper<ProjectRequirementModel>
    //{
    //    public ProjectRequirementModelMap()
    //        : base("ProjectRequirement")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.ProjectID);
    //        Map(x => x.Requirement);
    //        Map(x => x.Notes);
    //        Map(x => x.DateCreated);
    //        Map(x => x.DateModified);
    //        Map(x => x.Status).CustomType<ProjectRequirementStatus>();
    //    }
    //}

    //public class ResourceItemMap : TableMapper<ResourceItem>
    //{
    //    public ResourceItemMap()
    //        : base("ResourceItem")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Code);
    //        Map(x => x.Tr);
    //        Map(x => x.En);
    //        Map(x => x.Scope).CustomType<ResourceScope>();
    //    }
    //}

    //public class CompanyMap : TableMapper<CompanyModel>
    //{
    //    public CompanyMap()
    //        : base("Company")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.DomainID);
    //        Map(x => x.Name);
    //        Map(x => x.CompanyCode);
    //        Map(x => x.LogoPath);
    //        Map(x => x.Address);
    //        Map(x => x.WebAddress);
    //        Map(x => x.PhoneNumber);
    //        Map(x => x.DateCreated);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    //public class TeamMap : TableMapper<Team>
    //{
    //    public TeamMap()
    //        : base("Team")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.CompanyID);
    //        Map(x => x.Name);
    //        Map(x => x.ShortName);
    //        Map(x => x.Description);
    //        Map(x => x.Logo);
    //        Map(x => x.CreateDate);
    //        Map(x => x.CreatedBy);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    //public class TeamUserMap : TableMapper<TeamUser>
    //{
    //    public TeamUserMap()
    //        : base("TeamUser")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.TeamID);
    //        Map(x => x.UserID);
    //        Map(x => x.RoleID);
    //        Map(x => x.CreateDate);
    //        Map(x => x.CreatedBy);
    //        Map(x => x.Status).CustomType<StatusType>();
    //    }
    //}

    ////public class DomainMap : TableMapper<Domain>
    ////{
    ////    public DomainMap()
    ////        : base("Domain")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.Name);
    ////        Map(x => x.MailPort);
    ////        Map(x => x.MailHost);
    ////        Map(x => x.MailUser);
    ////        Map(x => x.MailPassword);
    ////    }
    ////}

    ////public class ChatChannelMap : TableMapper<ChatChannel>
    ////{
    ////    public ChatChannelMap() : base("ChatChannel")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.Name);
    ////        Map(x => x.UserIDs);
    ////        Map(x => x.DateCreated);
    ////        Map(x => x.CreatedBy);
    ////        Map(x => x.Status).CustomType<Statuses>();
    ////    }
    ////}

    ////public class ChatMessageMap : TableMapper<ChatMessage>
    ////{
    ////    public ChatMessageMap() : base("ChatMessage")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.Content);
    ////        Map(x => x.SenderID);
    ////        Map(x => x.ChannelID);
    ////        Map(x => x.SendDate);
    ////        Map(x => x.Status).CustomType<Statuses>();
    ////    }
    ////}

    //public class LookupMap : TableMapper<Lookup>
    //{
    //    public LookupMap()
    //        : base("Lookup")
    //    {
    //        Id(x => x.ID);
    //        Map(x => x.Name);
    //        Map(x => x.Type).CustomType<LookupType>();
    //        Map(x => x.Value);
    //        Map(x => x.Order).Column(ToColumnName("Order"));
    //        Map(x => x.IsActive);
    //    }
    //}

    ////public class ItemMap : ClassMap<Item>
    ////{
    ////    public ItemMap()
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.Value).Column("Value");
    ////    }
    ////}

    ////public class JobProcessMap : TableMapper<JobProcess>
    ////{
    ////    public JobProcessMap()
    ////        : base("JobProcess")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.TeamID);
    ////        Map(x => x.Name);
    ////        Map(x => x.ShortCode);
    ////        Map(x => x.Description);
    ////        Map(x => x.StartDate).Nullable();
    ////        Map(x => x.EndDate).Nullable();
    ////        Map(x => x.DateCreated);
    ////        Map(x => x.CreatedBy);
    ////        Map(x => x.Type).CustomType<JobProcessType>();
    ////        Map(x => x.Status).CustomType<Statuses>();
    ////    }
    ////}

    ////public class IssueMap : TableMapper<Issue>
    ////{
    ////    public IssueMap()
    ////        : base("Issue")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.ProcessID);
    ////        Map(x => x.Title);
    ////        Map(x => x.Content);
    ////        Map(x => x.Tags);
    ////        Map(x => x.DateCreated);
    ////        Map(x => x.CreatedBy);
    ////        Map(x => x.Type).CustomType<IssueType>();
    ////        Map(x => x.Priority).CustomType<IssuePriority>();
    ////        Map(x => x.Status);
    ////    }
    ////}

    ////public class IssueHistoryMap : TableMapper<IssueHistory>
    ////{
    ////    public IssueHistoryMap()
    ////        : base("IssueHistory")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.IssueID);
    ////        Map(x => x.ByUser);
    ////        Map(x => x.RecordDate);
    ////        Map(x => x.Value);
    ////        Map(x => x.Description);
    ////        Map(x => x.Type).CustomType<IssueHistoryType>();
    ////    }
    ////}

    ////public class IssueAssignMap : TableMapper<IssueAssign>
    ////{
    ////    public IssueAssignMap()
    ////        : base("IssueAssign")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.IssueID);
    ////        Map(x => x.ByUser);
    ////        Map(x => x.UserID);
    ////        Map(x => x.CurrentStatus);
    ////        Map(x => x.AssignDate);
    ////    }
    ////}

    ////public class JobProcessStepMap : TableMapper<JobProcessStep>
    ////{
    ////    public JobProcessStepMap()
    ////        : base("JobProcessStep")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.CreatedBy);
    ////        Map(x => x.DateCreated);
    ////        Map(x => x.Description);
    ////        Map(x => x.Name);
    ////        Map(x => x.PostRules);
    ////        Map(x => x.PreRules);
    ////        Map(x => x.ProcessID);
    ////        Map(x => x.Status).CustomType<Statuses>();
    ////    }
    ////}

    ////public class JobProcessRuleMap : TableMapper<JobProcessRule>
    ////{
    ////    public JobProcessRuleMap()
    ////        : base("JobProcessRule")
    ////    {
    ////        Id(x => x.ID);
    ////        Map(x => x.CreatedBy);
    ////        Map(x => x.DateCreated);
    ////        Map(x => x.Description);
    ////        Map(x => x.Name);
    ////        Map(x => x.RuleType).CustomType<JobProcessRuleType>();
    ////        Map(x => x.Status).CustomType<Statuses>();
    ////    }
    ////}

    //public class TableMapper<T> : ClassMap<T> where T : class
    //{
    //    public TableMapper(string tableName)
    //    {
    //        if (!string.IsNullOrEmpty(tableName))
    //        {
    //            tableName = ToTableName(tableName);
    //            Table(tableName);
    //        }
    //    }

    //    public string ToTableName(string tableName)
    //    {
    //        var dbType = SiteSettings.DatabaseType;
    //        switch (dbType)
    //        {
    //            case DatabaseType.MsSQL:
    //                return string.Format("[{0}]", tableName);
    //            case DatabaseType.MySQL:
    //                return string.Format("`{0}`", tableName.ToLowerInvariant());
    //        }
    //        return tableName;
    //    }

    //    public string ToColumnName(string columnName)
    //    {
    //        var dbType = SiteSettings.DatabaseType;
    //        switch (dbType)
    //        {
    //            case DatabaseType.MsSQL:
    //                return string.Format("[{0}]", columnName);
    //            case DatabaseType.MySQL:
    //                return string.Format("{0}", columnName);
    //        }
    //        return columnName;
    //    }
    //}
}