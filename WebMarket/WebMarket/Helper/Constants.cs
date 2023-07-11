namespace WebMarket.Helper
{
    public class Constants
    {
        public const int DailyCacheDuration = 28800;

        public const string MainPageCss = "~/resource/css/MainPageCss";
        public const string CommonScripts = "~/resource/js/Common";
        public const string ProductScripts = "~/resource/js/Product/js";
        public const string HomePageScripts = "~/resource/js/HomePageScripts";
        public const string ProductListPageScripts = "~/resource/js/ProductListPageScripts";
        public const string BasketScripts = "~/resource/js/Basket/js";
        public const string CampaignScripts = "~/resource/Campaign/js";
        public const string AngularScripts = "~/resource/js/Angular/js";
        
        public const string LayoutCss = "~/resource/css/layout";
        public const string ThemeWhiteCss = "~/resource/css/themewhite";
        public const string ThemeDarkCss = "~/resource/css/themedark";
        public const string PageWithProductCss = "~/resource/css/PageWithProductCss";
        public const string BasketCss = "~/resource/css/Basketcss";
        public const string ProductListCss = "~/resource/css/ProductListCss";
        public const string ProductDetailCss = "~/resource/css/ProductDetailCss";

        public const string ImageRepository = "CDN_Image";
        public const string LocalImageRepository = "/resource/img/";
    }

    public class RouteNames
    {
        public const string ItemDetailUrl = "urun";
        public const string ProductListRoute = "urunler";
        public const string ProductListCategoryRoute = "kategori-urunleri";
        public const string ProductListStoreRoute = "market-urunleri";
        public const string ProductListNamedRoute = "ProductListNamedRoute";
        public const string ProductCompareRoute = "urun-karsilastir";
        public const string ProductCommentsRoute = "urun-yorum";
        public const string BrowserNotSupportedRoute = "BrowserNotSupportedRoute";

        public const string LoginRoute = "uye-girisi";
        public const string LogoutRoute = "oturumu-kapat";
        public const string AccountDetailRoute = "bilgilerim";
        public const string ChangePasswordRoute = "sifre-degistir";
        public const string StoreListRoute = "marketler";
        public const string StoreDetailRoute = "market-detay";
        public const string ProductDetailRoute = "urun-detay";
        public const string MyBasketRoute = "sepetim";
        public const string MyFavorites = "favorilerim";
        public const string MyOrdersRoute = "siparislerim";
        public const string OrderRoute = "siparis";
        public const string OrderPaymentRoute = "siparis-odeme";
        public const string OrderPaymentResultRoute = "siparis-odeme-sonucu";
        public const string OrderApproveRoute = "siparis-onay";
        public const string OrderDetailRoute = "siparis-detay";

        public const string SelectRegionRoute = "semt-sec";
        public const string ChangeLanguage = "dil-secimi";
        public const string CampaignListRoute = "kampanyalar";
        public const string CampaignDetailRoute = "kampanya-detay";
        public const string RegisterRoute = "uye-ol";
        public const string ActivationRoute = "activation";

        public const string HomeRoute = "HomeRoute";
        public const string AboutUsRoute = "hakkimizda";
        public const string SssRoute = "SssRoute";
        public const string MembershipAggreementRoute = "MembershipAggreementRoute";
        public const string ContactUsRoute = "iletisim";
        public const string DynamicPageRoute = "dynamicpage";
        public const string NotFound = "NotFound";
        public const string SiteMapIndex = "SiteMapIndex";
    }
}