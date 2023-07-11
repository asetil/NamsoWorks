namespace Aware.ECommerce.Util
{
    public static class Constants
    {
        public const int PAGE_SIZE = 36;
        public const int STORE_PAGE_SIZE = 15;
        public const int COMMENT_PAGE_SIZE = 10;
        public const int ACTVATION_CODE_EXPIRE = 120;
        public const int STOCK_MAX = 9999;
        public const int PRICE_MAX = 9999999;
        public const string ConnectionString = "Main.ConnectionString";

        public const string LangSessionKey = "UserLang";
        public const string DefaultLanguage = "tr-TR";
        public const int DefaultLangID = -1;

        public const string LocalizationCookie = "Localization";
        public const string UserInfoCookie = "UserInfo";

        //MAIL TEMPLATE NAMES
        public const string WelcomeMailTemplate = "WelcomeMailTemplate";
        public const string ForgotPasswordMail = "ForgotPasswordMail";
        public const string PasswordChangedMail = "PasswordChangedMail";
        public const string ActivationMail = "ActivationMail";
        public const string CouponMail = "CouponMail";
        public const string WarningMail = "WarningMail";
        public const string ContactUsMail = "ContactUsMail";
        public const string OrderMail = "OrderMail";

        //CACHER KEYS
        public const string CK_TicketCache = "TICKET_CACHE";
        public const string CK_Regions = "REGIONS_CACHE";
        public const string CK_Region = "REGION_CACHE_{0}";
        public const string CK_ShippingMethods = "SHIPPING_METHODS_CACHE";
        public const string CK_FavoriteProducts = "FAVORITE_PRODUCTS_CACHE_{0}";
        public const string CK_MailTemplates = "MAIL_TEMPLATES_CACHE";
        public const string CK_MainCategories = "MAIN_CATEGORIES_CACHE";
        public const string CK_SimpleItems = "SIMPLE_ITEMS_CACHE_{0}";
        public const string CK_SiteModel = "SITE_SETTINGS";
        public const string CK_OrderSettings = "ORDER_SETTINGS";
        public const string CK_CategoryList = "CATEGORY_LIST_CACHE";
        public const string CK_CategoryLanguageValues = "CATEGORY_LANGVALUES_CACHE";
        public const string Slider_Cache = "Slider_Cache_{0}";

        public const string CK_RegionStores = "REGION_{0}_STORES_CACHE";
        public const string CK_ActiveCampaigns = "REGION_{0}_ACTIVE_CAMPAIGNS";
        public const string CK_BasketSummary = "BASKET_SUMMARY_{0}_CACHE";
        public const string CK_CustomerStores = "GET_USER_STORES_{0}";
        public const string CK_StoreStatistics = "STORE_STATISTICS_{0}";
        public const string CK_Properties = "ALL_PROPERTIES";
        public const string CK_Brands = "ALL_BRANDS";
        public const string CK_Banks = "ALL_BANKS";
        public const string CK_BinMumbers = "ALL_BinMumbers";
        public const string CK_Installments = "ALL_Installments";
        public const string CK_VariantProperties = "ALL_Variants";
        public const string CK_Languages = "Site_Languages";
        public const string CK_Lookups = "ALL_Lookups";


        public const int DAILY_CACHE_TIME = 24 * 60 * 60;
        public const int HOUR_CACHE_TIME = 60 * 60;
        public const string ORDER_DATE_FORMAT = "mmhhyyMMddss";
        public const string CreditCardRegex = @"^(?:(?<Mastercard>5[1-5]\d{14})|(?<Visa>4(?:\d{15}|\d{12}))|(?<Amex>3[47]\d{13})|)$";

        public const string GoogleRecaptchaValidateUrl = "https://www.google.com/recaptcha/api/siteverify";
        public const string YesNoOptions = "YesNoOptions";
        public const string HasNoOptions = "HasNoOptions";

        public const string Currency = "TL";

    }
}
