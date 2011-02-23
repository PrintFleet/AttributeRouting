using System;
using System.Text.RegularExpressions;
using System.Web;

namespace AttributeRouting.Extensions
{
    internal static class MvcExtensions
    {
        public static string GetControllerName(this Type type)
        {
            return Regex.Replace(type.Name, "Controller$", "");
        }

        public static string GetHttpMethod(this HttpRequestBase request)
        {
            return request.Headers.SafeGet(h => h["X-HTTP-Method-Override"]) ??
                   request.Form.SafeGet(f => f["X-HTTP-Method-Override"]) ??
                   request.QueryString.SafeGet(q => q["X-HTTP-Method-Override"]) ??
                   request.HttpMethod;
        }
    }
}