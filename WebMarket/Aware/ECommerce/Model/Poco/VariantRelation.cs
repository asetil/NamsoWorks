using System.ComponentModel.DataAnnotations;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class VariantRelation
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual int RelationID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual int VariantID { get; set; }
        public virtual int VariantValue { get; set; }
        public virtual decimal Price { get; set; }    //+5 TL
        public virtual Statuses Status { get; set; }
    }
}