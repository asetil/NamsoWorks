using Aware.BL.Model;
using Aware.Util.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Cinescope.Web.Controllers
{
    public class BaseController : Controller
    {
        protected OperationResult<T> Success<T>(T value, string code = ResultCodes.Success.OperationSuccess)
        {
            return OperationResult<T>.Success(code, value);
        }

        protected OperationResult<T> Failed<T>(string code = ResultCodes.Error.CheckParameters)
        {
            return OperationResult<T>.Error(code);
        }
    }
}