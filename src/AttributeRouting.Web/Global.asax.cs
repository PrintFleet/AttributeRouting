﻿using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Web.Controllers;
using ControllerBase = AttributeRouting.Web.Controllers.ControllerBase;

namespace AttributeRouting.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapAttributeRoutes(config =>
            {
                config.ScanAssemblyOf<ControllerBase>();
                config.AddDefaultRouteConstraint(@"[Ii]d$", new RegexRouteConstraint(@"^\d+$"));
            });

            routes.MapRoute("CatchAll",
                            "{*path}",
                            new { controller = "home", action = "filenotfound" },
                            new[] { typeof(HomeController).Namespace });
        }
    }
}