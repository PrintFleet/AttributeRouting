using System.Collections.Generic;
using System.Linq;
using AttributeRouting.Framework;

namespace AttributeRouting.Web.Models
{
    public class RouteTranslationProvider : ITranslationProvider
    {
        private readonly IEnumerable<RouteTranslation> _routeTranslations;

        public RouteTranslationProvider(string defaultCultureName, IEnumerable<string> availableCultureNames, params RouteTranslation[] routeTranslations)
        {
            DefaultCultureName = defaultCultureName;
            AvailableCultureNames = availableCultureNames;

            _routeTranslations = routeTranslations;
        }

        public string DefaultCultureName { get; private set; }

        public IEnumerable<string> AvailableCultureNames { get; private set; }

        public string GetTranslation(string key, string cultureName)
        {
            if (_routeTranslations == null)
                return null;

            return (from t in _routeTranslations
                    where t.CultureName.ToLowerInvariant() == cultureName.ToLowerInvariant() &&
                          t.Key.ToLowerInvariant() == key.ToLowerInvariant()
                    select t.Value).FirstOrDefault();
        }
    }
}