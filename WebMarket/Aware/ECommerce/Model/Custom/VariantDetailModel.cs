using System.Collections.Generic;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace Aware.ECommerce.Model
{
    public class VariantDetailModel
    {
        public VariantProperty VariantProperty { get; set; }
        public List<PropertyValue> ChildList { get; set; }
        public List<Lookup> PropertyDisplayModes { get; set; }
        public List<Lookup> StatusList { get; set; }
        public bool AllowEdit { get; set; }
        public Result SaveResult { get; set; }
    }
}