using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Aware.ECommerce.Interface;
using Aware.Util;
using Aware.Util.Enums;
using CleanCode.Helper;
using CleanFramework.Business.Model;
using CleanFramework.Business.Service;

namespace CleanCode.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IEntryService _entryService;

        public SiteMapController(IEntryService entryService, ICategoryService categoryService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var domainUrl = Config.DomainUrl;
            var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var element = new XElement(ns + "sitemapindex");

            var searchParams = new EntrySearchParams();
            searchParams.SetPaging(1, Int32.MaxValue);
            searchParams.Status=Statuses.Active;
            var entrySearchResult = _entryService.Search(searchParams,false,false);
            var categoryList = _categoryService.GetCategories();

            element.Add(Get(ns, "/", "1.00"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.Home), "0.80"));
            element.Add(Get(ns, Url.RouteUrl(RouteNames.AboutUs), "0.80"));

            if (entrySearchResult != null && entrySearchResult.HasResult)
            {
                foreach (var entry in entrySearchResult.Results)
                {
                    element.Add(Get(ns, Url.RouteUrl(RouteNames.EntryDetail, new { name = entry.Name.ToSeoUrl(true), id = entry.ID }), "0.90"));
                }
                categoryList=categoryList.Where(c => entrySearchResult.Results.Any(e => e.CategoryID == c.ID)).ToList();
            }

            if (categoryList != null && categoryList.Any())
            {
                foreach (var category in categoryList)
                {
                    element.Add(Get(ns, Url.RouteUrl(RouteNames.CategoryDetail, new { name = category.Name.ToSeoUrl(true), categoryID = category.ID }), "0.90"));
                }
            }

            var siteMapDocument = new XDocument(element);
            return Content(siteMapDocument.ToString(), "text/xml");
        }

        private XElement Get(XNamespace ns,string url,string priority="0.5", string changefreq = "daily")
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
