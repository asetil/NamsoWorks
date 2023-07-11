using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Util.Model;
using System.Web.Security;
using Aware;
using Aware.Util.View;

namespace CleanCode.Controllers
{
    public class UserController : AwareController
    {
        private readonly IUserService _userService;
        private readonly ISessionManager _sessionManager;
        public UserController(IUserService userService, ISessionManager sessionManager)
        {
            _userService = userService;
            _sessionManager = sessionManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string returnUrl="")
        {
            if (IsLoggedIn)
            {
                if (string.IsNullOrEmpty(returnUrl)) { returnUrl = "/manage/entry-list"; }
                return Redirect(returnUrl);
            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Login(User model)
        {
            var result = Result.Error(Resource.General_Error);
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Email))
            {
                result = _sessionManager.Authorize(model);
                if (result.OK)
                {
                    model = result.ValueAs<User>();
                    FormsAuthentication.SetAuthCookie(model.ID + ";;;;" + model.Name, false);
                }
            }
            return Json(new { success=result.IsSuccess, message=result.Message }, JsonRequestBehavior.DenyGet);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            _sessionManager.Logout();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LoginInfo()
        {
            return PartialView("_LoginInfo", CurrentUser);
        }
    }
}
