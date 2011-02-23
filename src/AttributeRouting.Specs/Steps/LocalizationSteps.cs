using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Specs.Subjects;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AttributeRouting.Specs.Steps
{
    [Binding]
    public class LocalizationSteps
    {
        private string _generatedUrl;

        [Given(@"I add the routes from the subject controllers")]
        public void GivenIAddTheRoutesFromTheSubjectControllers()
        {
            var configuration = ScenarioContext.Current.GetConfiguration();
            configuration.ScanAssemblyOf<LocalizationController>();
        }

        [Given(@"I configure a new TestTranslationProvider with:")]
        public void GivenIConfigureANewTestTranslationProvider(Table data)
        {
            var translations = data.Rows.Select(row => new Translation(row["key"], row["value"], row["cultureName"]));
            var availableCultureNames = translations.Select(t => t.CultureName).Distinct();
            var defaultCultureName = availableCultureNames.FirstOrDefault();

            var translationProvider = new TestTranslationProvider(defaultCultureName, availableCultureNames);
            foreach (var translation in translations)
                translationProvider.AddTranslation(translation);

            var configuration = ScenarioContext.Current.GetConfiguration();
            configuration.TranslationProvider = translationProvider;
        }

        [Given(@"I set the current thread's CurrentUICulture to ""(.*)""")]
        public void WhenISetTheCurrentThreadSCurrentUICultureToCultureName(string cultureName)
        {
            var cultureInfo = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        [When(@"I generate the url for the (.*) controller's (.*) action")]
        public void WhenIGenerateTheUrlForTheControllerAndAction(string controllerName, string actionName)
        {
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(r => r.ApplicationPath).Returns("/");
            mockRequest.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/");
            mockRequest.Setup(r => r.PathInfo).Returns("");
            mockRequest.Setup(r => r.ServerVariables).Returns(new NameValueCollection());

            var mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object); 
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object); 

            var routeData = new RouteData();
            routeData.Values.Add("controller", "stub");
            routeData.Values.Add("action", "stub");

            var requestContext = new RequestContext(mockHttpContext.Object, routeData);
            var urlHelper = new UrlHelper(requestContext, RouteTable.Routes);
            
            _generatedUrl = urlHelper.Action(actionName, controllerName);
        }

        [Then(@"the generated url is ""(.*)""")]
        public void ThenTheGeneratedUrlIs(string url)
        {
            Assert.That(_generatedUrl, Is.Not.Null);
            Assert.That(_generatedUrl, Is.EqualTo(url));
        }
    }
}
