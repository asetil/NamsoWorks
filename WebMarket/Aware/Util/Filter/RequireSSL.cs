using System.Web.Mvc;

namespace Aware.Util.Filter
{
    public class RequireSSLAttribute : RequireHttpsAttribute
    {
        public bool RequireSsl { get; set; }

        public RequireSSLAttribute()
        {
            RequireSsl = Config.RequireSsl;
        }

        public RequireSSLAttribute(bool requireSsl)
        {
            RequireSsl = requireSsl;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null && RequireSsl && !filterContext.HttpContext.Request.IsSecureConnection)
            {
                HandleNonHttpsRequest(filterContext);
            }
        }
    }
}