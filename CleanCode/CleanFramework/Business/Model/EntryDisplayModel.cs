using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Util.Model;

namespace CleanFramework.Business.Model
{
    public class EntryDisplayModel
    {
        public Entry Entry { get; set; }
        public List<Item> RelatedEntries { get; set; }
        public List<Category> CategoryHierarchy { get; set; }
    }
}