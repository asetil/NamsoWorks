using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.ECommerce.Enums;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Model;
using Aware.Util.View;

namespace CleanCode.Controllers
{
    public class AuthorController : AwareController
    {
        private readonly IUserService _userService;

        public AuthorController(IUserService userService)
        {
            _userService = userService;
        }

        [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser)]
        public ActionResult Index()
        {
            var model = _userService.GetCustomerUsers(CurrentUser.CustomerID);
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult Detail(int id)
        {
            if (CurrentUser.Role != UserRole.SuperUser && CurrentUserID != id)
            {
                return RedirectToRoute(Helper.RouteNames.UnAuthorizedRoute);
            }

            User model = null;
            if (id == 0 && CurrentUser.Role == UserRole.SuperUser)
            {
                model = new User();
            }
            else if (id > 0)
            {
                model = _userService.GetAdminUser(id);
                if (model != null && model.ID > 0)
                {
                    model.Password = Encryptor.Decrypt(model.Password);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Aware.Util.Filter.Authorize]
        public ActionResult Detail(User model)
        {
            if (CurrentUser.Role != UserRole.SuperUser && model.ID != CurrentUserID)
            {
                return RedirectToRoute(Helper.RouteNames.UnAuthorizedRoute);
            }

            var result = Result.Error();
            if (model != null)
            {
                if (model.ID > 0)
                {
                    var manager = _userService.GetAdminUser(model.ID);
                    if (manager != null)
                    {
                        manager.Name = model.Name;
                        manager.Email = model.Email;
                        result = _userService.SaveUser(manager);

                        var oldPassword = Encryptor.Decrypt(manager.Password);
                        if (result.OK && oldPassword != model.Password)
                        {
                            result = _userService.ChangePassword(manager.ID, oldPassword, model.Password);
                        }
                    }
                }
                else
                {
                    model.Status = Statuses.Active;
                    model.Role = UserRole.AdminUser;
                    result = _userService.SaveUser(model);

                    if (result.OK)
                    {
                        var author = result.ValueAs<User>();
                        return RedirectToRoute(Helper.RouteNames.AuthorDetail, new { id = author.ID, name = author.Name.ToSeoUrl() });
                    }
                }
            }

            ViewBag.OperationResult = result;
            return View(model);
        }
    }
}
