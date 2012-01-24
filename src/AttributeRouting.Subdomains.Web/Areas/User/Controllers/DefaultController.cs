using System.Web.Mvc;

namespace AttributeRouting.Subdomains.Web.Areas.User.Controllers
{
    [RouteArea("User", Subdomain = "{username}")]
    public class DefaultController : Controller
    {
        [GET("")]
        public ActionResult Index(string username)
        {
            return View(username);
        }
    }
}
