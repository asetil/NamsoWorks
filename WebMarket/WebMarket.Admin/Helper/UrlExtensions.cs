using System.Web.Mvc;
using System.Web.Routing;
using Aware.Util;
namespace WebMarket.Admin.Helper
{
    public static class UrlExtensions
    {
        public static string StoreItems(this UrlHelper url,int sid,string sname,string filter)
        {
            return url.Action("Index", "StoreItem", new { storeID = sid, storename = sname.ToSeoUrl(), filter = filter });
        }

        public static string PropertyDetail(this UrlHelper url, string name, int id)
        {
            return url.Action("Detail", "Property", new { name = name.ToSeoUrl(), id = id });
        }
    }
}