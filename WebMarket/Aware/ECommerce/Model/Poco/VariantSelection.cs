using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aware.ECommerce.Model
{
    public class VariantSelection
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual int RelationID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual string VariantCombination { get; set; } //Renk:Sarý ve Beden : XL için 5:13,7:15
        public virtual decimal Stock { get; set; }
        public virtual decimal Price { get; set; }
        public virtual string PriceInfo { get; set; } //Renk:Sarý +1TL ve Beden : XL 0.5TL için 5:13:1,7:15:0.5
        public virtual int Code { get; set; }

        [NotMapped]
        public virtual List<object> CombinationInfo { get; set; }

        public virtual VariantSelection Clone()
        {
            var result = MemberwiseClone() as VariantSelection;
            result.ID = 0;
            return result;
        }
    }
}