using System.Web;

namespace AttributeRouting.Framework
{
    public class RedirectHttpHandler : IHttpHandler
    {
        private readonly string _url;

        public RedirectHttpHandler(string url)
        {
            _url = url;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext httpContext)
        {

            httpContext.Response.RedirectPermanent(_url, true);
        }
    }
}