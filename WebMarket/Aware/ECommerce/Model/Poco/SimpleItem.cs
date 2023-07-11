using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class SimpleItem
    {
        public virtual int ID { get; set; }
        public virtual ItemType Type { get; set; }
        public virtual string Title { get; set; }
        public virtual string Value { get; set; }
        public virtual string Url { get; set; }
        public virtual int SubType { get; set; }
        public virtual decimal Cost { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }
    }
}