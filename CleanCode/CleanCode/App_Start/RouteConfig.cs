using CleanCode.Helper;
using System.Web.Mvc;
using System.Web.Routing;

namespace CleanCode
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            AddRoute(ref routes, RouteNames.Home, "anasayfa", "Home", "Index");
            AddRoute(ref routes, RouteNames.Search, "ara", "Home", "Index");
            AddRoute(ref routes, RouteNames.EntryDetail, "makale/{name}/{id}", "Entry", "Detail");
            AddRoute(ref routes, RouteNames.CategoryDetail, "{name}-makaleleri/{categoryID}", "Home", "Index");
            AddRoute(ref routes, RouteNames.AboutUs, "hakkimda", "Home", "AboutUs");
            AddRoute(ref routes, RouteNames.AngularJSRoute, "angular-demo/{name}", "Angular", "Index");
            AddRoute(ref routes, RouteNames.SiteMapIndex, "sitemap.xml", "SiteMap", "Index");

            AddRoute(ref routes, RouteNames.AdminDashboard, "yonetim", "Entry", "ManageList");
            AddRoute(ref routes, RouteNames.EntryManageList, "yonetim/makaleler", "Entry", "ManageList");
            AddRoute(ref routes, RouteNames.CacheManagement, "yonetim/cache", "Home", "CacheManagement");
            AddRoute(ref routes, RouteNames.EntryManageDetail, "yonetim/makale/{name}/{id}", "Entry", "ManageDetail");
            AddRoute(ref routes, RouteNames.CategoryManageList, "yonetim/kategoriler", "Category", "Index");
            AddRoute(ref routes, RouteNames.AuthorList, "yonetim/yazarlar", "Author", "Index");
            AddRoute(ref routes, RouteNames.AuthorDetail, "yonetim/yazar/{name}/{id}", "Author", "Detail");
            AddRoute(ref routes, RouteNames.GalleryManagement, "yonetim/galeri", "Gallery", "Index");

            AddRoute(ref routes, RouteNames.UserLogin, "uye-girisi", "User", "Login");
            AddRoute(ref routes, RouteNames.UserLogout, "oturumu-kapat", "User", "Logout");
            AddRoute(ref routes, RouteNames.UnAuthorizedRoute, "yetkiniz-yok", "Error", "UnAuthorized");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        private static void AddRoute(ref RouteCollection routes, string name, string url, string controller, string action)
        {
            routes.MapRoute(
               name: name,
               url: url,
               defaults: new { controller = controller, action = action, id = UrlParameter.Optional }
            );
        }
    }
}