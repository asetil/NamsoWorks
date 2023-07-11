using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class VariantProperty
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual PropertyDisplayMode DisplayMode { get; set; }
        public virtual int MaxSelection { get; set; }
        public virtual bool IsRequired { get; set; }
        public virtual bool TrackStock { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }

        [NotMapped]
        public virtual List<PropertyValue> OptionList { get; set; }
    }
}