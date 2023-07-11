using CleanFramework.Business.Model;
using System.Web.Mvc;
using Aware.Util;

namespace CleanCode.Helper
{
    public static class HtmlExtensions
    {
        private static string PAGE_DESCRIPTION = "Yazılım Notları. Asp.NET, MVC, C-Sharp, WCF Servis konularında yazılmış makaleler. Backend ve Frontend kategorisinde yazılım ipuçları. MsSQL veritabanı hakkında yazılmış notlar ve daha fazlası.";

        public static MvcHtmlString ButtonFor(this HtmlHelper html, string value, string css = "btn-info", string iconCss = "check", string extraProperties = "")
        {
            var iconHtml = string.IsNullOrEmpty(iconCss) ? string.Empty : "<i class='fa fa-" + iconCss + "'></i> ";
            var button = string.Format("<button class='btn {0}' {3}>{1}{2}</button>", css, iconHtml, value, extraProperties);
            return MvcHtmlString.Create(button);
        }

        public static SeoModel ToSeoModel(this HtmlHelper html, Entry entry, string pageUrl)
        {
            var result = new SeoModel("Makale Detay", string.Empty);
            if (entry != null)
            {
                result.Title = entry.Name.Short(66);
                result.Description = entry.Summary.Short(150);
                result.Keywords = entry.Keywords;
                result.ContentType = "article";
                result.PageUrl = pageUrl;
            }
            return result;
        }

        public static SeoModel GetHomePageSeoModel(this HtmlHelper html,string title)
        {
            title = string.IsNullOrEmpty(title) ? "Anasayfa" : title;
            var result = new SeoModel(title.Capitalize(), PAGE_DESCRIPTION)
            {
                ContentType = "article",
                PageUrl = string.Format("{0}/anasayfa", Config.DomainUrl)
            };
            return result;
        }

        //public static bool IsLoggedIn(this HtmlHelper htmlHelper)
        //{
        //    var currentUser = CurrentUser(htmlHelper);
        //    return currentUser != null && currentUser.ID > 0;
        //}
    }
}