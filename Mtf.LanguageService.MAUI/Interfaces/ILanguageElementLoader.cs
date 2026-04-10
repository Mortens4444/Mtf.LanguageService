using Mtf.LanguageService.MAUI.Models;

namespace Mtf.LanguageService.MAUI.Interfaces
{
    public interface ILanguageElementLoader
    {
        Dictionary<Translation, List<string>> LoadElements(string filePath);

        Dictionary<Translation, List<string>> LoadElements(Stream stream);
    }
}
