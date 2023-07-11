using System.Web.Mvc;
using Aware.Crm;
using Aware.Crm.Model;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Lookup;
using Aware.Util.Model;
using WebMarket.Admin.Models;
using WebMarket.Admin.Models.ModelBinder;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ILookupManager _lookupManager;

        public CustomerController(ICustomerService customerService, ILookupManager lookupManager)
        {
            _customerService = customerService;
            _lookupManager = lookupManager;
        }

        public ActionResult Search([ModelBinder(typeof(CustomerSearchBinder))] CustomerSearchParams searchParams = null)
        {
            var searchResult = _customerService.Search(searchParams);
            return View(searchResult);
        }

        public ActionResult Detail(int id)
        {
            var model = new CustomerViewModel()
            {
                Customer = id > 0 ? _customerService.Get(id) : new Customer { Name = "Yeni Firma" },
                StatusList = _lookupManager.GetLookups(LookupType.Status),
                IsSuper = CurrentUser.IsSuper
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Detail(Customer model)
        {
            var result = Result.Error();
            if (ModelState.IsValid && model != null)
            {
                var isNew = model.ID == 0;

                result = _customerService.Save(model);
                if (result.OK)
                {
                    model = result.ValueAs<Customer>();
                    var imgSaveResult = SaveMyFiles(model.ID, (int)RelationTypes.Customer);
                    if (imgSaveResult.OK)
                    {
                        model.Logo = imgSaveResult.Value.ToString();
                    }

                    if (isNew)
                    {
                        return RedirectToRoute(Helper.RouteNames.CustomerDetailRoute, new { name = model.Name.ToSeoUrl(), id = model.ID });
                    }
                }
            }

            ViewBag.SaveResult = result;
            var detailModel = new CustomerViewModel
            {
                Customer = model,
                StatusList = _lookupManager.GetLookups(LookupType.Status)
            };
            return View(detailModel);
        }

        [HttpPost]
        public JsonResult Delete(int customerID)
        {
            var result = _customerService.Delete(customerID);
            return ResultValue(result);
        }
    }
}
