using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AttributeRouting.Extensions
{
    internal static class StringExtensions
    {
        public static bool ValueEquals(this string s, string other)
        {
            return s.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasValue(this string s)
        {
            return !String.IsNullOrWhiteSpace(s);
        }

        public static bool IsBlank(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static string FormatWith(this string s, params object[] args)
        {
            return String.Format(s, args);
        }

        public static bool IsValidUrl(this string s, bool allowTokens = false)
        {
            var urlParts = s.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            var invalidUrlPattern = FormatWith(@"[#%&\*\\:<>/\+{0}]|\.\.|\.$|^ | $", allowTokens ? "" : @"\{\}?");

            return !urlParts.Any(p => Regex.IsMatch(p, invalidUrlPattern));
        }
    }
}