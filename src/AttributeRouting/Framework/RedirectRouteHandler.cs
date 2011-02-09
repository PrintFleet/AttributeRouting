using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using AttributeRouting.Extensions;

namespace AttributeRouting.Framework
{
    public class RedirectRouteHandler : IRouteHandler
    {
        public RedirectRouteHandler(string absoluteUrl)
        {
            AbsoluteUrl = absoluteUrl;
        }

        public string AbsoluteUrl { get; private set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            // Need to apply the left-hand part of the url.
            var url = AbsoluteUrl;
            if (requestContext.HttpContext.Request.Url != null)
            {
                var leftHandUrlPart = requestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                url = "{0}/{1}".FormatWith(leftHandUrlPart.TrimEnd('/'), url.TrimStart('/'));
            }

            return new RedirectHttpHandler(url);
        }
    }
}
