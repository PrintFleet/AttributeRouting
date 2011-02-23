using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Extensions;

namespace AttributeRouting.Framework
{
    public class AttributeRouteBuilder
    {
        private readonly AttributeRoutingConfiguration _configuration;

        public AttributeRouteBuilder(AttributeRoutingConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        public IEnumerable<AttributeRoute> BuildRoutes()
        {
            var routeReflector = new RouteSpecificationBuilder(_configuration);
            var routeSpecs = routeReflector.BuildSpecifications();

            if (_configuration.TranslationProvider == null)
            {
                foreach (var routeSpec in routeSpecs)
                    yield return BuildRoute(routeSpec, null);
            }
            else
            {
                foreach (var routeSpec in routeSpecs)
                    foreach (var cultureName in _configuration.TranslationProvider.AvailableCultureNames)
                        yield return BuildRoute(routeSpec, cultureName);
            }
        }

        public AttributeRoute BuildRoute(RouteSpecification routeSpec, string cultureName)
        {
            var routeName = GetRouteName(routeSpec, cultureName);

            return new AttributeRoute(routeName,
                                      GetRouteUrl(routeSpec, cultureName),
                                      GetRouteDefaults(routeSpec),
                                      GetRouteConstraints(routeSpec, cultureName),
                                      GetRouteDataTokens(routeSpec, routeName),
                                      _configuration.UseLowercaseRoutes);
        }

        private string GetRouteName(RouteSpecification routeSpec, string cultureName)
        {
            var cultureNameArg = (cultureName.HasValue()) ? cultureName + "_" : null;

            if (routeSpec.RouteName.HasValue())
                return "{0}{1}".FormatWith(cultureNameArg, routeSpec.RouteName);

            if (_configuration.AutoGenerateRouteNames)
            {
                var areaArg = (routeSpec.AreaName.HasValue()) ? routeSpec.AreaName + "_" : null;
                return "{0}{1}{2}_{3}".FormatWith(cultureNameArg, areaArg, routeSpec.ControllerName, routeSpec.ActionName);
            }

            return null;
        }

        private string GetRouteUrl(RouteSpecification routeSpec, string cultureName)
        {
            var translationKeys = routeSpec.TranslationKeys;

            var url = (translationKeys == null)
                          ? routeSpec.Url
                          : GetTranslation(routeSpec.Url, translationKeys.RouteUrlKey, cultureName);

            var detokenizedUrl = DetokenizeUrl(url);
            var urlParameterNames = GetUrlParameterNames(detokenizedUrl);

            // {controller} and {action} tokens are not valid
            if (urlParameterNames.Any(n => n.ValueEquals("controller")))
                throw new AttributeRoutingException("{controller} is not a valid url parameter.");
            if (urlParameterNames.Any(n => n.ValueEquals("action")))
                throw new AttributeRoutingException("{action} is not a valid url parameter.");

            // Explicitly defined area routes are not valid
            if (urlParameterNames.Any(n => n.ValueEquals("area")))
                throw new AttributeRoutingException(
                    "{area} url parameters are not allowed. Specify the area name by using the RouteAreaAttribute.");

            var urlBuilder = new StringBuilder(detokenizedUrl);

            // If this is not an absolute url, prefix with a route prefix or area name
            if (!routeSpec.IsAbsoluteUrl)
            {
                var routePrefix = (translationKeys == null)
                                      ? routeSpec.RoutePrefix
                                      : GetTranslation(routeSpec.RoutePrefix, translationKeys.RoutePrefixUrlKey, cultureName);

                if (routePrefix.HasValue() && !routeSpec.Url.StartsWith(routePrefix))
                    urlBuilder.Insert(0, routePrefix + "/");

                var areaUrl = (translationKeys == null)
                                  ? routeSpec.AreaUrl
                                  : GetTranslation(routeSpec.AreaUrl, translationKeys.AreaUrlKey, cultureName);

                if (areaUrl.HasValue() && !routeSpec.Url.StartsWith(areaUrl))
                    urlBuilder.Insert(0, areaUrl + "/");
            }

            return urlBuilder.ToString().Trim('/');
        }

        private string GetTranslation(string defaultValue, string translationKey, string cultureName)
        {
            var translationProvider = _configuration.TranslationProvider;

            if (translationProvider == null || translationKey.IsBlank() || cultureName.IsBlank())
                return defaultValue;

            return translationProvider.GetTranslation(translationKey, cultureName) ?? defaultValue;
        }

        private RouteValueDictionary GetRouteDefaults(RouteSpecification routeSpec)
        {
            var defaults = new RouteValueDictionary
            {
                { "controller", routeSpec.ControllerName },
                { "action", routeSpec.ActionName }
            };

            foreach (var defaultAttribute in routeSpec.DefaultAttributes.Where(d => !defaults.ContainsKey(d.Key)))
                defaults.Add(defaultAttribute.Key, defaultAttribute.Value);

            // Inspect the url for optional parameters, specified with a leading ?
            var optionalParameterDefaults =
                from parameter in GetUrlParameterContents(routeSpec.Url)
                where parameter.StartsWith("?")
                let parameterName = parameter.TrimStart('?')
                select new RouteDefaultAttribute(parameterName, UrlParameter.Optional);

            foreach (var defautAttribute in optionalParameterDefaults.Where(d => !defaults.ContainsKey(d.Key)))
                defaults.Add(defautAttribute.Key, defautAttribute.Value);

            return defaults;
        }

        private RouteValueDictionary GetRouteConstraints(RouteSpecification routeSpec, string cultureName)
        {
            var constraints = new RouteValueDictionary();

            // Default constraints
            constraints.Add("httpMethod", new RestfulHttpMethodConstraint(routeSpec.HttpMethod));

            var translationProvider = _configuration.TranslationProvider;
            if (translationProvider != null)
            {
                constraints.Add("currentUICultureName",
                                new CurrentUICultureConstraint(cultureName,
                                                               translationProvider.DefaultCultureName,
                                                               translationProvider.AvailableCultureNames));
            }

            // Attribute-based constraints
            foreach (var constraintAttribute in routeSpec.ConstraintAttributes.Where(c => !constraints.ContainsKey(c.Key)))
                constraints.Add(constraintAttribute.Key, constraintAttribute.Constraint);

            var detokenizedUrl = DetokenizeUrl(GetRouteUrl(routeSpec, cultureName));
            var urlParameterNames = GetUrlParameterNames(detokenizedUrl);

            // Convention-based constraints
            foreach (var defaultConstraint in _configuration.DefaultRouteConstraints)
            {
                var pattern = defaultConstraint.Key;
                var matchedUrlParameterNames = urlParameterNames.Where(n => Regex.IsMatch(n, pattern));
                foreach (var urlParameterName in matchedUrlParameterNames.Where(n => !constraints.ContainsKey(n)))
                    constraints.Add(urlParameterName, defaultConstraint.Value);
            }

            return constraints;
        }

        private RouteValueDictionary GetRouteDataTokens(RouteSpecification routeSpec, string routeName)
        {
            var dataTokens = new RouteValueDictionary
            {
                { "namespaces", new[] { routeSpec.ControllerType.Namespace } }
            };

            if (routeSpec.AreaName.HasValue())
            {
                dataTokens.Add("area", routeSpec.AreaName);
                dataTokens.Add("UseNamespaceFallback", false);
            }

            if (routeName.HasValue())
                dataTokens.Add("RouteName", routeName);

            if (routeSpec.TranslationKeys != null)
            {
                var keys = routeSpec.TranslationKeys;
                
                if (keys.AreaUrlKey.HasValue())
                    dataTokens.Add("AreaUrlTranslationKey", routeSpec.TranslationKeys.AreaUrlKey);
                
                if (keys.RoutePrefixUrlKey.HasValue())
                    dataTokens.Add("RoutePrefixUrlTranslationKey", routeSpec.TranslationKeys.RoutePrefixUrlKey);

                if (keys.RouteUrlKey.HasValue())
                    dataTokens.Add("RouteUrlTranslationKey", routeSpec.TranslationKeys.RouteUrlKey);
            }

            return dataTokens;
        }

        private static string DetokenizeUrl(string url)
        {
            return Regex.Replace(url, @"\{\?", "{");
        }

        private static IEnumerable<string> GetUrlParameterNames(string url)
        {
            return (from match in Regex.Matches(url, @"(?<={)\w*(?=})").Cast<Match>()
                    select match.Captures[0].ToString()).ToList();
        }

        private static IEnumerable<string> GetUrlParameterContents(string url)
        {
            return (from match in Regex.Matches(url, @"(?<={).*?(?=})").Cast<Match>()
                    select match.Captures[0].ToString()).ToList();
        }
    }
}