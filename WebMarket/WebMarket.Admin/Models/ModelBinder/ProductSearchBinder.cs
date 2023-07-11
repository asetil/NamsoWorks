using System.Web.Mvc;
using Aware.ECommerce.Search;
using Aware.Util;
using WebMarket.Admin.Helper;
using Aware.Util.Enums;

namespace WebMarket.Admin.Models.ModelBinder
{
    public class ProductSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ProductSearchParams))
            {
                var request = controllerContext.HttpContext.Request;
                var keyword = request.GetValue("q");
                var barcode = request.GetValue("b");
                var page = request.GetValue("page").Int(1);
                var size = request.GetValue("s").Int(25);

                var result = new ProductSearchParams(keyword, page, size,barcode);
                result.WithCategory(request.GetValue("cid").Int());
                result.Status = (Statuses)request.GetValue("st").Int();
                result.IDs = request.GetIDArray("ids");
                result.WithCount();
                return result;
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}