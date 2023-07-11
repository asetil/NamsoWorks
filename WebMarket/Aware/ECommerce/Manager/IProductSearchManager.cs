using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;

namespace Aware.ECommerce.Manager
{
    public interface IProductSearchManager
    {
        ProductSearchResult Search(ItemSearchParams searchParams, List<Store> storeList);
        Product GetProductDetail(int productID, int? storeID);
        List<Product> GetHomeCategoryItems(int regionID);
        List<StoreItem> GetOpportunityItems(int regionID);
    }
}