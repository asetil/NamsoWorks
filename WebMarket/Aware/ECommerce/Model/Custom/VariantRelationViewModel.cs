using System.Collections.Generic;
using Aware.Util;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class VariantRelationViewModel
    {
        public int RelationID { get; set; }
        public int RelationType { get; set; }
        public List<VariantSelection> Selections { get; set; }
        public List<VariantRelation> Relations { get; set; }
        public List<VariantProperty> VariantProperties { get; set; }
        public int MaxCombinations { get; set; }
    }

    public class VariantCheckResult
    {
        public bool Valid { get { return string.IsNullOrEmpty(Message); } }
        public bool UseStock { get { return VariantSelection != null; } }
        public string Message { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public VariantSelection VariantSelection { get; set; }
    }
}
