using Mtf.LanguageService.Core.Enums;
using System;
using System.Collections.Generic;

namespace Mtf.LanguageService.Core.Attributes
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
