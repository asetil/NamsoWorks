using System.Web.Mvc;
using Aware.Util;

namespace WebMarket.Helper
{
    public static class UrlExtensions
    {
        public static string MyBasket(this UrlHelper url)
        {
            return url.Action("Index", "Basket");
        }

        public static string StoreDetail(this UrlHelper url, int storeID,string storeName)
        {
            return url.Action("Detail", "Store", new {id = storeID, name = storeName.ToSeoUrl()});
        }

        public static string StoreItemDetail(this UrlHelper url, int sID, string storeName,int productID, string productName)
        {
            var nameValue = string.Format("{0}-{1}", storeName.ToSeoUrl(), productName.ToSeoUrl()).Trim('-');
            return url.Action("Detail", "Product", new { id = productID, storeID = sID,name = nameValue });
        }

        public static string CategoryItemList(this UrlHelper url, int categoryID, string categoryName)
        {
            return url.Action("Index", "Product", new { cid=categoryID,cname=categoryName.ToSeoUrl() });
        }

        public static string ProductList(this UrlHelper url, int storeID, string storeName,int categoryID,string categoryName)
        {
            return url.Action("Index", "Product", new { sid = storeID, sname = storeName.ToSeoUrl(), cid = storeID, cname = storeName.ToSeoUrl() });
        }

        public static string CampaignDetail(this UrlHelper url, string campaignName, int campaignID)
        {
            return url.Action("Detail", "Campaign", new { id = campaignID, name = campaignName.ToSeoUrl()});
        }

        public static string ProductDetail(this UrlHelper url, string productName, string productID)
        {
            return url.Action("Detail", "Product", new { id = productID, name = productName.ToSeoUrl() });
        }
    }
}