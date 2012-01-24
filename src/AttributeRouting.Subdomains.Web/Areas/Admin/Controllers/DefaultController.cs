using System.Web.Mvc;

namespace AttributeRouting.Subdomains.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", Subdomain = "admin")]
    public class DefaultController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
