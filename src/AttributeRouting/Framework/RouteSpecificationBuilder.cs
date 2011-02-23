using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AttributeRouting.Extensions;

namespace AttributeRouting.Framework
{
    public class RouteSpecificationBuilder
    {
        private readonly AttributeRoutingConfiguration _configuration;

        public RouteSpecificationBuilder(AttributeRoutingConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        public IEnumerable<RouteSpecification> BuildSpecifications()
        {
            var controllerRouteSpecs = BuildSpecifications(_configuration.PromotedControllerTypes);
            foreach (var spec in controllerRouteSpecs)
                yield return spec;

            if (_configuration.AddScannedRoutes)
            {
                var scannedControllerTypes = _configuration.Assemblies.SelectMany(a => a.GetControllerTypes()).ToList();
                var remainingControllerTypes = scannedControllerTypes.Except(_configuration.PromotedControllerTypes);

                var remainingRouteSpecs = BuildSpecifications(remainingControllerTypes);

                foreach (var spec in remainingRouteSpecs)
                    yield return spec;
            }
        }

        private IEnumerable<RouteSpecification> BuildSpecifications(IEnumerable<Type> controllerTypes)
        {
            var controllerCount = 0;

            var routes = (from controllerType in controllerTypes
                          let controllerIndex = controllerCount++
                          let conventionAttribute = controllerType.GetCustomAttribute<RouteConventionAttribute>(false)
                          from actionMethod in controllerType.GetActionMethods()
                          let routeAttributes = GetRouteAttributes(actionMethod, conventionAttribute)
                          from routeAttribute in routeAttributes
                          orderby controllerIndex , routeAttribute.Precedence
                          select new
                          {
                              ControllerType = controllerType,
                              ActionMethod = actionMethod,
                              ActionRoutesCount = routeAttributes.Count(),
                              RouteAttribute = routeAttribute,
                              ConventionAttribute = conventionAttribute
                          });

            var routeSpecs = new List<RouteSpecification>();
            MethodInfo previousActionMethod = null;
            var actionRouteIndex = 0;

            foreach (var route in routes)
            {
                var controllerType = route.ControllerType;
                var conventionAttribute = route.ConventionAttribute;
                var actionMethod = route.ActionMethod;
                var routeAttribute = route.RouteAttribute;
                var routeName = route.RouteAttribute.RouteName;
                var routeAreaAttribute = actionMethod.DeclaringType.GetCustomAttribute<RouteAreaAttribute>(true);
                var routePrefixAttribute = actionMethod.DeclaringType.GetCustomAttribute<RoutePrefixAttribute>(true);
                var actionRoutesCount = route.ActionRoutesCount;

                if (previousActionMethod == null)
                    previousActionMethod = actionMethod;
                else if (previousActionMethod == actionMethod)
                    actionRouteIndex++;
                else
                {
                    previousActionMethod = actionMethod;
                    actionRouteIndex = 0;
                }
                
                var routeSpec = new RouteSpecification();

                routeSpec.AreaName = GetAreaName(routeAreaAttribute);
                routeSpec.AreaUrl = GetAreaUrl(routeAreaAttribute);
                routeSpec.RoutePrefix = GetRoutePrefix(routePrefixAttribute, conventionAttribute, actionMethod);
                routeSpec.ControllerType = controllerType;
                routeSpec.ControllerName = controllerType.GetControllerName();
                routeSpec.ActionName = actionMethod.Name;
                routeSpec.ActionParameters = actionMethod.GetParameters();

                routeSpec.Url = routeAttribute.Url;
                routeSpec.HttpMethod = routeAttribute.HttpMethod;
                routeSpec.DefaultAttributes = GetDefaultAttributes(actionMethod, routeName, conventionAttribute);
                routeSpec.ConstraintAttributes = GetConstraintAttributes(actionMethod, routeName, conventionAttribute);
                routeSpec.RouteName = routeName;
                routeSpec.IsAbsoluteUrl = routeAttribute.IsAbsoluteUrl;

                routeSpec.TranslationKeys = GetTranslationKeys(actionRoutesCount, actionRouteIndex,
                                                               routeAttribute, routeAreaAttribute, routePrefixAttribute,
                                                               routeSpec.ControllerName, routeSpec.ActionName);

                routeSpecs.Add(routeSpec);
            }

            return routeSpecs;
        }

        private RouteTranslationKeys GetTranslationKeys(int actionRoutesCount, int actionRouteIndex, RouteAttribute routeAttribute, RouteAreaAttribute routeAreaAttribute, RoutePrefixAttribute routePrefixAttribute, string controllerName, string actionName)
        {
            if (_configuration.TranslationProvider == null)
                return null;

            var translationKeys = new RouteTranslationKeys();

            string areaName = null;
            if (routeAreaAttribute != null)
            {
                areaName = GetAreaName(routeAreaAttribute);
                translationKeys.AreaUrlKey = routeAreaAttribute.AreaUrlTranslationKey ??
                                             "{0}_AreaUrl".FormatWith(areaName);
            }

            var areaArg = (areaName.HasValue()) ? areaName + "_" : null;

            if (routePrefixAttribute != null)
            {
                translationKeys.RoutePrefixUrlKey = routePrefixAttribute.UrlTranslationKey ??
                                                    "{0}{1}_RoutePrefixUrl".FormatWith(areaArg, controllerName);
            }

            var routeIndexArg = (actionRoutesCount > 1) ? (actionRouteIndex + 1) + "_" : null;

            translationKeys.RouteUrlKey = routeAttribute.UrlTranslationKey ??
                                          "{0}{1}_{2}_{3}RouteUrl".FormatWith(areaArg, controllerName, actionName, routeIndexArg);
            
            return translationKeys;
        }

        private static IEnumerable<RouteAttribute> GetRouteAttributes(MethodInfo actionMethod,
                                                                      RouteConventionAttribute convention)
        {
            var attributes = new List<RouteAttribute>();

            // Add convention-based attributes
            if (convention != null)
                attributes.AddRange(convention.GetRouteAttributes(actionMethod));

            // Add explicitly-defined attributes
            attributes.AddRange(actionMethod.GetCustomAttributes<RouteAttribute>(false));

            return attributes.OrderBy(a => a.Order);
        }

        private static string GetAreaName(RouteAreaAttribute routeAreaAttribute)
        {
            if (routeAreaAttribute == null)
                return null;

            return routeAreaAttribute.AreaName;
        }

        private static string GetAreaUrl(RouteAreaAttribute routeAreaAttribute)
        {
            if (routeAreaAttribute == null)
                return null;

            return routeAreaAttribute.AreaUrl ?? routeAreaAttribute.AreaName;
        }

        private static string GetRoutePrefix(RoutePrefixAttribute routePrefixAttribute, RouteConventionAttribute convention, MethodInfo actionMethod)
        {
            // Return an explicitly defined route prefix, if defined
            if (routePrefixAttribute != null)
                return routePrefixAttribute.Url;

            // Otherwise, if this is a convention-based controller, get the convention-based prefix
            if (convention != null)
                return convention.GetDefaultRoutePrefix(actionMethod);

            return "";
        }

        private static ICollection<RouteDefaultAttribute> GetDefaultAttributes(MethodInfo actionMethod, string routeName,
                                                                               RouteConventionAttribute convention)
        {
            var defaultAttributes = new List<RouteDefaultAttribute>();

            // Yield explicitly defined default attributes first
            defaultAttributes.AddRange(
                from defaultAttribute in actionMethod.GetCustomAttributes<RouteDefaultAttribute>(false)
                where !defaultAttribute.ForRouteNamed.HasValue() ||
                      defaultAttribute.ForRouteNamed == routeName
                select defaultAttribute);

            // Yield convention-based defaults next
            if (convention != null)
                defaultAttributes.AddRange(convention.GetRouteDefaultAttributes(actionMethod));

            return defaultAttributes.ToList();
        }

        private static ICollection<RouteConstraintAttribute> GetConstraintAttributes(MethodInfo actionMethod,
                                                                                     string routeName,
                                                                                     RouteConventionAttribute convention)
        {
            var constraintAttributes = new List<RouteConstraintAttribute>();

            // Yield explicitly defined constraint attributes first
            constraintAttributes.AddRange(
                from constraintAttribute in actionMethod.GetCustomAttributes<RouteConstraintAttribute>(false)
                where !constraintAttribute.ForRouteNamed.HasValue() ||
                      constraintAttribute.ForRouteNamed == routeName
                select constraintAttribute);

            // Yield convention-based constraints next
            if (convention != null)
                constraintAttributes.AddRange(convention.GetRouteConstraintAtributes(actionMethod));

            return constraintAttributes.ToList();
        }
    }
}