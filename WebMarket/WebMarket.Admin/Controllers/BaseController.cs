using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aware;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Authority;
using Aware.Dependency;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;
using Aware.File;
using Aware.File.Model;
using WebMarket.Admin.Helper;
using Aware.ECommerce.Model.Custom;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize]
    public class BaseController : Controller
    {
        private CustomPrincipal _currentUser;
        protected CustomPrincipal CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = HttpContext.User as CustomPrincipal;
                    if (_currentUser == null)
                    {
                        var sessionManager = WindsorBootstrapper.Resolve<ISessionManager>();
                        sessionManager.Authenticate(ref _currentUser);

                        if (_currentUser != null)
                        {
                            _currentUser.Identity = HttpContext.User.Identity;
                            HttpContext.User = _currentUser;
                        }
                    }
                }
                return _currentUser;
            }
        }

        protected bool IsLoggedIn
        {
            get { return CurrentUser != null && CurrentUser.ID > default(int); }
        }

        protected int CurrentUserID
        {
            get { return IsLoggedIn ? CurrentUser.ID : default(int); }
        }

        protected UserInfoModel UserInfo
        {
            get
            {
                if (IsLoggedIn)
                {
                    return new UserInfoModel(CurrentUserID, CurrentUser.Role);
                }
                return null;
            }
        }

        public JsonResult ResultValue(Result result, JsonRequestBehavior behaviour = JsonRequestBehavior.DenyGet)
        {
            result = result ?? Result.Error(Resource.General_Error);
            return Json(new { success = result.IsSuccess, message = result.Message }, behaviour);
        }

        public JsonResult JsonValue(Result result, JsonRequestBehavior behaviour = JsonRequestBehavior.DenyGet)
        {
            return Json(new { result }, behaviour);
        }

        public bool IsSuper(bool addToView = true)
        {
            var isSuper = CurrentUser.Role == UserRole.SuperUser;
            if (addToView) { ViewBag.IsSuper = isSuper; }
            return isSuper;
        }

        public ViewTypes GetViewMode()
        {
            if (CurrentUser == null) { return ViewTypes.UnAuthorized; }
            switch (CurrentUser.Role)
            {
                case UserRole.AdminUser:
                    return ViewTypes.Read;
                case UserRole.SuperUser:
                    return ViewTypes.Editable;
                default:
                    return ViewTypes.UnAuthorized;
            }
        }

        public bool IsAuthorizedFor(AuthorityType authorityType, bool redirect = true)
        {
            if (CurrentUserID > 0 && CurrentUser.Role == UserRole.SuperUser) { return true; }

            var authorityManager = WindsorBootstrapper.Resolve<IAuthorityManager>();
            var success = authorityManager.Check(authorityType, CurrentUserID);
            if (!success && redirect)
            {
                Response.RedirectToRoute(RouteNames.UnauthorizedRoute);
            }
            return success;
        }

        protected Result SaveMyFiles(int relationID, int relationType)
        {
            var postedFiles = GetPostedFiles();
            if (postedFiles.Any())
            {
                var fileService = WindsorBootstrapper.Resolve<IFileService>();
                return fileService.SaveFileToDisc(relationID, relationType, postedFiles.FirstOrDefault());
            }
            return Result.Error();
        }

        protected List<PostedFileModel> GetPostedFiles()
        {
            var result = new List<PostedFileModel>();
            if (Request.Files != null && Request.Files.Count > 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase requestFile = Request.Files[i];
                    if (requestFile != null && requestFile.InputStream != null && requestFile.InputStream.Length > 0)
                    {
                        result.Add(new PostedFileModel
                        {
                            Name = requestFile.FileName,
                            Stream = requestFile.InputStream,
                            ContentType = requestFile.ContentType,
                            ContentLength = requestFile.ContentLength,
                        });
                    }
                }
            }
            return result;
        }

        protected bool IsValidOperation(int customerID)
        {
            if (!CurrentUser.IsSuper && CurrentUser.CustomerID != customerID)
            {
                return false;
            }
            return true;
        }
    }
}
