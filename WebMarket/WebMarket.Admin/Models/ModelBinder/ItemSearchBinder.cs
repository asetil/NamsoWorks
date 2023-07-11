using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce.Search;
using Aware.Util;
using WebMarket.Admin.Helper;
using Aware.Util.Enums;

namespace WebMarket.Admin.Models.ModelBinder
{
    public class ItemSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ItemSearchParams))
            {
                var request = controllerContext.HttpContext.Request;
                var keyword = request.GetValue("q");
                var barcode = request.GetValue("b");
                var categoryIDs = request.GetIDArray("cid");
                var storeIDs = request.GetIDArray("sid");
                var ids = request.GetIDArray("ids");
                var page = request.GetValue("page").Int(1);
                var size = request.GetValue("s").Int(25);
                var status = (Statuses)request.GetValue("st").Int();

                if (storeIDs==null || !storeIDs.Any())
                {
                    object outValue;
                    var routeValues = controllerContext.RouteData.Values;
                    routeValues.TryGetValue("storeID", out outValue);
                    if (outValue != null)
                    {
                        storeIDs=new List<int>(){ outValue.ToString().Int() };
                    }
                }

                var result = new ItemSearchParams(keyword, page, size, barcode)
                                    .WithStore(null, storeIDs)
                                    .WithCategory(null, categoryIDs);

                result.StoreIDs = storeIDs;
                result.Stock = request.GetValue("stock").Replace(",", ":");
                result.Price = request.GetValue("price").Replace(",", ":");
                result.Status = status == Statuses.None ? default(Statuses) : status;
                result.IDs = ids;
                result.IncludeUnlimitedStock = request.GetValue("ustock").Int() == 1;
                result.Fields = "Product";
                result.OnlyForSale= request.GetValue("forsale").Int()==1;
                return result;
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }
    }
}