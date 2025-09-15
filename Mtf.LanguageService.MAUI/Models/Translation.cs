using Mtf.LanguageService.Enums;
using System;

namespace Mtf.LanguageService.Models
{
    public class Translation
    {
        public Language Language { get; private set; }

        public string ElementIdentifier { get; private set; }

        public Translation(Language language, string elementIdentifier)
        {
            Language = language;
            ElementIdentifier = elementIdentifier;
        }

        public override bool Equals(object obj)
        {
            if (obj is Translation translation)
            {
                return translation.ElementIdentifier == ElementIdentifier && translation.Language == translation.Language;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Language.GetHashCode();
                hash = hash * 23 + (ElementIdentifier?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
