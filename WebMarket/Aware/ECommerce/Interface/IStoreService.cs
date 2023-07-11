using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.Util;

namespace Aware.ECommerce.Interface
{
    public interface IStoreService : IBaseService<Store>
    {
        Store GetRegionStore(int storeID, bool includeDetail = false);
        List<Store> GetRegionStores();

        Store GetCustomerStore(int customerID, int storeID);
        List<Store> GetCustomerStores(int customerID, int page, out int totalSize);
        List<Store> GetCustomerStores(int customerID);
        List<Store> GetStores(List<int> storeIDs);
        List<StoreStatisticModel> GetStoreStatistics(int customerID);
        
        bool SaveWorkTimeInfo(int customerID, int storeID, string workTimeInfo);
        void RefreshCache(int customerID, Store store);
    }
}