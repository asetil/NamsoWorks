using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Aware.Dependency;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Util;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Helper
{
    public static class HtmlExtensions
    {
        public static string ImageRepository = "/resource/img/";
        public static MvcHtmlString ImageFor(this HtmlHelper html, string directory, string imageName, string title = "", string cssClass = "", string alt = "", bool random = false)
        {
            string imageHtml = string.Empty;
            alt = string.IsNullOrEmpty(alt) ? imageName : alt;
            string classInfo = string.IsNullOrEmpty(cssClass) ? string.Empty : "class='" + cssClass + "'";
            string rnd = string.Empty;
            if (random)
            {
                var r = new Random().Next();
                rnd = string.Format("?r={0}", r);
            }

            if (string.IsNullOrEmpty(directory))
            {
                imageHtml = string.Format("<img src='{0}{1}{5}' alt='{4}' title='{2}' {3}/>", ImageRepository, imageName, title, classInfo, alt, rnd);
            }
            else
            {
                imageHtml = string.Format("<img src='{0}{1}/{2}{6}' alt='{5}' title='{3}' {4}/>", ImageRepository, directory, imageName, title, classInfo, alt, rnd);
            }
            return MvcHtmlString.Create(imageHtml);
        }
        public static MvcHtmlString ButtonFor(this HtmlHelper html, string value, string css = "btn-success", string iconCss = "check", string extraProperties = "")
        {
            var iconHtml = string.IsNullOrEmpty(iconCss) ? string.Empty : "<i class='fa fa-" + iconCss + "'></i> ";
            var button = string.Format("<button class='btn {0}' {3}>{1}{2}</button>", css, iconHtml, value, extraProperties);
            return MvcHtmlString.Create(button);
        }

        public static MvcHtmlString PagingInfo<T>(this HtmlHelper html, SearchResult<T> searchResult, string alias) where T : class
        {
            if (searchResult != null && searchResult.SearchParams!=null && (searchResult.TotalSize /searchResult.SearchParams.Size)>1)
            {
                var lowerBound = (searchResult.SearchParams.Page - 1) * searchResult.SearchParams.Size;
                var upperBound = lowerBound + searchResult.SearchParams.Size;

                var htmlString = string.Format("<h5 style='float:left;'><span><b>{0}</b> {1} <b>{2}-{3}</b> arasını görüntülemektesiniz.</span></h5>", searchResult.TotalSize, alias, lowerBound, Math.Min(searchResult.TotalSize, upperBound));
                return MvcHtmlString.Create(htmlString);
            }
            return MvcHtmlString.Create(string.Empty);
        }

        public static string LookupValue(this HtmlHelper html, List<Lookup> lookupList, int lookupID,string defaultValue="")
        {
            if (lookupList != null && lookupList.Any() && lookupID > 0)
            {
                var lookup = lookupList.FirstOrDefault(l => l.Value == lookupID);
                if (lookup != null) { return lookup.Name; }
            }
            return defaultValue;
        }

        public static List<Lookup> GetStatusList(this HtmlHelper html)
        {
            var lookupManager = WindsorBootstrapper.Resolve<ILookupManager>();
            return lookupManager.GetLookups(LookupType.Status);
        }

        public static MvcHtmlString StatusFor(this HtmlHelper html, Statuses status)
        {
            var st = "times";
            var css = "fail";
            var title = "Pasif";

            switch (status)
            {
                case Statuses.Active:
                    st = "check"; css = ""; title = "Aktif"; break;
                case Statuses.WaitingActivation:
                    st = "user"; css = "warn"; title = "Aktivasyon Bekleniyor"; break;
                case Statuses.Deleted:
                    st = "trash"; css = "fail"; title = "Silindi"; break;
            }
            var htmlString = string.Format("<i class='fa fa-{0} status-icon {1}' title='{2}'></i>", st, css, title);
            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString StatusFor(this HtmlHelper html, CommentStatus status)
        {
            var st = "times";
            var css = "fail";
            var title = "Onay Bekliyor";

            switch (status)
            {
                case CommentStatus.Approved:
                    st = "check"; css = ""; title = "Onaylandı"; break;
                case CommentStatus.WaitingApproval:
                    st = "comments"; css = "warn"; title = "Onay Bekliyor"; break;
                case CommentStatus.Rejected:
                    st = "remove"; css = "fail"; title = "Reddedildi"; break;
            }
            var htmlString = string.Format("<i class='fa fa-{0} status-icon {1}' title='{2}'></i>", st, css, title);
            return MvcHtmlString.Create(htmlString);
        }

        public static string OrderStatusCss(this HtmlHelper html, OrderStatuses status)
        {
            switch (status)
            {
                case OrderStatuses.WaitingCustomerApproval:
                    return "text-gray";
                case OrderStatuses.WaitingPayment:
                    return "text-gray";
                case OrderStatuses.WaitingApproval:
                    return "text-teal";
                case OrderStatuses.PreparingOrder:
                    return "text-aqua";
                case OrderStatuses.ShippingOrder:
                    return "text-lime";
                case OrderStatuses.DeliveredOrder:
                    return "text-green";
                case OrderStatuses.CancelledOrder:
                    return "text-fuchsia";
                case OrderStatuses.ReturnedOrder:
                    return "text-maroon";
            }
            return "text-black";
        }

        public static MvcHtmlString ToPrice(this HtmlHelper html, decimal value, string format = "#,##0.00")
        {
            var htmlString = string.Format("{0} {1}", value.ToString(format),Constants.Currency);
            return MvcHtmlString.Create(htmlString);
        }

        public static StringBuilder GetSubCategoryTree(IEnumerable<Category> categoryList, bool isSuper)
        {
            var subList = new StringBuilder();
            if (categoryList != null && categoryList.Any())
            {
                var ind = 0;
                var count = categoryList.Count();

                foreach (var item in categoryList)
                {
                    subList.AppendFormat("<div data-category-id='{0}' class='category level{1}'>{2}", item.ID, item.Level - 1, item.Name.Short(24));
                    if (isSuper)
                    {
                        if (ind < count - 1) { subList.Append("<i class='fa fa-chevron-down' data-direction='1'></i>"); }
                        if (ind != 0) { subList.Append("<i class='fa fa-chevron-up' data-direction='-1'></i>"); }
                        ind++;
                    }
                    subList.Append("</div>");
                }
            }
            return subList;
        }

        public static string DiscountSuffix(this HtmlHelper html, DiscountType discountType)
        {
            if (discountType == DiscountType.Rate || discountType == DiscountType.CouponAsRate || discountType == DiscountType.Shipping)
            {
                return "%";
            }
            return "TL";
        }

        public static KeyValuePair<string, string> GetRange(this HtmlHelper html, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var rangeString = value.Trim().Replace("[", "").Replace("]", "").Split(",").FirstOrDefault();
                if (!string.IsNullOrEmpty(rangeString))
                {
                    var ranges = rangeString.Split(":");
                    var from = ranges[0];
                    var to = ranges.Length>1 ? ranges[1] : string.Empty;
                    if (!string.IsNullOrEmpty(to) && to.Int() < from.Int()) { to = string.Empty; }
                    return new KeyValuePair<string, string>(from, to);
                }
            }
            return new KeyValuePair<string, string>("", "");
        }
    }
}
