using System.Collections.Generic;
using Aware.Search;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Model
{
    public class ProductListModel
    {
        public SearchResult<Product> SearchResult { get; set; }
        public List<Category> Categories { get; set; }
        public List<Lookup> StatusList { get; set; }
    }
}
