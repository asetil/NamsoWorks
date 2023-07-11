using System.Linq;
using System.Web.Mvc;
using Aware.ECommerce.Search;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using WebMarket.Admin.Helper;

namespace WebMarket.Admin.Models.ModelBinder
{
    public class CommentSearchBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(CommentSearchParams))
            {
                var request = controllerContext.HttpContext.Request;
                var keyword = request.Form.Get("q");
                var page =request.QueryString.Get("page").Int(1);
                var size = request.Form.Get("s").Int(10);

                var searchParams = new CommentSearchParams
                {
                    Keyword = (keyword ?? string.Empty).Trim().ToLowerInvariant(),
                    CommentStatus = (CommentStatus)request.Form.Get("st").Int(),
                    RelationType = request.Form.Get("rt").Int(),
                    Rating = request.Form.Get("r").Int(),
                    IDs = request.GetIDArray("ids"),
                };

                searchParams.SetPaging(page, size);

                if ((searchParams.IDs == null || !searchParams.IDs.Any()) && string.IsNullOrEmpty(keyword) && searchParams.RelationType == 0 
                    && searchParams.Rating == 0 && (CommentStatus)searchParams.Status.GetValueOrDefault() == CommentStatus.None)
                {
                    searchParams.RelationType = (int)RelationTypes.Product;
                }

                searchParams.SortBy(i=>i.DateCreated, true);
                return searchParams;
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}