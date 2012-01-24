using System.Web.Mvc;

namespace AttributeRouting.Subdomains.Web.Controllers
{
    public class DefaultController : Controller
    {
        [GET("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
