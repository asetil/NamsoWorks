using Aware.Data.NHibernate.Mapping;
using Aware.Util;
using Aware.Util.Enums;
using CleanFramework.Business.Model;

namespace CleanFramework.Data.Mapping
{
    public class EntryMap : TableMapper<Entry>
    {
        public EntryMap(): base("Entry")
        {
            Id(x => x.ID);
            Map(x => x.UserID);
            Map(x => x.CategoryID);
            Map(x => x.Name);
            Map(x => x.Content).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.Summary);
            Map(x => x.Keywords);
            Map(x => x.ImageInfo);
            Map(x => x.SortOrder);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }
}