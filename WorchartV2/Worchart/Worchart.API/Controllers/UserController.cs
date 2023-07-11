using Microsoft.AspNetCore.Mvc;
using System;
using Worchart.BL.Model;
using Worchart.BL.User;

namespace Worchart.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserManager _userManager;
        public UserController(IUserManager userManager, BL.Log.ILogger logger)
        {
            _userManager = userManager;
        }

        [HttpPost("login")]
        public ActionResult<OperationResult> Login([FromBody] LoginRequestModel requestModel)
        {
            var result = Validate();
            if (result.Success)
            {
                result = _userManager.Login(requestModel);
                if (result.Success)
                {
                    var authorizeModel = result.ValueAs<AuthorizedCustomerModel>();
                    //HttpContext.Response.Cookies.Append("osman", authorizeModel.CustomerToken, new Microsoft.AspNetCore.Http.CookieOptions()
                    //{
                    //    Expires = DateTime.Now.AddDays(7),
                    //    HttpOnly = true,
                    //    Path = "/",
                    //});

                    result.Value = new
                    {
                        userToken = authorizeModel.CustomerToken,
                        name = authorizeModel.Name,
                        firmId = authorizeModel.CompanyID
                    };
                }

            }
            return WithResult(result);
        }

        [HttpPost("detail")]
        public ActionResult<OperationResult> Detail(string authorizeToken)
        {
            var result = Validate();
            if (result.Success)
            {
                result.Value = _userManager.Get(CustomerID);
            }
            return WithResult(result);
        }

        [HttpPost("register")]
        public ActionResult<OperationResult> Register([FromBody] RegisterModel model)
        {
            var result = Validate();
            if (result.Success)
            {
                result = _userManager.Register(model);
            }
            return WithResult(result);
        }

        [HttpPost("update")]
        public ActionResult<OperationResult> UpdateUser([FromBody] UserModel model)
        {
            var result = Failed();
            if (ModelState.IsValid)
            {
                result = _userManager.Save(model);
            }
            return WithResult(result);
        }

        [HttpPost("change-password")]
        public ActionResult<OperationResult> ChangePassord(string authorizeToken, string newPassword)
        {
            var result = Validate();
            if (result.Success)
            {
                result = _userManager.ChangePassord(authorizeToken, newPassword);
            }
            return WithResult(result);
        }

        [HttpPost("logoff")]
        public ActionResult<OperationResult> Logoff(string authorizeToken)
        {
            var result = Failed();
            if (ModelState.IsValid)
            {
                result = _userManager.Logoff(authorizeToken);
            }
            return WithResult(result);
        }
    }
}