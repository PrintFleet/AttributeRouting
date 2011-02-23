using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Web.Controllers;
using AttributeRouting.Web.Models;
using ControllerBase = AttributeRouting.Web.Controllers.ControllerBase;

namespace AttributeRouting.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            BeginRequest += MvcApplication_BeginRequest;
        }

        void MvcApplication_BeginRequest(object sender, System.EventArgs e)
        {
            if (Request.UserLanguages != null && Request.UserLanguages.Any())
            {
                var cultureInfo = new CultureInfo(Request.UserLanguages[0]);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
        }

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
                var translationProvider = new RouteTranslationProvider(
                    "en", new[] { "en", "es", "fr" },
                    new RouteTranslation("Home_About_RouteUrl", "About", "en"),
                    new RouteTranslation("Home_About_RouteUrl", "Sobre", "es"),
                    new RouteTranslation("Home_About_RouteUrl", "Presque", "fr")
                    );

                config.ScanAssemblyOf<ControllerBase>();
                config.AddDefaultRouteConstraint(@"[Ii]d$", new RegexRouteConstraint(@"^\d+$"));
                config.TranslationProvider = translationProvider;
            });

            routes.MapRoute("CatchAll",
                            "{*path}",
                            new { controller = "home", action = "filenotfound" },
                            new[] { typeof(HomeController).Namespace });
        }
    }
}