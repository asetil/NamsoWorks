using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Model
{
    public class SimpleItemListModel
    {
        public List<SimpleItem> List { get; set; }
        public List<Lookup> StatusList { get; set; }
    }
}