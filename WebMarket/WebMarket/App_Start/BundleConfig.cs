using System.Web.Optimization;
namespace WebMarket
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(Helper.Constants.LayoutCss).Include(
                  "~/resource/css/bootstrap.css",
                   "~/resource/css/common.css",
                  "~/resource/css/basket.css",
                   "~/resource/css/font-awesome.css",
                    "~/resource/css/scrollbar.css"
                  ));

            bundles.Add(new StyleBundle(Helper.Constants.ThemeWhiteCss).Include(
                "~/resource/css/bootstrap.css",
                "~/resource/css/theme/theme-white.css",
                  "~/resource/css/basket.css",
                  "~/resource/css/font-awesome.css",
                  "~/resource/css/scrollbar.css"
                  ));

            bundles.Add(new StyleBundle(Helper.Constants.ThemeDarkCss).Include(
                "~/resource/css/bootstrap.css",
                 "~/resource/css/theme/theme-dark.css",
                "~/resource/css/basket.css",
                 "~/resource/css/font-awesome.css",
                  "~/resource/css/scrollbar.css"
                ));

            bundles.Add(new StyleBundle(Helper.Constants.PageWithProductCss).Include(
                  "~/resource/css/productList.css",
                  "~/resource/css/product.css"
                 ));

            bundles.Add(new StyleBundle(Helper.Constants.ProductDetailCss).Include(
                 "~/resource/css/productList.css",
                 "~/resource/css/product.css",
                 "~/resource/css/swiper.css"
                ));

            bundles.Add(new ScriptBundle(Helper.Constants.CommonScripts).Include(
                "~/resource/js/jquery.js",
                "~/resource/js/bootstrap.js",
                 "~/resource/js/scrollbar.js",
                "~/resource/js/aware.js",
                "~/resource/js/site.js",
                "~/resource/js/social.js"));

            bundles.Add(new ScriptBundle(Helper.Constants.HomePageScripts).Include(
                "~/resource/js/lazyLoading.js",
              "~/resource/js/productList.js"));

            bundles.Add(new ScriptBundle(Helper.Constants.ProductListPageScripts).Include(
                "~/resource/js/lazyLoading.js",
                "~/resource/js/Product/swiper.js",
                "~/resource/js/productList.js",
                "~/resource/js/productFilter.js"));

            bundles.Add(new ScriptBundle(Helper.Constants.ProductListCss).Include(
               "~/resource/css/productList.css",
                 "~/resource/css/product.css"));

            bundles.Add(new ScriptBundle(Helper.Constants.ProductScripts).Include(
                "~/resource/js/productList.js",
                "~/resource/js/Product/Product.js",
                "~/resource/js/Product/jquery.elevatezoom.js",
                "~/resource/js/Product/swiper.js",
                "~/resource/js/Product/variant.js"));

            bundles.Add(new ScriptBundle(Helper.Constants.CampaignScripts).Include(
                "~/resource/js/jquery.countdown.js"));

            bundles.Add(new ScriptBundle(Helper.Constants.AngularScripts).Include(
               "~/resource/js/Angular/angular.min.js",
               "~/resource/js/Angular/angular-resource.min.js",
                "~/resource/js/Angular/app.js",
               "~/resource/js/Angular/services.js",
               "~/resource/js/Angular/controllers.js"));
        }
    }
}