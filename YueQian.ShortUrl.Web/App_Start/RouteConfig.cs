using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YueQian.ShortUrl.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "VerifyCode",
               url: "VerifyCode/{action}/{id}",
               defaults: new { controller = "VerifyCode", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Validate",
                url: "Validate/{action}/{id}",
                defaults: new { controller = "Validate", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AdvManager",
                url: "System/AdvManager/{id}/{p}",
                defaults: new { controller = "System", action = "AdvManager", id = "a", p = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Setting",
                url: "System/Setting/{id}/{p}",
                defaults: new { controller = "System", action = "Setting", id = "a", p = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "System",
                url: "System/{action}/{id}",
                defaults: new { controller = "System", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "User",
                url: "User/{action}/{id}",
                defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Home",
                url: "{action}.html",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "Home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "ShortUrl",
               url: "{id}",
               defaults: new { controller = "ShortUrl", action = "Index" });

            routes.MapRoute(
                name: "Index",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}