using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Search;
using Aware.Search.ElasticSearch;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Search
{
    public class ItemSearchParams : SearchParams<StoreItem>
    {
        public List<int> StoreIDs { get; set; }
        public List<int> RegionStoreIDs { get; set; }
        public List<int> CategoryIDs { get; set; }
        public List<int> PropertyIDs { get; set; }
        public string Barcode { get; set; }
        public string Stock { get; set; }
        public bool IncludeUnlimitedStock { get; set; }
        public bool OnlyForSale { get; set; }
        public string Price { get; set; } //[25:45] => 25 ile 45 TL arası olan
        public string Rating { get; set; }
        public bool SearchCategories { get; set; }
        public bool OnlyFavorites { get; set; }

        private int _orderBy;
        public int OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                if (_orderBy == 0 || _orderBy == 1 || _orderBy == 2)
                {
                    SortBy(i => i.SalesPrice, _orderBy == 2);
                }
                else if (_orderBy == 3 || _orderBy == 4)
                {
                    SortBy(i => i.Product.Name, _orderBy == 4);
                }
            }
        }

        public ItemSearchParams WithStore(int? storeID, IEnumerable<int> storeIDs = null)
        {
            StoreIDs = StoreIDs ?? new List<int>();
            if (storeIDs != null) { StoreIDs.AddRange(storeIDs); }
            if (storeID.HasValue && storeID.Value > 0) { StoreIDs.Add(storeID.Value); }

            StoreIDs = StoreIDs.Distinct().ToList();
            return this;
        }

        public ItemSearchParams WithCategory(int? categoryID, IEnumerable<int> categoryIDs = null, bool containHierarchy = false)
        {
            CategoryIDs = CategoryIDs ?? new List<int>();
            if (categoryIDs != null) { CategoryIDs.AddRange(categoryIDs); }
            if (categoryID.HasValue && categoryID.Value > 0) { CategoryIDs.Add(categoryID.Value); }

            CategoryIDs = CategoryIDs.Distinct().ToList();
            return this;
        }

        public override SearchHelper<StoreItem> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            searchHelper.ClearFilters();

            if (OnlyFavorites && UserID > 0)
            {
                var favoriteService = WindsorBootstrapper.Resolve<IFavoriteService>();
                var favoriteIDs = favoriteService.GetUserFavorites(UserID);
                searchHelper.FilterBy(i => favoriteIDs.Contains(i.ProductID));
            }
            else if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID) || IDs.Contains(i.ProductID));
            }

            var storeIDs = StoreIDs ?? RegionStoreIDs;
            if (storeIDs != null && storeIDs.Any())
            {
                searchHelper.FilterBy(i => storeIDs.Contains(i.StoreID));
            }

            if (Status.HasValue && Status.Value != Statuses.None)
            {
                searchHelper.FilterBy(i => i.Status == Status.Value);
            }

            ArrangeStockFilter(ref searchHelper);
            ArrangePriceFilter(ref searchHelper);

            var hierarchicCategoryIDs = GetHierarchicalCategories();
            if (hierarchicCategoryIDs != null && hierarchicCategoryIDs.Any())
            {
                searchHelper.FilterBy(i => hierarchicCategoryIDs.Contains(i.Product.CategoryID));
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                var key = Keyword.Trim().ToLowerInvariant();
                searchHelper.FilterBy(i => i.Product.Name.ToLower().Contains(key));
            }

            if (!string.IsNullOrEmpty(Barcode))
            {
                searchHelper.FilterBy(i => i.Product.Barcode.Contains(Barcode));
            }

            if (OnlyForSale)
            {
                searchHelper.FilterBy(i => i.IsForSale);
            }


            return searchHelper;
        }

        private void ArrangePriceFilter(ref SearchHelper<StoreItem> searchHelper)
        {
            if (!string.IsNullOrEmpty(Price))
            {
                var priceFilters = GetFilterRanges(Price, Constants.PRICE_MAX);
                Expression<Func<StoreItem, bool>> result = null;

                foreach (var filter in priceFilters)
                {
                    if (filter.Value >= -1)
                    {
                        result = result.Combine(i => i.SalesPrice >= (decimal)filter.Key && i.SalesPrice <= (decimal)filter.Value, Expression.OrElse);
                    }
                    else
                    {
                        result = result.Combine(i => i.SalesPrice == (decimal)filter.Key, Expression.OrElse);
                    }
                }

                if (result != null)
                {
                    searchHelper.FilterBy(result);
                }
            }
        }

        private void ArrangeStockFilter(ref SearchHelper<StoreItem> searchHelper)
        {
            var stock = string.IsNullOrEmpty(Stock) ? string.Empty : Stock;
            var stockFilterValues = GetFilterRanges(stock, Constants.STOCK_MAX);
            if (IncludeUnlimitedStock) { stockFilterValues.Add(new KeyValuePair<double, double>(-1, -1)); }

            Expression<Func<StoreItem, bool>> stockFilters = null;
            foreach (var filter in stockFilterValues)
            {
                if (filter.Value >= -1)
                {
                    stockFilters = stockFilters.Combine(i => i.Stock >= (decimal)filter.Key && i.Stock <= (decimal)filter.Value, Expression.OrElse);
                }
                else
                {
                    stockFilters = stockFilters.Combine(i => i.Stock == (decimal)filter.Key, Expression.OrElse);
                }
            }

            if (stockFilters != null)
            {
                searchHelper.FilterBy(stockFilters);
            }
        }

        private List<int> GetHierarchicalCategories()
        {
            if (CategoryIDs != null && CategoryIDs.Any())
            {
                var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                return categoryService.GetRelatedCategoryIDs(CategoryIDs.ToList());
            }
            return null;
        }

        public ItemSearchParams(string keyword = "", int page = 1, int size = 25, string barcode = "")
        {
            Keyword = keyword;
            Barcode = barcode;
            SetPaging(page, size);
        }

        public override ElasticHelper<TX> GetElasticHelper<TX>()
        {
            var helper = new ElasticHelper<ElasticProduct>();
            if (OnlyFavorites && UserID > 0)
            {
                var favoriteService = WindsorBootstrapper.Resolve<IFavoriteService>();
                var favoriteIDs = favoriteService.GetUserFavorites(UserID);
                helper.Terms(t => t.ID, favoriteIDs);
            }
            else if (IDs != null && IDs.Any())
            {
                helper.Terms(t => t.ID, IDs);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.ToElasticKeyword().Split(' ').Where(i => i.Length >= 2);
                if (keywords.Count() > 1)
                {
                    var keywordQueries = helper.Sub();
                    foreach (var keyword in keywords)
                    {
                        keywordQueries.Wildcard(f => f.Name, string.Format("*{0}*", keyword));
                    }
                    helper.Bool(b => b.Should(keywordQueries.QueryList.ToArray()));
                }
                else
                {
                    helper.Wildcard(f => f.Name, string.Format("*{0}*", keywords.FirstOrDefault()));
                }
            }

            if (!string.IsNullOrEmpty(Barcode))
            {
                helper.Wildcard(f => f.Barcode, string.Format("*{0}*", Barcode));
            }

            var hierarchicCategoryIDs = GetHierarchicalCategories();
            if (hierarchicCategoryIDs != null && hierarchicCategoryIDs.Any())
            {
                helper.Terms(t => t.CategoryID, hierarchicCategoryIDs);
            }

            if (Status.HasValue)
            {
                helper.Term(f => f.Status, ((int)Status.Value).ToString());
            }

            var nestedQuries = helper.Sub();
            var storeIDs = StoreIDs ?? RegionStoreIDs;
            if (storeIDs != null && storeIDs.Any())
            {
                nestedQuries.Terms(t => t.Items.FirstOrDefault().StoreID, storeIDs);
            }

            if (OnlyForSale)
            {
                nestedQuries.Term(t => t.Items.FirstOrDefault().IsForSale, "true");
            }

            var stock = string.IsNullOrEmpty(Stock) ? string.Empty : Stock;
            var stockRangeList = GetFilterRanges(stock, Constants.STOCK_MAX);
            if (IncludeUnlimitedStock) { stockRangeList.Add(new KeyValuePair<double, double>(-1, -1)); }
            GetElasticRangeFilters(ref nestedQuries, stockRangeList, f => f.Items.FirstOrDefault().Stock);

            if (!string.IsNullOrEmpty(Price))
            {
                var priceRangeList = GetFilterRanges(Price, Constants.PRICE_MAX);
                GetElasticRangeFilters(ref nestedQuries, priceRangeList, f => f.Items.FirstOrDefault().SalesPrice);
            }

            if (!string.IsNullOrEmpty(Rating))
            {
                var ratingRangeList = GetFilterRanges(Rating, 5).Select(i =>
                {
                    var key = Math.Max(0, i.Key - 0.5);
                    var value = Math.Min(5.01, i.Key + 0.5);
                    return new KeyValuePair<double, double>(key, value);
                }).ToList();

                GetElasticRangeFilters(ref helper, ratingRangeList, f => f.Rating);
            }

            if (nestedQuries.QueryList != null && nestedQuries.QueryList.Any())
            {
                helper.Nested(n => n.Items).Bool(b => b.Must(nestedQuries.QueryList.ToArray()));
            }

            if (PropertyIDs != null && PropertyIDs.Any())
            {
                nestedQuries = helper.Sub();
                nestedQuries.Terms(t => t.Properties.FirstOrDefault().Value, PropertyIDs);
                helper.Nested(n => n.Properties).Bool(b => b.Must(nestedQuries.QueryList.ToArray()));
            }

            if (IncludeAggregations)
            {
                helper.Aggregation(t => t.CategoryID).SetName("Kategoriler", AgregationMapType.Category, "cid").Add();
                helper.Aggregation(t => t.Items.FirstOrDefault().StoreID, 10).SetNested("items").SetName("Marketler", AgregationMapType.Store, "sid").Add();

                var propertyValues = helper.Aggregation(t => t.Properties.FirstOrDefault().Value, 20).SetName("values", AgregationMapType.PropertyValue, "pid").Aggregation;
                helper.Aggregation(t => t.Properties.FirstOrDefault().ID, 10)
                    .SetNested("properties").SetName("Özellikler", AgregationMapType.Property, "property")
                    .AddChild(propertyValues).Add();

                helper.Aggregation(t => t.Rating, 10, AgregationType.Range)
                   .AddRange("0", "0 Yıldız", 0, 0.5)
                   .AddRange("1", "1 Yıldız", 0.5, 1.5)
                   .AddRange("2", "2 Yıldız", 1.50, 2.5)
                   .AddRange("3", "3 Yıldız", 2.5, 3.5)
                   .AddRange("4", "4 Yıldız", 3.5, 4.5)
                   .AddRange("5", "5 Yıldız", 4.5, 5.0)
                   .SetName("Puanlama", AgregationMapType.CommentRating, "rating").Add();

                helper.Aggregation(t => t.Items.FirstOrDefault().Stock, 10, AgregationType.Range).SetNested("items")
                   .AddRange("-1", "Sınırsız", -1, 0)
                   .AddRange("0", "Tükenen", 0, 0.01)
                   .AddRange("0:50", "0-50", 0.01, 50)
                   .AddRange("50:100", "50-100", 50, 100)
                   .AddRange("100:9999", "100 +", 100, 9999.01)
                   .SetName("Stok", AgregationMapType.Stock, "stock").Add();

                helper.Aggregation(t => t.Items.FirstOrDefault().SalesPrice, 10, AgregationType.Range).SetNested("items")
                    .AddRange("0:25", "0 - 25 TL Arası", 0, 25)
                    .AddRange("25:50", "25 - 50 TL Arası", 25, 50)
                    .AddRange("50:100", "50 - 100 TL Arası", 50, 100)
                    .AddRange("100:250", "100 - 250 TL Arası", 100, 250)
                    .AddRange("250:500", "250 - 500 TL Arası", 250, 500)
                    .AddRange("500:100", "500 - 1000 TL Arası", 500, 1000)
                    .AddRange("1000:9999", "1000 TL ve üstü", 1000, 9999.01)
                    .SetName("Fiyat Aralığı", AgregationMapType.Price, "price").Add();

                //helper.Aggregation(t => t.ID, 1, AgregationType.Cardinality).SetName("Ürün Sayısı","product_count").Add();
                //helper.Aggregation(t => t.Items.FirstOrDefault().SalesPrice, 5, AgregationType.Max).SetName("Satış Fiyatı","sale_price").SetNested("items").Add();
                //helper.Aggregation(t => t.Items.FirstOrDefault().Stock, 10).SetNested("items").SetName("Stok","stock").Add();

                //var dateHistogram = helper.Aggregation(t => t.DateModified, 255, AgregationType.DateHistogram)
                //    .SetName("Histogram","histogram").SetHistogram(IntervalType.Month).Aggregation;
                //helper.Aggregation(t => t.CategoryID, 10).AddChild(dateHistogram).SetName("Kategori","osman").Add();
            }

            if (SearchHelper.SortList != null && SearchHelper.SortList.Any())
            {
                helper.SortList = SearchHelper.SortList.Select(sortField =>
                {
                    var elasticSorter = new Sorter<ElasticProduct>()
                    {
                        Descending = sortField.Descending
                    };

                    //TODO#
                    //var sortType = (ItemSortField)((int)sortField.ElasticOrderBy());
                    //switch (sortType)
                    //{
                    //    case ItemSortField.SalesPrice:
                    //        Expression<Func<ElasticProduct, decimal>> predicate = (i => i.Items.FirstOrDefault().SalesPrice);
                    //        elasticSorter.DynamicOnField = predicate;
                    //        elasticSorter.ResultType = typeof(decimal);
                    //        break;
                    //    case ItemSortField.Name:
                    //        Expression<Func<ElasticProduct, string>> namePredicate = (i => i.Name);
                    //        elasticSorter.DynamicOnField = namePredicate;
                    //        elasticSorter.ResultType = typeof(string);
                    //        break;
                    //}
                    return elasticSorter;
                }).ToList();
            }

            return helper as ElasticHelper<TX>;
        }

        //public ItemSearchParams Sort(Enums.ItemSortField sortField, bool descending = false)
        //{
        //    SortFields = SortFields ?? new List<KeyValuePair<Enums.ItemSortField, bool>>();
        //    SortFields.Add(new KeyValuePair<Enums.ItemSortField, bool>(sortField, descending));
        //    return this;
        //}

        private void GetElasticRangeFilters(ref ElasticHelper<ElasticProduct> elasticHelper, List<KeyValuePair<double, double>> filters, Expression<Func<ElasticProduct, object>> onField)
        {
            if (filters != null && filters.Any())
            {
                var sub = elasticHelper.Sub();
                foreach (var filter in filters)
                {
                    if (filter.Value >= -1)
                    {
                        sub.Range(onField, filter.Key, filter.Value);
                    }
                    else
                    {
                        sub.Term(onField, filter.Key.ToString());
                    }
                }

                if (sub.QueryList.Count == 1)
                {
                    elasticHelper.QueryList.Add(sub.QueryList.FirstOrDefault());
                }
                else if (sub.QueryList.Any())
                {
                    elasticHelper.Bool(b => b.Should(sub.QueryList.ToArray()));
                }
            }
        }
    }
}
