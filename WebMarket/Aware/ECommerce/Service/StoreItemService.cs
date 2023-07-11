using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.ECommerce.Model;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Search;
using Aware.ECommerce.Util;
using Aware.Search;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class StoreItemService : IStoreItemService
    {
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly IRepository<StoreItem> _itemRepository;
        private readonly IApplication _application;

        public StoreItemService(ICategoryService categoryService, IStoreService storeService, IRepository<StoreItem> itemRepository, IProductService productService, IApplication application)
        {
            _categoryService = categoryService;
            _storeService = storeService;
            _itemRepository = itemRepository;
            _productService = productService;
            _application = application;

        }

        public StoreItemDetailModel GetDetailModel(int customerID, int itemID)
        {
            if (customerID>0)
            {
                var item = itemID > 0 ? Get(customerID, itemID) : new StoreItem()
                {
                    Store = new Store(),
                    Product = new Product(),
                    Status = Statuses.Active
                };

                return new StoreItemDetailModel()
                {
                    StoreItem = item,
                    StoreList = _storeService.GetCustomerStores(customerID),
                    StatusList = _application.Lookup.GetLookups(LookupType.Status),
                    YesNoList = _application.Lookup.GetLookups(LookupType.YesNoOptions),
                };
            }
            return null;
        }

        public StoreItem Get(int customerID, int itemID)
        {
            if (customerID>0 && itemID > 0)
            {
                var result = _itemRepository.Get(itemID);
                if (result != null && result.ID == itemID)
                {
                    result.Store = _storeService.GetCustomerStore(customerID, result.StoreID);
                    result.Product = _productService.Get(result.ProductID);
                    return result;
                }
            }
            return null;
        }

        public StoreItem GetStoreItem(int storeID, int productID)
        {
            if (storeID > 0 && productID > 0)
            {
                return _itemRepository.First(i => i.StoreID == storeID && i.ProductID == productID);
            }
            return null;
        }

        public StoreItemListModel GetItems(ItemSearchParams searchParams, int customerID = 0)
        {
            try
            {
                var result = new StoreItemListModel
                {
                    Categories = _categoryService.GetCategories(),
                };

                if (customerID > 0)
                {
                    result.Stores = _storeService.GetCustomerStores(customerID);
                    if (searchParams.StoreIDs == null || !searchParams.StoreIDs.Any())
                    {
                        searchParams.StoreIDs = result.Stores.Select(i => i.ID).ToList();
                    }
                }
                else if (searchParams.StoreIDs != null && searchParams.StoreIDs.Any())
                {
                    result.Stores = _storeService.GetStores(searchParams.StoreIDs);
                }

                if (result.Stores == null || !result.Stores.Any())
                {
                    result.SearchResult = new SearchResult<StoreItem> { SearchParams = searchParams };
                    return result;
                }

                searchParams.WithCount();
                var searchResult = _itemRepository.Find(searchParams);
                if (searchResult.HasResult)
                {
                    var productIDs = searchResult.Results.Select(i => i.ProductID);
                    var productSearchParams = new ProductSearchParams().SetPaging(1, 999);
                    productSearchParams.IDs = productIDs;

                    var productSearchResult = _productService.SearchProducts(productSearchParams as ProductSearchParams);
                    searchResult.Results = searchResult.Results.Select(i =>
                    {
                        i.Product = productSearchResult.Results.FirstOrDefault(p => p.ID == i.ProductID);
                        i.Store = result.Stores.FirstOrDefault(s => s.ID == i.StoreID);
                        return i;
                    }).ToList();
                }

                searchResult.Success = true;
                result.SearchResult = searchResult;
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("ParoductService > Search - Fail", ex);
            }
            return new StoreItemListModel();
        }

        public List<StoreItem> GetModifiedItems(DateTime? start)
        {
            if (start.HasValue && start.Value > DateTime.MinValue)
            {
                var modifiedItems = _itemRepository.Where(i => i.Status == Statuses.Active
                        && i.Product.Status == Statuses.Active
                        && (i.DateModified >= start.Value || i.Product.DateModified >= start.Value)).ToList();
                return modifiedItems;
            }
            return _itemRepository.Where(i => i.Status == Statuses.Active && i.Product.Status == Statuses.Active).ToList();
        }

        public List<StoreItem> GetOpportunityItems(int regionID)
        {
            try
            {
                var sql = SqlHelper.GetOpportunityItems(regionID, 24);
                var items = _itemRepository.GetWithSql(sql).ToList();
                return items;
            }
            catch (Exception ex)
            {
                _application.Log.Error("ItemService > GetOpportunityItems - Failed", ex);
            }
            return null;
        }

        public Result Save(StoreItem model)
        {
            try
            {
                if (model == null || model.StoreID == 0 || model.ProductID == 0) { Result.Error(Resource.General_Error); }
                var existItem = _itemRepository.Where(i => i.StoreID == model.StoreID && i.ProductID == model.ProductID && i.ID != model.ID).First();
                if (existItem != null && existItem.ID > 0)
                {
                    var result = Result.Error(Resource.Store_AlreadyHasItem, model);
                    return result;
                }

                if (model.ID > 0)
                {
                    var item = _itemRepository.Where(i => i.ID == model.ID).First();
                    if (item != null && item.ID > 0)
                    {
                        Mapper.Map(ref item, model);
                        _itemRepository.Update(item);
                        return Result.Success(item, Resource.General_Success);
                    }
                    _application.Log.Error(string.Format("Product > Save - Not Found with  with : {0}!", model.ID), null);
                }
                else
                {
                    model.DateModified = DateTime.Now;
                    _itemRepository.Add(model);
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                var id = model != null ? model.ID : 0;
                _application.Log.Error("Product > Save - Fail with : {0}!", ex, id);
            }
            return Result.Error(Resource.General_Error, model);
        }

        public Result QuickUpdateItem(int itemID, decimal price, decimal listPrice, decimal stock)
        {
            try
            {
                if (itemID > 0)
                {
                    var item = _itemRepository.Get(itemID);
                    if (item != null && item.ID == itemID)
                    {
                        item.Stock = stock;
                        item.SalesPrice = price;
                        item.ListPrice = listPrice;
                        item.DateModified = DateTime.Now;
                        _itemRepository.Update(item);
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("Product > RefreshItemStock - Fail with itemID:{0}, stock:{1}!", ex, itemID, stock);
            }
            return Result.Error(Resource.General_Error);
        }


        public void LoadItems(ref List<Product> products, List<Store> stores = null)
        {
            if (products != null && products.Any())
            {
                stores = stores ?? _storeService.GetRegionStores();
                var storeIDs = stores.Select(s => s.ID);
                var productIDs = products.Select(i => i.ID);

                var items = _itemRepository.Where(i => productIDs.Contains(i.ProductID) && storeIDs.Contains(i.StoreID)).ToList();
                items = items.Select(i =>
                {
                    i.Store = stores.FirstOrDefault(s => s.ID == i.StoreID);
                    return i;
                }).ToList();

                products = products.Select(p =>
                {
                    p.Items = items.Where(item => item.ProductID == p.ID);
                    return p;
                }).ToList();
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