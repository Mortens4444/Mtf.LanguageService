using Mtf.LanguageService.Models;

namespace Mtf.LanguageService.Interfaces
{
    public interface ILanguageElementLoader
    {
        Dictionary<Translation, List<string>> LoadElements(string filePath);

        Dictionary<Translation, List<string>> LoadElements(Stream stream);
    }
}
