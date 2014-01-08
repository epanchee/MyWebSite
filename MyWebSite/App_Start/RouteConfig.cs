using System.Web.Mvc;
using System.Web.Routing;

namespace MyWebSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Root",
                "",
                new { Controller = "Home", Action = "Index" }
            );

            routes.MapRoute(
                "Users",
                "users/{action}/{opts}",
                new { Controller = "Users"}
            );

            routes.MapRoute(
                "Navigate",
                "{action}",
                new { Controller = "Home" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}