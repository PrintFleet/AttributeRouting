using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AttributeRouting.Framework;

namespace AttributeRouting.Specs.Subjects
{
    public class TestTranslationProvider : ITranslationProvider
    {
        private readonly List<Translation> _translations;

        public TestTranslationProvider(string defaultCultureName, IEnumerable<string> availableCultureNames)
        {
            DefaultCultureName = defaultCultureName;
            AvailableCultureNames = availableCultureNames;

            _translations = new List<Translation>();
        }

        public string DefaultCultureName { get; private set; }

        public IEnumerable<string> AvailableCultureNames { get; private set; }

        public string GetTranslation(string key, string cultureName)
        {
            return (from t in _translations
                    where t.CultureName.ToLowerInvariant() == cultureName.ToLowerInvariant() &&
                          t.Key.ToLowerInvariant() == key.ToLowerInvariant()
                    select t.Value).FirstOrDefault();
        }

        public void AddTranslation(Translation translation)
        {
            _translations.Add(translation);
        }
    }

    public class Translation
    {
        public Translation(string key, string value, string cultureName)
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
