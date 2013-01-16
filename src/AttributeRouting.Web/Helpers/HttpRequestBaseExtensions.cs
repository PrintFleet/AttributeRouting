using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using AttributeRouting.Helpers;

namespace AttributeRouting.Web.Helpers
{
    public static class HttpRequestBaseExtensions
    {
        private static bool _isSystemWebWebPagesUnavailable;

        public static string GetFormValue(this HttpRequestBase request, string key)
        {
            return request.GetUnvalidatedCollectionValue("Form", key) ?? request.Form[key];
        }

        public static string GetQueryStringValue(this HttpRequestBase request, string key)
        {
            return request.GetUnvalidatedCollectionValue("QueryString", key) ?? request.QueryString[key];
        }

        /// <summary>
        /// Loads the Form or QueryString collection value from the unvalidated object in System.Web.Webpages, 
        /// if that assembly is available.
        /// </summary>
        private static string GetUnvalidatedCollectionValue(this HttpRequestBase request, string unvalidatedObjectPropertyName, string key)
        {
            if (_isSystemWebWebPagesUnavailable)
                return null;

            try
            {
                var webPagesAssembly = Assembly.Load("System.Web.WebPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                var validationType = webPagesAssembly.GetType("System.Web.Helpers.Validation");
                var unvalidatedMethod = validationType.GetMethod("Unvalidated", new[] { request.GetType() });
                var unvalidatedObject = unvalidatedMethod.Invoke(null, new[] { request });
                var collectionProperty = unvalidatedObject.GetType().GetProperty(unvalidatedObjectPropertyName);
                var collection = (NameValueCollection)collectionProperty.GetValue(unvalidatedObject, null);

                return collection[key];
            }
            catch
            {
                _isSystemWebWebPagesUnavailable = true;

                return null;
            }
        }

        /// <remarks>
        /// The reason we have our own is to provide support for System.Web.Helpers.Validation.Unvalidated calls.
        /// </remarks>
        public static string GetHttpMethod(this HttpRequestBase request)
        {
            var httpMethod = request.HttpMethod;

            // If not a post, method overrides don't apply.
            if (!httpMethod.ValueEquals("POST"))
            {
                return httpMethod;
            }

            // Get the method override and if it's not for a GET or POST, then apply it.
            var methodOverride = request.SafeGet(r => r.Headers["X-HTTP-Method-Override"]) ??
                                 request.SafeGet(r => GetFormValue(r, "X-HTTP-Method-Override")) ??
                                 request.SafeGet(r => GetQueryStringValue(r, "X-HTTP-Method-Override"));

            if (methodOverride != null &&
                (!methodOverride.ValueEquals("GET") && !methodOverride.ValueEquals("POST")))
            {
                return methodOverride;
            }

            // Otherwise, just return the http method.
            return httpMethod;
        }
    }
}
