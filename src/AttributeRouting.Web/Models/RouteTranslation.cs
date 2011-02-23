namespace AttributeRouting.Web.Models
{
    public class RouteTranslation
    {
        public RouteTranslation(string key, string value, string cultureName)
        {
            Key = key;
            Value = value;
            CultureName = cultureName;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string CultureName { get; set; }
    }
}
