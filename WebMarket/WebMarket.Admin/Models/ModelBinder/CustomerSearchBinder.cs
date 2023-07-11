using System.Web.Mvc;
using Aware.Crm.Model;
using Aware.Util;
using Aware.Util.Enums;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Models.ModelBinder
{
    public class CustomerSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(CustomerSearchParams))
            {
                var request = controllerContext.HttpContext.Request;
                var keyword = request.GetValue("q");
                var ids = request.GetIDArray("ids");
                var page = request.GetValue("page").Int(1);
                var size = request.GetValue("s").Int(25);
                var status = (Statuses)request.GetValue("st").Int();

                var result = new CustomerSearchParams
                {
                    Keyword = keyword,
                    Status = status == Statuses.None ? default(Statuses) : status,
                    IDs = ids
                };

                result.SetPaging(page, size).WithCount();
                return result;
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }
    }
}