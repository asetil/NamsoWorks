using System.Collections.Generic;
using System.Linq;
using Aware.Search;
using Aware.Util.Model;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Model
{
    public class StoreItemListModel
    {
        public SearchResult<StoreItem> SearchResult { get; set; }
        public List<Store> Stores { get; set; }
        public List<Category> Categories { get; set; }

        public string GetCategoryInfo(int categoryID)
        {
            if (Categories != null && categoryID > 0)
            {
                var category = Categories.FirstOrDefault(i => i.ID == categoryID);
                return category != null ? category.Name : string.Empty;
            }
            return string.Empty;
        }

        public string GetStoreInfo(int storeID)
        {
            if (Stores != null)
            {
                var store = Stores.FirstOrDefault(i => i.ID == storeID);
                return store != null ? store.DisplayName : string.Empty;
            }
            return string.Empty;
        }
    }

    public class StoreItemDetailModel
    {
        public StoreItem StoreItem { get; set; }
        public List<Store> StoreList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public List<Lookup> YesNoList { get; set; }
        public Result SaveResult { get; set; }
    }
}