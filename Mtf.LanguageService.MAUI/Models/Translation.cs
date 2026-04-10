using Mtf.LanguageService.Enums;

namespace Mtf.LanguageService.MAUI.Models
{
    public class Translation(Language language, string elementIdentifier)
    {
        public Language Language { get; private set; } = language;

        public string ElementIdentifier { get; private set; } = elementIdentifier;

        public override bool Equals(object? obj)
        {
            if (obj is Translation translation)
            {
                return translation.ElementIdentifier == ElementIdentifier && translation.Language == Language;
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
