using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using System;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using Aware.Regional;
using Aware.Util;
using Aware.Util.Log;
using Aware.Authenticate;

namespace Aware.ECommerce.Service
{
    public class StoreService : BaseService<Store>, IStoreService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IAddressService _addressService;
        private readonly ICacher _cacher;

        public StoreService(ISessionManager sessionManager, IRepository<Store> storeRepository, IAddressService addressService, ICacher cacher, ILogger logger)
            : base(storeRepository, logger)
        {
            _sessionManager = sessionManager;
            _addressService = addressService;
            _cacher = cacher;
        }

        #region SiteOperations

        public Store GetRegionStore(int storeID, bool includeDetail = false)
        {
            try
            {
                if (storeID > 0)
                {
                    var stores = GetRegionStores();
                    if (stores != null && stores.Any(i => i.ID == storeID))
                    {
                        var store = stores.FirstOrDefault(i => i.ID == storeID);
                        if (store != null && includeDetail)
                        {
                            var regionIDs = store.RegionInfo.Trim(',').Split(",").Select(i => i.Int()).ToList();
                            store.ServiceRegions = _addressService.GetRegionsWithIDs(regionIDs);
                        }
                        return store;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("StoreService > GetStoreDetail - Fail with ID:{0}", ex, storeID);
            }
            return null;
        }

        public List<Store> GetRegionStores()
        {
            var regionID = _sessionManager.GetCurrentRegion();
            var cacheKey = string.Format(Constants.CK_RegionStores, regionID);

            var result = _cacher.Get<List<Store>>(cacheKey);
            if (result == null)
            {
                var sql = SqlHelper.GetRegionStores(regionID);
                result = Repository.GetWithSql(sql).OrderByDescending(i => i.ID).ToList();
                _cacher.Add(cacheKey, result);
            }
            return result;
        }

        #endregion

        #region AdminOperations
        public Store GetCustomerStore(int customerID, int storeID)
        {
            if (customerID >0 && storeID > 0)
            {
                var stores = GetCustomerStores(customerID);
                return stores.FirstOrDefault(i => i.ID == storeID);
            }
            return null;
        }

        public List<Store> GetCustomerStores(int customerID, int page, out int totalSize)
        {
            if (customerID>0)
            {
                var result = GetCustomerStores(customerID);
                totalSize = result.Count();

                if (totalSize < Constants.STORE_PAGE_SIZE) { return result; }
                return result.Skip(Constants.STORE_PAGE_SIZE * (page - 1))
                             .Take(Constants.STORE_PAGE_SIZE).ToList();
            }

            totalSize = 0;
            return new List<Store>();
        }

        public List<Store> GetCustomerStores(int customerID)
        {
            if (customerID > 0)
            {
                var cacheKey = string.Format(Constants.CK_CustomerStores, customerID);
                var result = _cacher.Get<List<Store>>(cacheKey);
                if (result == null)
                {
                    result = Repository.Where(i => i.CustomerID == customerID).ToList();
                    _cacher.Add(cacheKey, result, 400);
                }
                return result;
            }
            return null;
        }

        public List<Store> GetStores(List<int> storeIDs)
        {
            if (storeIDs != null && storeIDs.Any())
            {
                return Repository.Where(i => storeIDs.Contains(i.ID)).ToList();
            }
            return null;
        }

        public List<StoreStatisticModel> GetStoreStatistics(int customerID)
        {
            var cacheKey = string.Format(Constants.CK_StoreStatistics, customerID);
            var result = _cacher.Get<List<StoreStatisticModel>>(cacheKey);
            if (result == null || !result.Any())
            {
                var sql = SqlHelper.GetStoreStatistics(customerID);
                result = Repository.GetWithSql<StoreStatisticModel>(sql, false).ToList();
                _cacher.Add(cacheKey, result.ToList(), 10);
            }
            return result;
        }

        protected override void OnBeforeUpdate(ref Store existing, Store model)
        {
            if (existing != null && model != null)
            {
                existing.Name = model.Name;
                existing.DisplayName = model.DisplayName;
                existing.Description = model.Description;
                existing.MinOrderAmount = model.MinOrderAmount;
                existing.Status = model.Status;
                existing.RegionInfo = model.RegionInfo;
            }
        }

        public bool SaveWorkTimeInfo(int customerID, int storeID, string workTimeInfo)
        {
            try
            {
                if (customerID>0 && storeID > 0 && !string.IsNullOrEmpty(workTimeInfo))
                {
                    var store = Get(storeID);
                    if (store != null && store.ID > 0)
                    {
                        store.WorkTimeInfo = workTimeInfo;
                        Repository.Update(store);
                        RefreshCache(customerID, store);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("StoreService > SaveWorkTimeInfo - Fail for ID:{0}, WorkTimeInfo : {1}", ex, storeID, workTimeInfo);
            }
            return false;
        }

        public void RefreshCache(int customerID, Store store)
        {
            if (customerID>0 && store != null)
            {
                var cacheKey = string.Format(Constants.CK_CustomerStores, customerID);
                var result = _cacher.Get<List<Store>>(cacheKey) ?? new List<Store>();
                result = result.Where(i => i.ID != store.ID).ToList();
                result.Add(store);
                _cacher.Add(cacheKey, result, 400);
            }
        }

        #endregion
    }
}