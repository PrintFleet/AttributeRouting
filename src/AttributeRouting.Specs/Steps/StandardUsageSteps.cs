using System.Linq;
using AttributeRouting.Framework;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AttributeRouting.Specs.Steps
{
    [Binding]
    public class StandardUsageSteps
    {
        [Then(@"the default for ""(.*?)"" is ""(.*?)""")]
        public void ThenTheDefaultForIs(string key, object value)
        {
            var route = ScenarioContext.Current.GetFetchedRoutes().FirstOrDefault();

            Assert.That(route, Is.Not.Null);
            Assert.That(route.Defaults[key], Is.EqualTo(value));
        }

        [Then(@"the namespace is ""(.*?)""")]
        public void ThenTheNamespaceIs(string ns)
        {
            var route = ScenarioContext.Current.GetFetchedRoutes().FirstOrDefault();

            Assert.That(route, Is.Not.Null);
            Assert.That(route.DataTokens["namespaces"], Is.EqualTo(new[] { ns }));
        }

        [Then(@"the route is constrained to (.*?) requests")]
        public void ThenTheRouteIsConstrainedToRequests(string method)
        {
            var route = ScenarioContext.Current.GetFetchedRoutes().FirstOrDefault();

            Assert.That(route, Is.Not.Null);

            var constraint = route.Constraints["httpMethod"] as RestfulHttpMethodConstraint;

            Assert.That(constraint, Is.Not.Null);
            Assert.That(constraint.AllowedMethods.Count, Is.EqualTo(1));
            Assert.That(constraint.AllowedMethods.First(), Is.EqualTo(method));
        }

        [Then(@"the route for (.*?) is constrained to (.*?) requests")]
        public void ThenTheRouteForIsConstrainedToRequests(string action, string method)
        {
            var route = ScenarioContext.Current.GetFetchedRoutes().FirstOrDefault(r => r.Defaults["action"].ToString() == action);

            Assert.That(route, Is.Not.Null);

            var constraint = route.Constraints["httpMethod"] as RestfulHttpMethodConstraint;

            Assert.That(constraint, Is.Not.Null);
            Assert.That(constraint.AllowedMethods.Count, Is.EqualTo(1));
            Assert.That(constraint.AllowedMethods.First(), Is.EqualTo(method));
        }
    }
}
