using System.Collections.Generic;
using Aware.ECommerce.Model;
using CleanFramework.Business.Model;
using Aware.Search;
using Aware.Util.Lookup;

namespace CleanFramework.Business.Model
{
    public class EntryListModel
    {
        public SearchResult<Entry> SearchResult { get; set; }
        public List<Category> CategoryList { get; set; }
        public List<Lookup> StatusList { get; set; }
    }
}