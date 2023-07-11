using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.ECommerce.Util;
using Aware.Search;
using Aware.Search.ElasticSearch;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Enums;
using Aware.Language;
using Aware.Language.Model;

namespace Aware.ECommerce.Manager
{
    public class ProductSearchManager : IProductSearchManager
    {
        private readonly IProductService _productService;
        private readonly IStoreItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IElasticService _elasticService;
        private readonly ILanguageService _languageService;
        private readonly IFavoriteService _favoriteService;

        private readonly ILogger _logger;

        public ProductSearchManager(IProductService productService, IStoreItemService itemService, ILanguageService languageService,
            IStoreService storeService, ICategoryService categoryService, IElasticService elasticService, ILogger logger, IFavoriteService favoriteService)
        {
            _productService = productService;
            _itemService = itemService;
            _languageService = languageService;
            _storeService = storeService;
            _categoryService = categoryService;
            _elasticService = elasticService;
            _logger = logger;
            _favoriteService = favoriteService;
        }

        public ProductSearchResult Search(ItemSearchParams searchParams,List<Store> storeList)
        {
            var result = new ProductSearchResult(searchParams);
            try
            {
                result.Stores = storeList;
                if (result.Stores != null && result.Stores.Any())
                {
                    result.Categories = _categoryService.GetCategories();
                    if (searchParams.StoreIDs == null || !searchParams.StoreIDs.Any())
                    {
                        searchParams.RegionStoreIDs = result.Stores.Select(i => i.ID).ToList();
                    }

                    //Load from Elastic Search
                    if (_elasticService.IsActive)
                    {
                        var response = _elasticService.Search<ElasticProduct>(searchParams);
                        if (response != null && response.Success)
                        {
                            var products = Mapper.MapToProduct(response.Results, result.Stores);
                            result.TotalSize = (int)response.TotalSize;
                            result.Took = response.Took;
                            result.Results = products.ToList();
                            result.Aggregations = response.Aggregations;
                        }
                    }

                    if (result.Results == null) //Load from DB, since Elastic not available
                    {
                        if (result.SearchParams.OnlyFavorites && result.SearchParams.UserID > 0)
                        {
                            var favoriteIDs = _favoriteService.GetUserFavorites(result.SearchParams.UserID);
                            result.SearchParams.IDs = favoriteIDs;
                        }
                        _productService.SearchFromDb(ref result);
                    }

                    //Set multilanguage values
                    result.Results = SetLanguageValues(result.Results);
                    if (searchParams.SearchCategories)
                    {
                        result.Categories = result.Categories.Where(c => c.Name.ToLowerInvariant().Contains(searchParams.Keyword)).ToList();
                    }

                    result.SearchParams = searchParams;
                    if (searchParams.IncludeAggregations)
                    {
                        result.ArrangeAggregations();
                    }
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                //TODO:#Buraya filtreler string olarak eklenebilir.
                _logger.Error("ProductSearchManager > SearchProducts - Fail with : {0}", ex, "TODO");
            }
            return result;
        }
       
        public Product GetProductDetail(int productID, int? storeID)
        {
            Product product = null;
            try
            {
                if (productID > 0)
                {
                    var stores = _storeService.GetRegionStores();
                    if (stores != null && stores.Any())
                    {
                        if (_elasticService.IsActive)
                        {
                            var searchParams = new ProductSearchParams { IDs = new List<int> { productID } };
                            searchParams.SetPaging(0, 1);

                            var response = _elasticService.Search<ElasticProduct>(searchParams);
                            if (response != null && response.Success)
                            {
                                var products = Mapper.MapToProduct(response.Results, stores);
                                product = products.FirstOrDefault();
                            }
                        }
                        else
                        {
                            product = _productService.Get(productID);
                            var products = new List<Product> { product };
                            _itemService.LoadItems(ref products, stores);
                            product = products.FirstOrDefault();
                        }

                        //Set multilanguage values
                        var list = SetLanguageValues(new List<Product>() { product });
                        product = list.FirstOrDefault();

                        if (product != null && storeID.GetValueOrDefault() > 0 && product.Items.Any(i => i.StoreID == storeID.Value))
                        {
                            product.Items.FirstOrDefault(i => i.StoreID == storeID.Value).AddScore(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ProductService > GetProductDetail - Fail for productID:{0}!", ex, productID);
            }
            return product;
        }

        public List<Product> GetHomeCategoryItems(int regionID)
        {
            var products = _productService.GetHomeCategoryItems(regionID);
            _itemService.LoadItems(ref products);
            return products;
        }

        public List<StoreItem> GetOpportunityItems(int regionID)
        {
            return _itemService.GetOpportunityItems(regionID);
        }

        private List<Product> SetLanguageValues(List<Product> productList)
        {
            if (productList != null && productList.Any())
            {
                var productIDs = productList.Select(i => i.ID).ToList();
                var languageValues = _languageService.GetLanguageValues((int)RelationTypes.Product, productIDs, Constants.DefaultLangID);

                productList = productList.Select(i =>
                {
                    i.Name = _languageService.GetFieldContent(languageValues, i.ID, "Name", i.Name);
                    i.Description = _languageService.GetFieldContent(languageValues, i.ID, "Description", i.Description);
                    i.ShortDescription = _languageService.GetFieldContent(languageValues, i.ID, "ShortDescription", i.ShortDescription);
                    return i;
                }).ToList();
            }
            return productList;
        }
    }
}
