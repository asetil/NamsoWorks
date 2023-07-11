namespace CleanCode.Helper
{
    public class Constants
    {
        public const int DailyCacheDuration = 28800;
        public const string CommonScripts = "~/resource/js/Common";
        public const string EntryScripts = "~/resource/js/entry";
        public const string CommonCss = "~/resource/css/layout";
        public const string AdminCss = "~/resource/css/admin";
    }

    public class RouteNames
    {
        public const string Home = "Home";
        public const string Search = "Search";
        public const string EntryDetail = "EntryDetail";
        public const string CategoryDetail = "CategoryDetail";
        public const string AdminDashboard = "AdminDashboard";
        public const string EntryManageDetail = "EntryManageDetail";
        public const string EntryManageList = "EntryManageList";
        public const string AuthorList = "AuthorList";
        public const string AuthorDetail = "AuthorDetail";
        public const string CategoryManageList = "CategoryManageList";
        public const string CacheManagement = "CacheManagement";
        public const string UserLogin = "UserLogin";
        public const string UserLogout = "UserLogout";
        public const string AboutUs = "AboutUs";
        public const string AngularJSRoute = "AngularJSRoute";
        public const string UnAuthorizedRoute = "UnAuthorizedRoute";
        public const string GalleryManagement = "GalleryManagementRoute";

        public const string SiteMapIndex = "SiteMapIndex";
        public const string SiteMapCategories = "SiteMapCategories";
        public const string SiteMapEntries = "SiteMapEntries";
    }

    public enum BannerType
    {
        Default=0,
        BannerAuto=1,
        BannerLeftPanel=2,
        BannerFeed=3,
        BannerEntry=4
    }
}