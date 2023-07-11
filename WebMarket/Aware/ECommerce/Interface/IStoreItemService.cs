using System.Collections.Generic;
using Aware.ECommerce.Model;
using System;
using Aware.ECommerce.Search;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface IStoreItemService
    {
        StoreItemDetailModel GetDetailModel(int customerID, int itemID);
        StoreItem Get(int customerID, int itemID);
        StoreItem GetStoreItem(int storeID,int productID);
        StoreItemListModel GetItems(ItemSearchParams searchParams, int customerID = 0);
        List<StoreItem> GetModifiedItems(DateTime? start);
        List<StoreItem> GetOpportunityItems(int regionID);
        Result Save(StoreItem model);
        Result QuickUpdateItem(int itemID, decimal price, decimal listPrice, decimal stock);
        void LoadItems(ref List<Product> products, List<Store> stores = null);
    }
}