using System.ComponentModel.DataAnnotations;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class PropertyValue
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual PropertyType Type { get; set; } // özelliğin tipi ne?
        public virtual int ParentID { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }
    }
}