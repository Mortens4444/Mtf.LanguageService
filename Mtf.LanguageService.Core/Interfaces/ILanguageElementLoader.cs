using Mtf.LanguageService.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace Mtf.LanguageService.Core.Interfaces
{
    public interface ILanguageElementLoader
    {
        Dictionary<Translation, List<string>> LoadElements(string filePath);

        Dictionary<Translation, List<string>> LoadElements(Stream stream);
    }
}
