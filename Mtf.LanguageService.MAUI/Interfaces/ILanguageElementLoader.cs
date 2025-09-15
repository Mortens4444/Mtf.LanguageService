using Mtf.LanguageService.Models;
using System.Collections.Generic;

namespace Mtf.LanguageService.Interfaces
{
    public interface ILanguageElementLoader
    {
        Dictionary<Translation, List<string>> LoadElements(string filePath);
    }
}
