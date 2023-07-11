using System.Web.Mvc;
using System.Web.Routing;
using WebMarket.Admin.Helper;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace WebMarket.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(RouteNames.CustomerSearchRoute, "firmalar", defaults: new { controller = "Customer", action = "Search"});
            routes.MapRoute(RouteNames.CustomerDetailRoute, "firma-detay/{name}/{id}", defaults: new { controller = "Customer", action = "Detail"});
            routes.MapRoute(RouteNames.CustomerUsersRoute, "{name}/kullanicilar/{customerID}", defaults: new { controller = "User", action = "UserList" });
            routes.MapRoute(RouteNames.CustomerOrdersRoute, "{name}/siparisler/{customerID}", defaults: new { controller = "Order", action = "Index" });
            routes.MapRoute(RouteNames.CustomerStoresRoute, "{name}/marketler/{customerID}", new { controller = "Store", action = "Index" });

            routes.MapRoute(RouteNames.UserListRoute, "kullanicilar", defaults: new { controller = "User", action = "UserList" });
            routes.MapRoute(RouteNames.StoreListRoute, "magazalar", new { controller = "Store", action = "Index" });
            routes.MapRoute(RouteNames.MarketListRoute, "marketler", new { controller = "Store", action = "Index" });
            routes.MapRoute(RouteNames.StoreDetail, "market-detay/{storename}/{id}", new { controller = "Store", action = "Detail" });

            routes.MapRoute(RouteNames.SiteSettingsRoute, "site-ayarlari", defaults: new { controller = "Common", action = "SiteSettings" });
            routes.MapRoute(RouteNames.SlidersRoute, "slider-ogeleri", defaults: new { controller = "Management", action = "SliderItems", type = (int)SliderType.Main });

            routes.MapRoute(RouteNames.BrandsRoute, "marka-yonetimi", defaults: new { controller = "Brand", action = "Index" });
            routes.MapRoute(RouteNames.UserPermissionsRoute, "kullanici-izinleri", defaults: new { controller = "Common", action = "SimpleItems", itemType = (int)ItemType.UserPermissions });
            routes.MapRoute(RouteNames.UserDetail, "kullanici-detay-{userID}", defaults: new { controller = "User", action = "UserDetail" });
            routes.MapRoute(RouteNames.MyInfoRoute, "bilgilerim", defaults: new { controller = "User", action = "MyInfo" });

            routes.MapRoute(RouteNames.TasksRoute, "planlanmis-gorevler", defaults: new { controller = "Task", action = "Index" });
            routes.MapRoute(RouteNames.TaskDetailRoute, "gorev/{name}/{id}", defaults: new { controller = "Task", action = "Detail" });
            routes.MapRoute(RouteNames.AuthorityDefinitionRoute, "yetkiler", defaults: new { controller = "Common", action = "AuthorityDefinition" });
            routes.MapRoute(RouteNames.CommentListRoute, "yorum-yonetimi", defaults: new { controller = "Property", action = "CommentManagement" });
            routes.MapRoute(RouteNames.ShippingMethodsRoute, "kargo-yonetimi", defaults: new { controller = "Order", action = "ShippingMethods" });
            routes.MapRoute(RouteNames.ShippingMethodDetailRoute, "kargo-detay/{id}", defaults: new { controller = "Order", action = "ShippingMethod" });
            routes.MapRoute(RouteNames.PropertyManagementRoute, "ozellik-yonetimi", defaults: new { controller = "Property", action = "Index" });
            routes.MapRoute(RouteNames.PropertyDetailRoute, "ozellik-detay/{name}-{id}", defaults: new { controller = "Property", action = "Detail" });
            routes.MapRoute(RouteNames.VariantPropertiesRoute, "varyant-ozellikler", new { controller = "Variant", action = "Index" });
            routes.MapRoute(RouteNames.VariantDetailRoute, "varyant-detay/{name}-{id}", new { controller = "Variant", action = "Detail" });
            routes.MapRoute(RouteNames.OnlinePosRoute, "sanal-poslar", new { controller = "Common", action = "OnlinePos" });
            routes.MapRoute(RouteNames.OnlinePosDetailRoute, "sanal-pos-detay/{id}", new { controller = "Common", action = "OnlinePosDetail" });

            routes.MapRoute(RouteNames.PaymentTypesRoute, "siparis-ayarlari", defaults: new { controller = "Common", action = "OrderSettings" });
            routes.MapRoute(RouteNames.BankInfoRoute, "banka-yonetimi", new { controller = "Order", action = "BankInfo" });
            routes.MapRoute(RouteNames.InstallmentInfoRoute, "taksit-yonetimi", new { controller = "Order", action = "InstallmentInfo" });
            routes.MapRoute(RouteNames.LanguageRoute, "dil-yonetimi", new { controller = "Management", action = "Language" });
            routes.MapRoute(RouteNames.RegionManagement, "bolge-yonetimi", new { controller = "Region", action = "Index" });
            routes.MapRoute(RouteNames.GalleryManagement, "galeri-yonetimi", new { controller = "Gallery", action = "Index" });

            routes.MapRoute(RouteNames.DashboardRoute, "dashboard", new { controller = "Home", action = "Index" });
            routes.MapRoute(RouteNames.MailTemplatesRoute, "eposta-sablonlari", new { controller = "Common", action = "MailTemplates" });
            routes.MapRoute(RouteNames.CacheManagementRoute, "cache-yonetimi", new { controller = "Common", action = "CacheManagement" });
            routes.MapRoute(RouteNames.LoginRoute, "uye-giris", new { controller = "User", action = "Index" });
            routes.MapRoute(RouteNames.LogoutRoute, "oturumu-kapat", new { controller = "User", action = "Logout" });
            routes.MapRoute(RouteNames.ForgotPasswordRoute, "sifremi-unuttum", new { controller = "User", action = "ForgotPassword" });
            routes.MapRoute(RouteNames.ChangePasswordRoute, "sifre-degistir", new { controller = "User", action = "ChangePassword" });
            routes.MapRoute(RouteNames.UnSuccessfullRoute, "islem-basarisiz", new { controller = "Error", action = "Index" });
            routes.MapRoute(RouteNames.UnauthorizedRoute, "yetki-yok", new { controller = "Error", action = "Unauthorized" });
            routes.MapRoute(RouteNames.NoAccessRoute, "erisim-yok", new { controller = "User", action = "NoAccess" });
            routes.MapRoute(RouteNames.NotFoundRoute, "sayfa-bulunamadi", new { controller = "Error", action = "NotFound" });

            routes.MapRoute(RouteNames.AllItemsRoute, "urun-yonetimi", new { controller = "StoreItem", action = "Index" });
            routes.MapRoute(RouteNames.StoreItemsRoute, "{storename}-urunleri/{storeID}", new { controller = "StoreItem", action = "Index" });
            routes.MapRoute(RouteNames.StoreItemDetailRoute, "{storename}-urun-detay/{productname}-{id}", new { controller = "StoreItem", action = "Detail" });
            routes.MapRoute(RouteNames.StoreNewItemRoute, "{storename}/yeni-urun/{storeID}", new { controller = "StoreItem", action = "Detail", id = 0, storeID = UrlParameter.Optional });

            routes.MapRoute(RouteNames.CampaignListRoute, "kampanya-yonetimi", new { controller = "Campaign", action = "Index" });
            routes.MapRoute(RouteNames.CampaignDetailRoute, "kampanya-detay/{name}-{id}", new { controller = "Campaign", action = "Detail" });
            routes.MapRoute(RouteNames.CategoryRoute, "kategori-yonetimi", new { controller = "Category", action = "Index" });
            routes.MapRoute(RouteNames.ProductListRoute, "urun-katalogu", new { controller = "Product", action = "Index" });
            routes.MapRoute(RouteNames.ProductDetailRoute, "urun-detay/{name}-{id}", new { controller = "Product", action = "Detail" });
            routes.MapRoute(RouteNames.OrderListRoute, "siparis-yonetimi", new { controller = "Order", action = "Index" });
            routes.MapRoute(RouteNames.OrderDetailRoute, "siparis-detay-{id}", new { controller = "Order", action = "Detail" });
            routes.MapRoute(RouteNames.NotificationRoute, "bildirim-yonetimi", new { controller = "Notification", action = "Index" });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}