using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Model
{
    public class VariantListModel
    {
        public List<VariantProperty> VariantProperties { get; set; }
        public List<Lookup> PropertyDisplayModes { get; set; }
    }
}