using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace ArzTalep.Web.Helper
{
    public static class RouteHelper
    {
        public static void MapRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(name: "uye-girisi", template: WebConstants.PageUrl.Login, defaults: new { controller = "Account", action = "Index" });
            routes.MapRoute(name: "sifre-degistir", template: "hesap/sifre-degistir", defaults: new { controller = "Account", action = "ChangePassword" });
            routes.MapRoute(name: "uye-ol", template: "hesap/uye-ol", defaults: new { controller = "Account", action = "Register" });
            routes.MapRoute(name: "uye-bilgileri", template: "hesap/uye-bilgileri", defaults: new { controller = "Account", action = "Detail" });
            routes.MapRoute(name: "uyelik-aktivasyonu", template: "hesap/uyelik-aktivasyonu", defaults: new { controller = "Account", action = "Activation" });
            routes.MapRoute(name: "campaign", template: "kampanya", defaults: new { controller = "Home", action = "Campaign" });
            routes.MapRoute(name: "sayfa-bulunamadi", template: "sayfa-bulunamadi", defaults: new { controller = "Error", action = "Index", code = 404 });
            routes.MapRoute(name: "error", template: "error/{code}", defaults: new { controller = "Error", action = "Index" });
            routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
