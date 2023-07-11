using System.Collections.Generic;
using Aware.ECommerce.Enums;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace CleanFramework.Business.Model
{
    public class EntryDetailModel
    {
        public Entry Entry { get; set; }
        public List<Item> CategoryList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public UserRole AuthorRole { get; set; }
    }
}