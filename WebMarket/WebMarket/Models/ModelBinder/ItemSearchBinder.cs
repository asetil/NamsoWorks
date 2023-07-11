using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aware.ECommerce.Search;
using Aware.Util;
using WebMarket.Helper;

namespace WebMarket.Models.ModelBinder
{
    public class ItemSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ItemSearchParams))
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;

                var keyword = request.GetValue("q");
                var categoryIDs = GetIDList(request.GetValue("cid"));
                var storeIDs = GetIDList(request.GetValue("sid"));
                var page = request.GetValue("page").Int(1);
                var size = request.GetValue("size").Int(36);

                var result = new ItemSearchParams(keyword, page, size);
                result.WithStore(null, storeIDs).WithCategory(null, categoryIDs);
                result.Price = request.GetValue("price");
                result.Stock = request.GetValue("stock");
                result.Rating = request.GetValue("rating");
                result.PropertyIDs = GetIDList(request.GetValue("pid"));
                result.OrderBy = request.GetValue("sirala").Int();

                var routeValues = controllerContext.RouteData.Values;
                object outValue;
                if (categoryIDs == null)
                {
                    routeValues.TryGetValue("cid", out outValue);
                    if (outValue != null) { result.WithCategory(outValue.ToString().Int()); }
                }

                if (storeIDs == null)
                {
                    routeValues.TryGetValue("sid", out outValue);
                    if (outValue != null) { result.WithStore(outValue.ToString().Int()); }
                }
                return result;

                //// call the default model binder this new binding context
                //return base.BindModel(controllerContext, newBindingContext);
            }
            return base.BindModel(controllerContext, bindingContext);
        }

        private List<int> GetIDList(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Split(',').Where(i => !string.IsNullOrEmpty(i)).Select(i => i.Int()).ToList();
            }
            return null;
        }
    }
}