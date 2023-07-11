using Aware.Util;
using Aware.Util.Enums;

namespace Aware.Regional.Model
{
    public class Region
    {
        public virtual int ID { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string Name { get; set; }
        public virtual RegionType Type { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
