using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.Dependency;
using Aware.ECommerce.Model;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Search;
using Aware.ECommerce.Util;
using Aware.Search;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<StoreItem> _itemRepository;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly ILookupManager _lookupManager;

        public ProductService( IRepository<Product> productRepository, IRepository<StoreItem> itemRepository,ICategoryService categoryService, ILogger logger, ILookupManager lookupManager)
        {
            _categoryService = categoryService;
            _productRepository = productRepository;
            _logger = logger;
            _lookupManager = lookupManager;
            _itemRepository = itemRepository;
        }

        public Product Get(int id)
        {
            return id > 0 ? _productRepository.Get(id) : new Product()
            {
                Category = new Category(),
                Status = Statuses.Active
            };
        }

        public Product GetProductWithName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _productRepository.First(i => i.Name == name);
            }
            return null;
        }

        public ProductListModel GetProductListModel(ISearchParams<Product> searchParams)
        {
            try
            {
                if (searchParams != null)
                {
                    var result = new ProductListModel()
                    {
                        SearchResult = _productRepository.Find(searchParams),
                        Categories = _categoryService.GetCategories(),
                        StatusList = _lookupManager.GetLookups(LookupType.Status)
                    };

                    result.SearchResult.Results = result.SearchResult.Results.Select(i =>
                    {
                        i.Category = result.Categories.FirstOrDefault(c => c.ID == i.CategoryID);
                        return i;
                    }).ToList();
                    result.SearchResult.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ProductService > GetProducts - Failed", ex);
            }
            return new ProductListModel();
        }

        public SearchResult<Product> SearchProducts(ProductSearchParams searchParams)
        {
            try
            {
                if (searchParams != null)
                {
                    var result = _productRepository.Find(searchParams);
                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ProductService > GetProducts - Failed", ex);
            }
            return null;
        }


        public List<Product> GetBarcodeProducts(List<string> barcodeList)
        {
            if (barcodeList != null && barcodeList.Any())
            {
                return _productRepository.Where(i => barcodeList.Contains(i.Barcode)).ToList();
            }
            return new List<Product>();
        }

        public List<Product> GetHomeCategoryItems(int regionID)
        {
            try
            {
                var sql = SqlHelper.GetHomeCategoryItems(regionID);
                var products = _productRepository.GetWithSql(sql, true, new object[] { }).ToList();
                return products;
            }
            catch (Exception ex)
            {
                _logger.Error("ProductService > GetHomeCategoryItems - Failed", ex);
            }
            return null;
        }

        public Result SaveProduct(Product model)
        {
            try
            {
                if (model == null) { return Result.Error(); }
                if (model.ID > 0)
                {
                    var product = _productRepository.Where(i => i.ID == model.ID).First();
                    if (product != null)
                    {
                        Mapper.Map(ref product, model);
                        var success = _productRepository.Update(product);
                        return success ? Result.Success(product, Resource.General_Success) : Result.Error(Resource.General_Error);
                    }
                    _logger.Error(string.Format("Product > SaveProduct - Not Found with  with : {0}!", model.ID), null);
                }
                else
                {
                    model.DateModified = DateTime.Now;
                    _productRepository.Add(model);
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                var id = model != null ? model.ID : 0;
                _logger.Error(string.Format("Product > SaveProduct - Fail with : {0}!", id), ex);
            }
            return Result.Error();
        }

        public void SearchFromDb(ref ProductSearchResult searchResult)
        {
            if (searchResult != null)
            {
                var stores = searchResult.Stores;
                var productSearchSql = SqlHelper.GetProductSearchSQL(searchResult.SearchParams);
                var products = _productRepository.GetWithSql(productSearchSql);

                if (products.Any())
                {
                    var productIDs = products.Select(i => i.ID).ToList();
                    var itemSearchSql = SqlHelper.GetItemSearchSql(searchResult.SearchParams, productIDs);

                    var storeItems = _itemRepository.GetWithSql(itemSearchSql) ?? new List<StoreItem>();
                    storeItems = storeItems.Select(si =>
                    {
                        si.Store = stores.FirstOrDefault(s => s.ID == si.StoreID) ?? new Store();
                        return si;
                    }).ToList();

                    products = products.Select(i =>
                    {
                        i.Items = storeItems.Where(si => si.ProductID == i.ID);
                        return i;
                    }).ToList();
                    searchResult.Results = products;

                    if (searchResult.SearchParams.Page == 0 || searchResult.SearchParams.IncludeCount)
                    {
                        var countSql = SqlHelper.GetProductSearchCountSQL(searchResult.SearchParams);
                        if (Config.DatabaseType == DatabaseType.MsSQL)
                        {
                            var countInfo = _productRepository.GetWithSql<int>(countSql);
                            searchResult.TotalSize = countInfo != null && countInfo.Any() ? countInfo.FirstOrDefault() : 0;
                        }
                        else if (Config.DatabaseType == DatabaseType.MySQL)
                        {
                            var countInfo = _productRepository.GetWithSql<long>(countSql);
                            searchResult.TotalSize = countInfo != null && countInfo.Any() ? countInfo.FirstOrDefault() : 0;
                        }
                    }
                    else
                    {
                        searchResult.TotalSize = searchResult.Results.Count();
                    }
                }
            }
        }
    }
}

//var spOutput = new SqlParameter
//{
//    ParameterName = "@ResultCount",
//    SqlDbType = System.Data.SqlDbType.BigInt,
//    Direction = System.Data.ParameterDirection.Output
//};


//string sql = StoredProcedure.SearchProductsV2(searchModel.Keyword, searchModel.Barcode, searchModel.CategoryID, searchModel.Size, searchModel.Page - 1);
//searchModel.Products = GetWithSql<Product>(sql, new object[] { spOutput });
//searchModel.Categories = _categoryService.GetCategories();
//searchModel.TotalSize = spOutput.Value.ToString().Int();
//searchModel.Success = true;