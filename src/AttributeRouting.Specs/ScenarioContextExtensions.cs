using System.Collections.Generic;
using System.Web.Routing;
using TechTalk.SpecFlow;

namespace AttributeRouting.Specs
{
    public static class ScenarioContextExtensions
    {
        public static void SetFetchedRoutes(this ScenarioContext context, IEnumerable<Route> routes)
        {
            context.Set(routes, "FetchedRoutes");
        }

        public static IEnumerable<Route> GetFetchedRoutes(this ScenarioContext context)
        {
            return context.Get<IEnumerable<Route>>("FetchedRoutes");
        }
        
        public static void SetConfiguration(this ScenarioContext context, AttributeRoutingConfiguration configuration)
        {
            context.Set(configuration, "Configuration");
        }

        public static AttributeRoutingConfiguration GetConfiguration(this ScenarioContext context)
        {
            return context.Get<AttributeRoutingConfiguration>("Configuration");
        }
        
        public static void SetRouteData(this ScenarioContext context, RouteData routeData)
        {
            context.Set(routeData, "RouteData");
        }

        public static RouteData GetRouteData(this ScenarioContext context)
        {
            return context.Get<RouteData>("RouteData");
        }
    }
}
