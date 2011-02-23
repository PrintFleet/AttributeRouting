using System;
using System.Linq;
using System.Web.Routing;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AttributeRouting.Specs.Steps
{
    [Binding]
    public class SharedSteps
    {
        [Given(@"I generate the routes defined in the subject controllers")]
        public void GivenIGenerateTheRoutesDefinedInTheSubjectControllers()
        {
            RouteTable.Routes.Clear();
            RouteTable.Routes.MapAttributeRoutes();
        }

        [Given(@"I have a new configuration object")]
        public void GivenIHaveANewConfigurationObject()
        {
            ScenarioContext.Current.SetConfiguration(new AttributeRoutingConfiguration());
        }

        [Given(@"I add the routes from the (.*) controller")]
        public void GivenIAddTheRoutesFromTheController(string controllerName)
        {
            var controllerType = GetControllerType(controllerName);
            ScenarioContext.Current.GetConfiguration().AddRoutesFromController(controllerType);
        }

        [Given(@"I add the routes from controllers derived from the (.*) controller")]
        public void GivenIAddTheRoutesFromControllersOfTypeBaseController(string baseControllerName)
        {
            var baseControllerType = GetControllerType(baseControllerName);
            ScenarioContext.Current.GetConfiguration().AddRoutesFromControllersOfType(baseControllerType);
        }

        [Given(@"I generate the routes with this configuration")]
        [When(@"I generate the routes with this configuration")]
        public void WhenIGenerateTheRoutesWithThisConfiguration()
        {
            RouteTable.Routes.Clear();
            RouteTable.Routes.MapAttributeRoutes(ScenarioContext.Current.GetConfiguration());
        }

        [When(@"I fetch the routes for the (.*?) controller's (.*?) action")]
        public void WhenIFetchTheRoutesFor(string controllerName, string actionName)
        {
            var routes = from route in RouteTable.Routes.Cast<Route>()
                         where route.Defaults["controller"].ToString() == controllerName &&
                               route.Defaults["action"].ToString() == actionName
                         select route;

            ScenarioContext.Current.SetFetchedRoutes(routes);
        }

        [When(@"I fetch the routes for the (.*?) controller")]
        public void WhenIFetchTheRoutesFor(string controllerName)
        {
            var routes = from route in RouteTable.Routes.Cast<Route>()
                         where route.Defaults["controller"].ToString() == controllerName
                         select route;

            ScenarioContext.Current.SetFetchedRoutes(routes);
        }

        [Then(@"(.*?) routes? are found")]
        public void ThenNRoutesShouldBeFound(int n)
        {
            var routes = ScenarioContext.Current.GetFetchedRoutes();

            Assert.That(routes.Count(), Is.EqualTo(n));
        }

        [Then(@"the (?:(\d+)(?:st|nd|rd|th)\s)?route(?:'s)? url is ""(.*)""")]
        public void ThenTheRouteUrlIs(string nth, string url)
        {
            var route = GetNthRoute(nth);

            Assert.That(route, Is.Not.Null);
            Assert.That(route.Url, Is.EqualTo(url));
        }

        [Then(@"the (?:(\d+)(?:st|nd|rd|th)\s)?route(?:'s)? data token for ""(.*)"" is ""(.*)""")]
        public void ThenTheDataTokenForKeyIsValue(string nth, string key, string value)
        {
            var route = GetNthRoute(nth);

            Assert.That(route, Is.Not.Null);
            Assert.That(route.DataTokens.ContainsKey(key), Is.True);
            Assert.That(route.DataTokens[key], Is.EqualTo(value));
        }

        [Then(@"each route has a constraint with the key ""(.*)""")]
        public void ThenEachRouteHasAConstraintWithTheKey(string key)
        {
            var routes = ScenarioContext.Current.GetFetchedRoutes();

            foreach (var route in routes)
                Assert.That(route.Constraints.ContainsKey(key), Is.True);
        }

        [Then(@"the (?:(\d+)(?:st|nd|rd|th)\s)?route has (a|no) constraint with the key ""(.*)""")]
        public void ThenNthRouteHasYesNoConstraintWithTheKey(string nth, string aNo, string key)
        {
            var route = GetNthRoute(nth);

            Assert.That(route, Is.Not.Null, "No route to test.");
            Assert.That(route.Constraints.ContainsKey(key), Is.EqualTo(aNo == "a"));
        }

        private static Route GetNthRoute(string nth)
        {
            var i = nth.HasValue() ? int.Parse(nth) - 1 : 0;
            var routes = ScenarioContext.Current.GetFetchedRoutes();

            Assert.That(routes.Count(), Is.GreaterThan(i), "There is no {0} route available.", nth);

            return routes.ElementAt(i);
        }

        private static Type GetControllerType(string controllerName)
        {
            var typeName = String.Format("AttributeRouting.Specs.Subjects.{0}Controller, AttributeRouting.Specs",
                                         controllerName);

            var type = Type.GetType(typeName);

            Assert.That(type, Is.Not.Null, "The controller type \"{0}\" could not be resolved.", typeName);

            return type;
        }
    }
}
