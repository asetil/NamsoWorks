using System.Web.Mvc;
using System.Web.Routing;
using WebMarket.Helper;

namespace WebMarket
{
    public class RouteConfig
    {
        private static RouteCollection _routes;
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("*.woff2");
            _routes = routes;

            AddRoute(RouteNames.HomeRoute, "anasayfa", "Home", "Index");
            AddRoute(RouteNames.AboutUsRoute, "hakkimizda", "Home", "AboutUs");
            AddRoute(RouteNames.SssRoute, "yardim-destek", "Home", "Sss");
            AddRoute(RouteNames.MembershipAggreementRoute, "uyelik-sozlesmesi", "Home", "MembershipAggreement");
            AddRoute(RouteNames.ContactUsRoute, "iletisim", "Home", "Contact");
            AddRoute(RouteNames.SiteMapIndex, "sitemap.xml", "SiteMap", "Index");
            AddRoute(RouteNames.SelectRegionRoute, "semt-sec", "Helper", "SelectRegion");
            AddRoute(RouteNames.ChangeLanguage, "dil-degistir/{lang}", "Helper", "ChangeLanguage");
            AddRoute(RouteNames.DynamicPageRoute, "dynamic/{pname}/{pageID}", "Helper", "DynamicPage");
                     
            AddRoute(RouteNames.NotFound, "sayfa-bulunamadi", "Error", "NotFound");
            AddRoute(RouteNames.BrowserNotSupportedRoute, "desteklenmeyen-tarayici", "Error", "BrowserNotSupported");
            AddRoute(RouteNames.LoginRoute, "uye-girisi", "Account", "Index");
            AddRoute(RouteNames.LogoutRoute, "oturumu-sonlandir", "Account", "Logout");
            AddRoute(RouteNames.AccountDetailRoute, "bilgilerim", "Account", "Detail");
            AddRoute(RouteNames.ChangePasswordRoute, "sifre-degistir", "Account", "ChangePassword");
            AddRoute(RouteNames.RegisterRoute, "uye-ol", "Account", "Register");
            AddRoute(RouteNames.ActivationRoute, "aktivasyon/{id}", "Account", "Activation");
           
            AddRoute(RouteNames.StoreListRoute, "marketler", "Store", "Index");
            AddRoute(RouteNames.StoreDetailRoute, "market/{name}-{id}", "Store", "Detail");
            AddRoute(RouteNames.MyBasketRoute, "sepetim", "Basket", "Index");
            AddRoute(RouteNames.MyFavorites, "favorilerim", "Product", "MyFavorites");
            AddRoute(RouteNames.MyOrdersRoute, "siparislerim", "Order", "MyOrders");
            AddRoute(RouteNames.OrderRoute, "siparis", "Order", "Index");
            AddRoute(RouteNames.OrderPaymentRoute, "siparis-odeme-{id}", "Order", "Payment");
            AddRoute(RouteNames.OrderPaymentResultRoute, "siparis-odeme-sonucu-{uniqueOrderID}/{success}", "Order", "OnlinePaymentResult");
            AddRoute(RouteNames.OrderApproveRoute, "siparis-onay", "Order", "Approval");
            AddRoute(RouteNames.OrderDetailRoute, "siparis-detay-{id}", "Order", "Detail");
                     
            AddRoute(RouteNames.CampaignListRoute, "kampanyalar", "Campaign", "Index");
            AddRoute(RouteNames.CampaignDetailRoute, "kampanya-detay/{name}/{id}", "Campaign", "Detail");
                     
            AddRoute(RouteNames.ProductListNamedRoute, "{sname}-{cname}-urunleri-{sid}-{cid}", "Product", "Index");
            AddRoute(RouteNames.ProductListStoreRoute, "{sname}-market-urunleri-{sid}", "Product", "Index");
            AddRoute(RouteNames.ProductListCategoryRoute, "{cname}-urunleri-{cid}", "Product", "Index");
            AddRoute(RouteNames.ProductListRoute, "urunler", "Product", "Index");
            AddRoute(RouteNames.ProductDetailRoute, "urun-detay/{name}-{id}", "Product", "Detail");
            AddRoute(RouteNames.ItemDetailUrl, "urun/{productname}/{id}", "Product", "Detail");
            AddRoute(RouteNames.ProductCompareRoute, "urun-karsilastir", "Product", "CompareProducts");
            AddRoute(RouteNames.ProductCommentsRoute,"urun-yorum/{name}-{id}","Product","Comments");

            AddRoute("Default", "{controller}/{action}/{id}", "Home", "Index");
        }

        private static void AddRoute(string name, string url, string controller, string action)
        {
            _routes.MapRoute(name, url, new { controller, action, id = UrlParameter.Optional });
        }
    }
}