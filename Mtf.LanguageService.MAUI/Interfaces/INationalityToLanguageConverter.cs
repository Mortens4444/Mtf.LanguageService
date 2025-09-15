using Mtf.LanguageService.Enums;

namespace Mtf.LanguageService.Interfaces
{
    public interface INationalityToLanguageConverter
    {
        Language Convert(Nationality nationality);
    }
}
