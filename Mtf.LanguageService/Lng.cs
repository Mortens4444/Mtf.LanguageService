using Mtf.LanguageService.Enums;
using Mtf.LanguageService.Interfaces;
using Mtf.LanguageService.Models;
using Mtf.LanguageService.Ods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Mtf.LanguageService
{
    public static class Lng
    {
        private const string LanguageFile = "Languages.ods";

        public static Language DefaultLanguage;

        public static readonly Dictionary<Translation, List<string>> AllLanguageElements;

        private static readonly ILanguageElementLoader languageElementLoader = new OdsLanguageElementLoader();

        static Lng()
        {
            SetDefaultLanguage();

            var languageFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, LanguageFile);
            if (languageFiles.Length != 0)
            {
                AllLanguageElements = languageElementLoader.LoadElements(languageFiles.First());
            }
            else
            {
                throw new InvalidOperationException($"Cannot find {LanguageFile} file in directory {AppDomain.CurrentDomain.BaseDirectory}. This file can be found in the packages\\Mtf.LanguageService.1.0.x\\lib folder of the solution.");
            }
        }

        private static void SetDefaultLanguage()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var languageName = currentCulture.EnglishName.Split(' ').First();
            DefaultLanguage = (Language)Enum.Parse(typeof(Language), languageName);
        }

        public static string Elem(string elementIdentifier, int index = 0)
        {
            return Elem(DefaultLanguage, elementIdentifier, index);
        }

        public static string FormattedElem(string elementIdentifier, int index = 0, params object[] args)
        {
            var elem = Elem(DefaultLanguage, elementIdentifier, index);
            return String.Format(elem, args);
        }

        public static string FormattedElem(Language toLanguage, string elementIdentifier, int index = 0, params object[] args)
        {
            var elem = Elem(toLanguage, elementIdentifier, index);
            return String.Format(elem, args);
        }

        /// <summary>
        /// Get a translation of an English expression.
        /// </summary>
        /// <param name="toLanguage">The language of the requested translation.</param>
        /// <param name="elementIdentifier">The requested element, which wanted to be translated (must be in English).</param>
        /// <param name="index">Index of the specified translations. If not set, it will return the first translation.</param>
        /// <returns>Returns the translation or the requested element itself, if it is not present in the dictionary.</returns>
        public static string Elem(Language toLanguage, string elementIdentifier, int index = 0)
        {
            var result = GetLanguageElement(elementIdentifier, index, toLanguage);
            if (String.IsNullOrEmpty(result))
            {
                result = GetLanguageElement(elementIdentifier, index);
            }
            return String.IsNullOrEmpty(result) ? elementIdentifier : result;
        }

        /// <summary>
        /// Get a translation of an expression.
        /// </summary>
        /// <param name="fromLanguage">The language of the language element.</param>
        /// <param name="languageElement">The language element, which is needed to be translated.</param>
        /// <param name="toLanguage">The translation destination language.</param>
        /// <returns>The translated element if it's translation exists, otherwise the language element itself is returned.</returns>
        public static string Translate(Language fromLanguage, string languageElement, Language toLanguage)
        {
            foreach (var keyValuePair in AllLanguageElements.Where(elem => elem.Key.Language == fromLanguage))
            {
                if (keyValuePair.Value.Any(elem => elem == languageElement))
                {
                    if (toLanguage == Language.English)
                    {
                        return keyValuePair.Key.ElementIdentifier;
                    }

                    return GetLanguageElement(keyValuePair.Key.ElementIdentifier, 0, toLanguage);
                }
            }

            return languageElement;
        }

        private static string GetLanguageElement(string elementIdentifier, int index, Language language = Language.English)
        {
            var key = new Translation(language, elementIdentifier);
            return AllLanguageElements != null && AllLanguageElements.TryGetValue(key, out var value) ? value[index] : null;
        }
    }
}
