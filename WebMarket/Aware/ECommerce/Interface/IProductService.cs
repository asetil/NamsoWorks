using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.Search;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface IProductService
    {
        Product Get(int id);
        Product GetProductWithName(string name);
        ProductListModel GetProductListModel(ISearchParams<Product> searchParams);
        SearchResult<Product> SearchProducts(ProductSearchParams searchParams);
        void SearchFromDb(ref ProductSearchResult result);
        List<Product> GetBarcodeProducts(List<string> barcodeList);
        List<Product> GetHomeCategoryItems(int regionID);
        Result SaveProduct(Product model);
    }
}