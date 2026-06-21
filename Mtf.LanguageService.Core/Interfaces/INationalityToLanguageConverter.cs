using Mtf.LanguageService.Core.Enums;

namespace Mtf.LanguageService.Core.Interfaces
{
    public interface INationalityToLanguageConverter
    {
        Language Convert(Nationality nationality);
    }
}
