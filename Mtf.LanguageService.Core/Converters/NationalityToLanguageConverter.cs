using Mtf.Extensions;
using Mtf.LanguageService.Core.Attributes;
using Mtf.LanguageService.Core.Enums;
using Mtf.LanguageService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtf.LanguageService.Core.Converters
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
