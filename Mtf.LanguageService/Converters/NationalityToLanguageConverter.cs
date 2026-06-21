using Mtf.Extensions;
using Mtf.LanguageService.Attributes;
using Mtf.LanguageService.Enums;
using Mtf.LanguageService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtf.LanguageService.Converters
{
    public class NationalityToLanguageConverter : INationalityToLanguageConverter
    {
        public Language Convert(Nationality nationality)
        {
            var nationalityEnum = nationality as Enum;
            var firstDefaultLanguageAttribute = nationalityEnum.GetSingleEnumAttribute<DefaultLanguagesAttribute>("Languages");
            if (firstDefaultLanguageAttribute is IEnumerable<Language> defaultLanguages)
            {
                return defaultLanguages.First();
            }
            return Language.English;
        }
    }
}
