using Mtf.LanguageService.Enums;
using System;
using System.Collections.Generic;

namespace Mtf.LanguageService.Attributes
{
    public class DefaultLanguagesAttribute : Attribute
    {
        public IEnumerable<Language> Languages { get; internal set; }

        public DefaultLanguagesAttribute(params Language[] languages)
        {
            Languages = languages;
        }
    }
}
