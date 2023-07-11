using CleanCode.Helper;
using System.Web.Optimization;

namespace CleanCode
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(Constants.CommonScripts).Include(
                "~/res/js/jquery.js",
                "~/res/js/bootstrap.js",
                "~/res/js/aware.js",
                "~/res/js/site.js"));

            bundles.Add(new ScriptBundle(Constants.EntryScripts).Include(
                "~/res/js/prism.js"));

            bundles.Add(new StyleBundle(Constants.CommonCss).Include(
                  "~/res/css/bootstrap.css",
                   "~/res/css/font-awesome.css",
                   "~/res/css/site.css"
                  ));

            bundles.Add(new StyleBundle(Constants.AdminCss).Include(
                 "~/res/css/bootstrap.css",
                  "~/res/css/font-awesome.css",
                  "~/res/css/admin.css"
                 ));
        }
    }
}