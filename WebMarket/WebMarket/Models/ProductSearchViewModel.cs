using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Search;
using Aware.Util.Model;
using Aware.Util.View;
using Aware.Util.Enums;

namespace WebMarket.Models
{
    public class ProductSearchViewModel
    {
        public ProductSearchResult SearchResult { get; set; }
        public LayoutDirection FilterDirection { get; set; }
        public List<int> FavoriteProducts { get; set; }
        public bool ForStoreDetail { get; set; }
        public bool AllowCompare { get; set; }

        public bool IsFavorite(int productID)
        {
            return productID > 0 && FavoriteProducts != null && FavoriteProducts.Contains(productID);
        }

        public string Title
        {
            get
            {
                var result = string.Empty;
                if (SearchResult != null && SearchResult.SearchParams != null)
                {
                    var searchParams = SearchResult.SearchParams;
                    if (searchParams.OnlyFavorites && searchParams.UserID > 0)
                    {
                        return "Favori Ürünlerim";
                    }

                    if (searchParams.StoreIDs != null && searchParams.StoreIDs.Count() == 1 && SearchResult.Stores != null)
                    {
                        var store = SearchResult.Stores.FirstOrDefault(i => i.ID == searchParams.StoreIDs.FirstOrDefault());
                        result = store != null ? store.DisplayName : result;
                    }

                    if (searchParams.CategoryIDs != null && searchParams.CategoryIDs.Count() == 1 && SearchResult.Categories != null)
                    {
                        var category = SearchResult.Categories.FirstOrDefault(i => i.ID == searchParams.CategoryIDs.FirstOrDefault());
                        result += category != null ? string.Format(", {0}", category.Name) : string.Empty;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        return string.Format("{0} Ürünleri", result.Trim(',').Trim());
                    }
                }
                return "Ürünler";
            }
        }

        public List<Selecto> GetFilters()
        {
            var filters = new List<Selecto>();
            var resultCount = SearchResult.Results != null && SearchResult.Results.Any() ? SearchResult.Results.Count : 0;
            var searchParams = SearchResult.SearchParams as ItemSearchParams;

            if (FilterDirection == LayoutDirection.Horizantal)
            {
                if (!ForStoreDetail && SearchResult.Stores != null && SearchResult.Stores.Count() > 1)
                {
                    var marketList = new List<Item>();
                    foreach (var item in SearchResult.Stores)
                    {
                        marketList.Add(Item.New(item.ID, item.DisplayName));
                    }

                    var selectedStore = searchParams.StoreIDs != null && searchParams.StoreIDs.Count() == 1 ? searchParams.StoreIDs.FirstOrDefault() : 0;
                    filters.Add(new Selecto(string.Empty, marketList, selectedStore, "select-store", "filterByStore", "Market"));
                }


                if ((!ForStoreDetail || (ForStoreDetail && resultCount > 0)) && SearchResult.Categories != null && SearchResult.Categories.Count() > 1)
                {
                    var categoryList = new List<Item>();
                    foreach (var item in SearchResult.Categories)
                    {
                        categoryList.Add(Item.New(item.ID, item.Name));
                    }

                    var selectedCategory = searchParams.CategoryIDs != null && searchParams.CategoryIDs.Count() == 1 ? searchParams.CategoryIDs.FirstOrDefault() : 0;
                    filters.Add(new Selecto(string.Empty, categoryList, selectedCategory, "select-category", "filterByCategory", "Kategori"));
                }
            }

            if (resultCount > 0)
            {
                var orderByList = new List<Item>
                            {
                                new Item(1, "Ucuzdan pahalýya"),
                                new Item(2, "Pahalýdan ucuza"),
                                new Item(3, "Ýsme Göre (A-Z)"),
                                new Item(4, "Ýsme Göre (Z-A)"),
                            };
                filters.Add(new Selecto(string.Empty, orderByList, searchParams.OrderBy, "select-order", "orderItemsBy", "Sýrala"));
            }
            return filters;
        }
    }
}