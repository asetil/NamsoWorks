using System.Web.Optimization;

namespace WebMarket.Admin
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/resource/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/resource/js/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/resource/js/common").Include(
              "~/resource/js/jquery.js",
               "~/resource/js/bootstrap.js",
                "~/resource/js/perfect.scrollbar.js",
              "~/resource/js/aware.js",
              "~/resource/js/site.js"
             ));

            bundles.Add(new ScriptBundle("~/resource/js/loginjs").Include(
              "~/resource/js/jquery.js",
              "~/resource/js/bootstrap.js",
              "~/resource/js/aware.js",
              "~/resource/js/site.js",
              "~/resource/js/user.js"));

            bundles.Add(new ScriptBundle("~/resource/js/categoryjs").Include(
             "~/resource/js/category.js",
             "~/resource/js/gallery.js"));


            bundles.Add(new ScriptBundle("~/resource/js/productjs").Include(
            "~/resource/js/product.js",
            "~/resource/js/property.js",
            "~/resource/js/gallery.js"));

            bundles.Add(new ScriptBundle("~/resource/js/angularjs").Include(
              "~/resource/js/Angular/angular.min.js",
              "~/resource/js/Angular/angular-resource.min.js",
               "~/resource/js/Angular/app.js",
              "~/resource/js/Angular/services.js",
              "~/resource/js/Angular/controllers.js"));

            bundles.Add(new StyleBundle("~/resource/css").Include(
                "~/resource/css/bootstrap.css",
                "~/resource/css/admin.new.css",
                 "~/resource/css/font-awesome.css"
            ));

            bundles.Add(new StyleBundle("~/resource/css/logincss").Include(
               "~/resource/css/bootstrap.css",
               "~/resource/css/admin.new.css",
                "~/resource/css/font-awesome.css"));
        }
    }
}