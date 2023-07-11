using System.Web;
using Aware.Util.Model;

namespace Aware.ECommerce.Manager
{
    public interface IUploadItemManager
    {
        Result UploadStoreItems(HttpRequestBase request, int storeID);
    }
}