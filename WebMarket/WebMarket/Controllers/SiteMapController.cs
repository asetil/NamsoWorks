using Aware.ECommerce.Interface;
using Aware.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebMarket.Helper;

namespace WebMarket.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly ICategoryService _categoryService;

        public SiteMapController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var domainUrl = Config.DomainUrl;
            var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var element = new XElement(ns + "sitemapindex");

            var categoryList = _categoryService.GetCategories();

            element.Add(Get(ns, "/", "1.00"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.HomeRoute), "0.80"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.AboutUsRoute), "0.70"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.SssRoute), "0.60"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.ContactUsRoute), "0.60"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.StoreListRoute), "0.80"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.CampaignListRoute), "0.80"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.ProductListRoute), "0.90"));

            //if (entrySearchResult != null && entrySearchResult.HasResult)
            //{
            //    foreach (var entry in entrySearchResult.Results)
            //    {
            //        element.Add(Get(ns, Url.RouteUrl(RouteNames.EntryDetail, new { name = entry.Name.ToSeoUrl(true), id = entry.ID }), "0.90"));
            //    }
            //    categoryList = categoryList.Where(c => entrySearchResult.Results.Any(e => e.CategoryID == c.ID)).ToList();
            //}

            if (categoryList != null && categoryList.Any())
            {
                foreach (var category in categoryList)
                {
                    element.Add(Get(ns, Url.RouteUrl(RouteNames.ProductListCategoryRoute, new { cname = category.Name.ToSeoUrl(true), cid = category.ID }), "0.90"));
                }
            }

            var siteMapDocument = new XDocument(element);
            return Content(siteMapDocument.ToString(), "text/xml");
        }

        private XElement Get(XNamespace ns, string url, string priority = "0.5", string changefreq = "daily")
        {
            return new XElement
            (
                ns + "sitemap",
                new XElement(ns + "loc", string.Format("{0}{1}", Config.DomainUrl, url)),
                new XElement(ns + "changefreq", changefreq),
                new XElement(ns + "priority", priority)
            );
        }
    }
}
