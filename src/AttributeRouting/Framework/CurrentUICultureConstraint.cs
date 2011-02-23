using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Routing;
using AttributeRouting.Extensions;

namespace AttributeRouting.Framework
{
    public class CurrentUICultureConstraint : IRouteConstraint
    {
        private readonly string _defaultCultureName;
        private readonly IEnumerable<string> _availableCultureNames;

        public CurrentUICultureConstraint(string cultureName, string defaultCultureName, IEnumerable<string> availableCultureNames)
        {
            CultureName = cultureName;
            _defaultCultureName = defaultCultureName;
            _availableCultureNames = availableCultureNames;
        }

        public string CultureName { get; private set; }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // Only apply the matching logic if generating urls.
            if (routeDirection == RouteDirection.IncomingRequest)
                return true;

            var currentCulture = Thread.CurrentThread.CurrentUICulture;

            // If the culture is available, then check whether this constraint is for the current culture
            var cultureIsAvailable = _availableCultureNames.Any(n => n.ValueEquals(currentCulture.Name));
            if (cultureIsAvailable)
                return currentCulture.Name.ValueEquals(CultureName);
        
            // If the language is available, check whether this constraint applies to the language
            var languageIsAvailable = _availableCultureNames.Any(n => n.ValueEquals(currentCulture.TwoLetterISOLanguageName));
            if (languageIsAvailable)
                return currentCulture.TwoLetterISOLanguageName.ValueEquals(CultureName);

            // If neither a culture or langaue is available, then apply the route with the default culture
            return CultureName.ValueEquals(_defaultCultureName);
        }
    }
}
