using System.Collections.Generic;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace Aware.ECommerce.Model
{
    public class PropertyViewModel
    {
        public PropertyValue Property { get; set; }
        public List<PropertyValue> ParentList { get; set; }
        public List<PropertyValue> ChildList { get; set; }
        public List<Lookup> PropertyTypeList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public bool AllowEdit { get; set; }
        public Result Result { get; set; }
    }
}
