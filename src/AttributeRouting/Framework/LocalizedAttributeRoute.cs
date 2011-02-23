using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Routing;

namespace AttributeRouting.Framework
{
    public class CultureNameConstraint : IRouteConstraint
    {
        private readonly string _cultureName;

        public CultureNameConstraint(string cultureName)
        {
            _cultureName = cultureName;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return Thread.CurrentThread.CurrentUICulture.Name == _cultureName;
        }
    }
}
