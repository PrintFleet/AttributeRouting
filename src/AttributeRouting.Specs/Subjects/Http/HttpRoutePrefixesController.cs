using System.Web.Http;
using AttributeRouting.Web.Http;

namespace AttributeRouting.Specs.Subjects.Http
{
    [RoutePrefix("ApiPrefix")]
    public class HttpRoutePrefixesController : ApiController
    {
        [GET("Get")]
        public string Get()
        {
            return "";
        }

        [GET("ApiPrefix/DuplicatePrefix")]
        public string DuplicatePrefix()
        {
            return "";
        }

        [GET("ApiPrefixAbsolute", IsAbsoluteUrl = true)]
        public string Absolute()
        {
            return "";
        }

        [GET("ApiPrefixer")]
        public string RouteBeginsWithRoutePrefix()
        {
            return "";
        }
    }
}