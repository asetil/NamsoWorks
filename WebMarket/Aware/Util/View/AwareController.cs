using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.Dependency;
using Aware.File.Model;
using Aware.Util.Model;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Aware.Util.View
{
    public abstract class AwareController : Controller
    {
        private CustomPrincipal _currentUser;
        private int _currentUserID = -1;
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
            get { return CurrentUserID > default(int); }
        }

        protected int CurrentUserID
        {
            get
            {
                if (_currentUserID == -1)
                {
                    _currentUserID = (CurrentUser != null ? CurrentUser.ID : default(int));
                }
                return _currentUserID == -1 ? 0 : _currentUserID;
            }
        }

        public string IPAddress
        {
            get
            {
                return Request.ServerVariables["http_client_ip"] ?? Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        public JsonResult ResultValue(Result result, JsonRequestBehavior behaviour = JsonRequestBehavior.DenyGet)
        {
            result = result ?? Result.Error(Resource.General_Error);
            return Json(new { success = result.IsSuccess, message = result.Message }, behaviour);
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
    }
}
