using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Extensions;

namespace AttributeRouting.Framework
{
    /// <summary>
    /// Custom Route used to apply some AttributeRouting magic.
    /// </summary>
    public class AttributeRoute : Route
    {
        public AttributeRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
            : base(url, defaults, constraints, dataTokens, new MvcRouteHandler())
        {
            MappedSubdomains = new List<string>();
        }

        /// <summary>
        /// List of all the subdomains mapped via AttributeRouting.
        /// </summary>
        public List<string> MappedSubdomains { get; set; }
        
        /// <summary>
        /// The subdomain this route is to be applied against.
        /// </summary>
        public string Subdomain { get; set; }

        /// <summary>
        /// The name of this route, for supporting named routes.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The AttributeRouting configuration options.
        /// </summary>
        public AttributeRoutingConfiguration Configuration { get; set; }

        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            // If no subdomains are mapped with AR, then just resort to default behavior.
            if (!MappedSubdomains.Any())
                return base.GetRouteData(httpContext);

            // Get the subdomain from the requested hostname.
            var subdomain = Configuration.DefaultSubdomain;
            var url = httpContext.Request.Url;
            if (url.SafeGet(u => u.HostNameType) == UriHostNameType.Dns)
            {
                var host = httpContext.Request.Headers["host"];
                
                var match = Regex.Match(host, Configuration.SubdomainMatchPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    subdomain = match.Groups.Cast<Group>().Last().Value;
            }

            // If this route is mapped to the requested host's subdomain, 
            // then return the route data for this request.
            if (Subdomain.ValueEquals(subdomain))
                return base.GetRouteData(httpContext);

            // Otherwise, return null, which will prevent this route from being matched for the request.
            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var virtualPathData = base.GetVirtualPath(requestContext, values);

            // Apply lowercase route convention.
            if (virtualPathData != null && Configuration.UseLowercaseRoutes)
                virtualPathData.VirtualPath = virtualPathData.VirtualPath.ToLowerInvariant();
            
            return virtualPathData;
        }
    }
}