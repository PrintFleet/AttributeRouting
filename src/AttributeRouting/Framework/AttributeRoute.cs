using System.Web.Mvc;
using System.Web.Routing;

namespace AttributeRouting.Framework
{
    public class AttributeRoute : Route
    {
        private readonly bool _useLowercaseRoutes;

        public AttributeRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                              RouteValueDictionary dataTokens, bool useLowercaseRoutes, IRouteHandler handler)
            : this(null, url, defaults, constraints, dataTokens, useLowercaseRoutes, handler) {}

        public AttributeRoute(string name, string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                              RouteValueDictionary dataTokens, bool useLowercaseRoutes, IRouteHandler handler)
            : base(url, defaults, constraints, dataTokens, handler)
        {
            Name = name;
            _useLowercaseRoutes = useLowercaseRoutes;
        }

        public string Name { get; set; }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var data = base.GetVirtualPath(requestContext, values);
            
            if (_useLowercaseRoutes && data != null)
                data.VirtualPath = data.VirtualPath.ToLowerInvariant();

            return data;
        }
    }
}