using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace AttributeRouting.Specs.Subjects
{
    public class LocalizationController : Controller
    {
        [GET("", Order = 1)]
        [GET("Index", Order = 2)]
        public ActionResult Index()
        {
            return Content("");
        }

        [GET("ExplicitTranslationKey", UrlTranslationKey = "Localization_Explicit_RouteUrl")]
        public ActionResult ExplicitTranslationKey()
        {
            return Content("");
        }
    }

    [RouteArea("AreaLocalization")]
    public class AreaLocalizationController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return Content("");
        }
    }

    [RouteArea("AreaLocalizationExplicitKey", AreaUrlTranslationKey = "Explicit_AreaUrl")]
    public class AreaLocalizationExplicitKeyController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return Content("");
        }
    }

    [RoutePrefix("PrefixLocalization")]
    public class PrefixLocalizationController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return Content("");
        }
    }

    [RoutePrefix("PrefixLocalizationExplicitKey", UrlTranslationKey = "Explicit_RoutePrefixUrl")]
    public class PrefixLocalizationExplicitKeyController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return Content("");
        }
    }

    [RouteArea("Area")]
    [RoutePrefix("SomePrefix")]
    public class PrefixedAreaLocalizationController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return Content("");
        }
    }
}
