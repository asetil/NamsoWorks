using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class PropertyRelation
    {
        [Key]
        public virtual int ID { get; set; }

        [ForeignKey("PropertyValue")]
        public virtual int PropertyValueID { get; set; }
        public virtual int RelationID { get; set; }
        public virtual string Value { get; set; }
        public virtual string SortOrder { get; set; } // Özellikler sıralanmak istenirse
        public virtual int RelationType { get; set; }
        public virtual Statuses Status { get; set; }
        public virtual PropertyValue PropertyValue { get; set; }
    }
}