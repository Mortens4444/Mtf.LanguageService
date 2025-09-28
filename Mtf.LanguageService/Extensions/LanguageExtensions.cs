using Mtf.Extensions;
using System;
using System.Linq;

namespace Mtf.LanguageService.Extensions
{
    public static class LanguageExtensions
    {
        public static Enums.ImplementedLanguage ToImplementedLanguage(this Enums.Language lang)
        {
            var name = lang.ToString();
            foreach (var v in Enum.GetValues(typeof(Enums.ImplementedLanguage)).Cast<Enums.ImplementedLanguage>())
            {
                if (String.Equals(v.ToString(), name, StringComparison.OrdinalIgnoreCase))
                {
                    return v;
                }
            }

            var desc = lang.GetDescription();
            foreach (var v in Enum.GetValues(typeof(Enums.ImplementedLanguage)).Cast<Enums.ImplementedLanguage>())
            {
                if (String.Equals(v.GetDescription(), desc, StringComparison.Ordinal))
                {
                    return v;
                }
            }

            return Enums.ImplementedLanguage.English;
        }

        public static Enums.Language ToLanguage(this Enums.ImplementedLanguage impl)
        {
            var name = impl.ToString();
            foreach (var v in Enum.GetValues(typeof(Enums.Language)).Cast<Enums.Language>())
            {
                if (String.Equals(v.ToString(), name, StringComparison.OrdinalIgnoreCase))
                {
                    return v;
                }
            }

            var desc = impl.GetDescription();
            foreach (var v in Enum.GetValues(typeof(Enums.Language)).Cast<Enums.Language>())
            {
                if (String.Equals(v.GetDescription(), desc, StringComparison.Ordinal))
                {
                    return v;
                }
            }

            return Enums.Language.English;
        }
    }
}
