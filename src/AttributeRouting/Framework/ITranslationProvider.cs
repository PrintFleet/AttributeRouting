using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AttributeRouting.Framework
{
    public interface ITranslationProvider
    {
        string DefaultCultureName { get; }
        IEnumerable<string> AvailableCultureNames { get; }
        
        string GetTranslation(string key, string cultureName);
    }
}
