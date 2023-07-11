using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aware.ECommerce.Model;
using System.Web.Mvc;
using Aware.Authenticate.Model;
using Aware.ECommerce.Interface;
using Aware.Util;
using WebMarket.Models;
using Aware.Dependency;
using Aware.Util.Model;
using Aware.Util.View;
using HtmlHelper = System.Web.Mvc.HtmlHelper;
using Aware.ECommerce.Enums;

namespace WebMarket.Helper
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString ButtonFor(this HtmlHelper html, string value, string css = "btn-info", string iconCss = "check", string extraProperties = "")
        {
            var iconHtml = string.IsNullOrEmpty(iconCss) ? string.Empty : "<i class='fa fa-" + iconCss + "'></i> ";
            var button = string.Format("<button class='btn {0}' {3}>{1}{2}</button>", css, iconHtml, value, extraProperties);
            return MvcHtmlString.Create(button);
        }

        public static string ImagePath(this HtmlHelper html, string imagePath, string sizePath = "m", bool addDomain = false)
        {
            var imageName = imagePath.Split("/").LastOrDefault();
            imagePath = imagePath.Replace(imageName, string.Format("{0}/{1}", sizePath, imageName));

            var result = string.Format("{0}{1}", Config.ImageRepository, imagePath);
            if (addDomain && !(result.StartsWith("//") || result.StartsWith("http://") || result.StartsWith("https://")))
            {
                result = string.Format("{0}{1}", Aware.Util.Config.DomainUrl, result);
            }
            return result;
        }

        public static MvcHtmlString ImageFor(this HtmlHelper html, string directory, string imageName, string title = "", string cssClass = "", string alt = "")
        {
            string imageHtml;
            var imageRepository = Config.ImageRepository;

            alt = string.IsNullOrEmpty(alt) ? (string.IsNullOrEmpty(title) ? imageName : title) : alt;
            var classInfo = string.IsNullOrEmpty(cssClass) ? string.Empty : "class='" + cssClass + "'";
            if (string.IsNullOrEmpty(directory))
            {
                imageHtml = string.Format("<img src='{0}{1}' alt='{4}' title='{2}' {3}/>", imageRepository, imageName, title, classInfo, alt);
            }
            else
            {
                imageHtml = string.Format("<img src='{0}{1}/{2}' alt='{5}' title='{3}' {4}/>", imageRepository, directory, imageName, title, classInfo, alt);
            }
            return MvcHtmlString.Create(imageHtml);
        }

        public static MvcHtmlString LocalImage(this HtmlHelper html, string directory, string imageName, string title = "", string cssClass = "", string alt = "")
        {
            string imageHtml;
            var classInfo = string.IsNullOrEmpty(cssClass) ? string.Empty : "class='" + cssClass + "'";
            alt = string.IsNullOrEmpty(alt) ? (string.IsNullOrEmpty(title) ? imageName : title) : alt;

            if (string.IsNullOrEmpty(directory))
            {
                imageHtml = string.Format("<img src='{0}{1}' alt='{4}' title='{2}' {3}/>", Constants.LocalImageRepository, imageName, title, classInfo, alt);
            }
            else
            {
                imageHtml = string.Format("<img src='{0}{1}/{2}' alt='{5}' title='{3}' {4}/>", Constants.LocalImageRepository, directory, imageName, title, classInfo, alt);
            }
            return MvcHtmlString.Create(imageHtml);
        }

        public static Pricer Pricer(this HtmlHelper html, decimal price)
        {
            return new Pricer(price);
        }

        public static MvcHtmlString QuantityBox(this HtmlHelper htmlHelper, string id, string css, decimal quantity, MeasureUnits unit, string title = "")
        {
            var suffix = unit == MeasureUnits.Kg ? "kg" : (unit == MeasureUnits.Gram ? "gr" : "ad");
            var html = Selecto.DrawIncremental(id, css, quantity.DecString("0.#"), title, suffix);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DrawPageTitle(this HtmlHelper htmlHelper, string pageTitle)
        {
            var html = new StringBuilder();
            html.AppendFormat(@"<section class='page-section breadcrumbs'>
                            <div class='container'>
                                <div class='page-header'>
                                    <h1>{0}</h1>
                                </div>
                            </div>
                        </section>",pageTitle);
            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString PriceHtml(this HtmlHelper html, decimal price, string suffix = "")
        {
            var pricer = new Pricer(price);
            var priceHtml = string.Format("<span class='lp'>{0},</span><span class='rp'>{1} TL{2}</span>", pricer.Left, pricer.Right, suffix);
            return MvcHtmlString.Create(priceHtml);
        }

        public static bool IsLoggedIn(this HtmlHelper htmlHelper)
        {
            var currentUser = CurrentUser(htmlHelper);
            return currentUser != null && currentUser.ID > 0;
        }

        public static CustomPrincipal CurrentUser(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.User as CustomPrincipal ?? new CustomPrincipal();
        }

        public static string CssPath(this HtmlHelper htmlHelper, string css)
        {
            return string.Format("/resource/css/{0}", css);
            var theme = Config.CssTheme;
            switch (theme)
            {
                case "white":
                    return string.Format("/Resource/Content/twhite/{0}", css);
                default:
                    return string.Format("/Resource/Content/{0}", css);
            }
        }

        public static Item GetOrderStatus(this HtmlHelper htmlHelper, OrderStatuses status)
        {
            var result = Item.New(0, string.Empty);
            switch (status)
            {
                case OrderStatuses.WaitingCustomerApproval:
                    result = new Item(0, "Onayınız Bekleniyor", "fa-check");
                    break;
                case OrderStatuses.WaitingPayment:
                    result = new Item(10, "Ödeme Bekleniyor", "fa-money");
                    break;
                case OrderStatuses.WaitingApproval:
                    result = new Item(20, "Onay Bekleniyor", "fa-file-text-o");
                    break;
                case OrderStatuses.PreparingOrder:
                    result = new Item(40, "Hazırlanıyor", "fa-check");
                    break;
                case OrderStatuses.ShippingOrder:
                    result = new Item(65, "Kargoya Verildi", "fa-truck");
                    break;
                case OrderStatuses.DeliveredOrder:
                    result = new Item(100, "Teslim Edildi", "fa-dropbox");
                    break;
                case OrderStatuses.CancelledOrder:
                    result = new Item(100, "İptal Edildi", "fa-times");
                    break;
                case OrderStatuses.ReturnedOrder:
                    result = new Item(100, "İade Edildi", " fa-undo");
                    break;
            }

            result.OK = (status != OrderStatuses.CancelledOrder && status != OrderStatuses.ReturnedOrder);
            return result;
        }

        public static MvcHtmlString GetCommentRating(this HtmlHelper htmlHelper, decimal rating)
        {
            var html = new StringBuilder();
            html.Append("<span>");
            html.AppendFormat("<i class='fa fa-star {0}'></i>", rating >= 1 ? "active" : "");
            html.AppendFormat("<i class='fa fa-star {0}'></i>", rating >= 2 ? "active" : "");
            html.AppendFormat("<i class='fa fa-star {0}'></i>", rating >= 3 ? "active" : "");
            html.AppendFormat("<i class='fa fa-star {0}'></i>", rating >= 4 ? "active" : "");
            html.AppendFormat("<i class='fa fa-star {0}'></i>", rating >= 5 ? "active" : "");
            html.Append("</span>");
            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString GetSearchNavigation(this HtmlHelper html, List<int> selectedCategories)
        {
            var htmlResult = new StringBuilder();
            if (selectedCategories != null && selectedCategories.Any())
            {
                var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                var levelDictionary = new Dictionary<int, List<Category>>();
                foreach (int categoryID in selectedCategories)
                {
                    var hierarchy = categoryService.GetCategoryHierarchy(categoryID);
                    foreach (var item in hierarchy)
                    {
                        var result = string.Empty;
                        if (levelDictionary.ContainsKey(item.Level) && !levelDictionary[item.Level].Any(i => i.ID == item.ID))
                        {
                            var value = levelDictionary[item.Level];
                            value.Add(item);
                            levelDictionary[item.Level] = value;
                        }
                        else if (!levelDictionary.ContainsKey(item.Level))
                        {
                            levelDictionary.Add(item.Level, new List<Category>() { item });
                        }
                    }
                }

                foreach (var item in levelDictionary)
                {
                    htmlResult.AppendFormat(string.Join("<span>, </span>", item.Value.Select(i =>
                    {
                        return string.Format("<a href='/{0}-urunleri-{1}'>{2}</a>", i.Name.ToSeoUrl(), i.ID, i.Name);
                    })));
                    htmlResult.AppendFormat("<i class='fa fa-angle-right'></i>");
                }
            }
            else
            {
                htmlResult.Append("<a href='/urunler'>Ürünler</a> <i class='fa fa-angle-right'></i>");
            }

            return MvcHtmlString.Create(htmlResult.ToString());
        }
    }
}
