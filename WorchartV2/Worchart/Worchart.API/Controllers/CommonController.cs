using Microsoft.AspNetCore.Mvc;
using Worchart.BL.Model;
using Microsoft.AspNetCore.Authorization;
using Worchart.BL.Manager;

namespace Worchart.API.Controllers
{
    [Authorize(Policy = "public")]
    public class CommonController : BaseController
    {
        private readonly ICommonManager _commonManager;

        public CommonController(ICommonManager commonManager)
        {
            _commonManager = commonManager;
        }

        [HttpGet("slideritems")]
        public ActionResult<OperationResult> SliderItems()
        {
            var model = _commonManager.SearchBy(i => i.Status == BL.Enum.StatusType.Active);
            return Success(model);
        }
    }
}