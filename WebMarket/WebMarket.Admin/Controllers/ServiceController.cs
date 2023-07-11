using System.Web.Mvc;
using Aware;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Search;
using Aware.Util.Enums;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
    public class ServiceController : BaseController
    {
        private readonly IStoreItemService _itemService;
        public ServiceController(IStoreItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost]
        public JsonResult GetItems(string ticket, int size, int page = 1)
        {
            if (IsAuthorized(ticket))
            {
                var searchParams = new ItemSearchParams(string.Empty, page, size)
                {
                    Fields = "Product,Product.Category"
                };

                var result = _itemService.GetItems(searchParams, CurrentUser.CustomerID);
                if (result != null && result.SearchResult != null && result.SearchResult.Success)
                {
                    return Json(new { success = result.SearchResult.Success ? 1 : 0, result.SearchResult.Results, result.Categories }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = 0, message = Resource.General_Error }, JsonRequestBehavior.AllowGet);
        }

        private bool IsAuthorized(string ticket)
        {
            return ticket == "sngrlu";
        }
    }
}
