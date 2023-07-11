//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Aware.ECommerce.Model;
//using Aware.ECommerce.Search;
//using Aware.Search;
//using Aware.Util;

//namespace Aware.ECommerce.Manager
//{
//    public class SmartSearchManager
//    {
//        public static ProductSearchResult SearchSmart(IEnumerable<Category> categories, string smartText, string fields = "")
//        {
//            //papaz eriği 5-20
//            //mevye suyu cappy şeftali aromalı 1lt çeşitleri 3-5

//            var smartFilterText = smartText.Trim().Trim('[').Trim(']').Trim('(').Trim(')').ToLowerInvariant().RemoveInjection();
//            var result = CalculateSmartFilters(smartFilterText, GetSmartLookup());
//            return result;
//        }

//        private static ProductSearchResult CalculateSmartFilters(string smartText, Dictionary<Enums.FilterType, string> lookupTable)
//        {
//            Expression<Func<StoreItem, bool>> left = null;
//            var orFilters = smartText.Trim(',').Split(',');

//            var selectedCategories = new List<Category>();
//            //foreach (var orFilter in orFilters)
//            //{
//            //    var result = GetOrFilters(searchParams, orFilter, lookupTable);
//            //    selectedCategories.AddRange(GetCategories(searchParams, orFilter));
//            //    left = left.Combine(result, Expression.OrElse);
//            //}

//            //if (left != null)
//            //{
//            //    searchParams.Filters.Add(left);
//            //}
//            ////searchParams.Categories = selectedCategories;
//            //return searchParams;

//            //foreach (var lookup in lookupTable)
//            //{
//            //    if (lookup.Value.IndexOf(searchText) > -1)
//            //    {
//            //        var filter = GetFilter(lookup, searchText);
//            //        if (filter != null)
//            //        {
//            //            left = left != null ? left.Combine(filter, Expression.AndAlso) : filter;
//            //        }
//            //    }
//            //}

//            return null;
//        }

//        private static IEnumerable<Category> GetCategories(ProductSearchParams searchParams, string filterText)
//        {
//            var words = filterText.Split(' ').Where(i => i.Length > 2 && !(i.Int() > 0 || i.IndexOf('-') > -1));
//            var selectedCategories = new List<Category>();
//            foreach (var word in words)
//            {
//                //selectedCategories.AddRange(searchParams.Categories.Where(i => i.Name.ToLowerInvariant().Contains(word)));
//            }
//            return selectedCategories;
//        }

//        private static Expression<Func<StoreItem, bool>> GetOrFilters(ProductSearchParams searchParams, string filterText, Dictionary<Enums.FilterType, string> lookupTable)
//        {
//            var priceFilters = GetPriceFilters(filterText);
//            var keywordFilters = GetKeywordFilters(filterText);
//            return priceFilters.Combine(keywordFilters, Expression.AndAlso);
//        }

//        private static Expression<Func<StoreItem, bool>> GetPriceFilters(string filterText)
//        {
//            Expression<Func<StoreItem, bool>> left = null;
//            var words = filterText.Split(' ').Where(i => !string.IsNullOrEmpty(i) && (i.Int() > 0 || i.IndexOf('-') > -1));
//            foreach (var word in words)
//            {
//                if (word.Int() > 0 || word.IndexOf('-') > -1) //Fiyat olabilir?
//                {
//                    Expression<Func<StoreItem, bool>> priceFilter = null;
//                    if (word.IndexOf('-') > -1)
//                    {
//                        var minPrice = word.Split('-')[0].Dec();
//                        var maxPrice = word.Split('-')[1].Dec();
//                        priceFilter = i => i.SalesPrice >= minPrice && i.SalesPrice <= maxPrice;
//                    }
//                    else if (word.Int() > 0)
//                    {
//                        var price = word.Dec();
//                        priceFilter = i => i.SalesPrice <= price;
//                    }
//                    left = left.Combine(priceFilter, Expression.AndAlso);
//                }
//            }
//            return left;
//        }

//        private static Dictionary<Enums.FilterType, string> GetSmartLookup()
//        {
//            var result = new Dictionary<Enums.FilterType, string>();
//            result.Add(Enums.FilterType.Brand, "1:adidas,2:mercedes,3:lg,4:samsung,5:apple");
//            result.Add(Enums.FilterType.Size, "1:m,2:s,3:l,4:xl,5:xxl");
//            return result;
//        }
//        private static Expression<Func<StoreItem, bool>> GetFilter(KeyValuePair<Enums.FilterType, string> lookup, string searchText)
//        {
//            var ss = lookup.Value.Split(',');
//            var ll = ss.Where(i => i.Contains(searchText));
//            var selectedIDs = ll.Select(i => i.Split(':')[0].Int());
//            return null;
//        }

//        private static Expression<Func<StoreItem, bool>> GetKeywordFilters(string smartText)
//        {
//            Expression<Func<StoreItem, bool>> left = null;
//            smartText = smartText.Trim().Trim('[').Trim(']').Trim('(').Trim(')').ToLowerInvariant();
//            var words = smartText.Split(' ').Where(i => i.Length > 2 && !(i.Int() > 0 || i.IndexOf('-') > -1));
//            foreach (var word in words)
//            {
//                Expression<Func<StoreItem, bool>> singleKeywordFilter = i => i.Product.Name.Contains(word);
//                left = left.Combine(singleKeywordFilter, Expression.OrElse);
//            }
//            return left;
//        }
//    }
//}