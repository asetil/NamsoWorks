using System;
using System.Web.Mvc;
using System.Web.WebPages;
using Aware.ECommerce.Search;
using Aware.Util;
using WebMarket.Admin.Helper;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace WebMarket.Admin.Models.ModelBinder
{
    public class OrderSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(OrderSearchParams))
            {
                var request = controllerContext.HttpContext.Request;
                var paymentType = request.GetValue("pt").Int();
                var startDate = request.GetValue("sdate").AsDateTime();
                var endDate = request.GetValue("edate").AsDateTime();
                var page = request.GetValue("page").Int(1);
                var size = request.GetValue("s").Int(10);
                var status = (OrderStatuses)request.GetValue("st").Int();
                var period = request.GetValue("period");

                var result = OrderSearchParams.Init(Statuses.None, page, size);
                result.PaymentType = paymentType;
                result.OrderStatus = status;
                result.IDs = request.GetIDArray("ids");
                result.StoreIDs = request.GetIDArray("sid");

                if (!string.IsNullOrEmpty(period))
                {
                    switch (period)
                    {
                        case "daily":
                            startDate = DateTime.Today; break;
                        case "weekly":
                            startDate = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1); break;
                        case "monthly":
                            startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); break;
                        case "yearly":
                            startDate = new DateTime(DateTime.Now.Year, 1, 1); break;
                    }
                    endDate = DateTime.Now;
                }

                result.StartDate = startDate;
                result.EndDate = endDate;
                return result;
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}