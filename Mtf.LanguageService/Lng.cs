using Mtf.LanguageService.Enums;
using Mtf.LanguageService.Models;
using Mtf.LanguageService.Ods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Mtf.LanguageService
{
    public static class Lng
    {
        private const string LanguageFile = "Languages.ods";

        private static Language defaultLanguage = Language.English;

        public static Language DefaultLanguage
        {
            get
            {
                return defaultLanguage;
            }
            set
            {
                if (defaultLanguage != value)
                {
                    defaultLanguage = value;
                    LanguageChanged?.Invoke();
                }
            }
        }

        public static bool IsRtl => DefaultLanguage == Language.Arabic || DefaultLanguage == Language.Hebrew;

        private static readonly OdsLanguageElementLoader languageElementLoader = new OdsLanguageElementLoader();

        private static readonly Lazy<Dictionary<Translation, List<string>>> _allLanguageElementsLazy = new Lazy<Dictionary<Translation, List<string>>>(() =>
        {
            // Deferred load: load from embedded resource first, otherwise from file system.
            var asm = typeof(Lng).Assembly;
            var names = asm.GetManifestResourceNames();

            var resourceName = names.FirstOrDefault(n => n.EndsWith(LanguageFile, StringComparison.OrdinalIgnoreCase) || n.Contains("Languages.ods", StringComparison.OrdinalIgnoreCase));
            if (resourceName != null)
            {
                using var stream = asm.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Resource {resourceName} found but stream is null.");
                return languageElementLoader.LoadElements(stream);
            }

            var languageFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, LanguageFile);
            if (languageFiles.Length != 0)
            {
                return languageElementLoader.LoadElements(languageFiles.First());
            }

            throw new InvalidOperationException($"Cannot find {LanguageFile} file as embedded resource or in directory {AppDomain.CurrentDomain.BaseDirectory}.");
        }, isThreadSafe: true);

        public static Dictionary<Translation, List<string>> AllLanguageElements => _allLanguageElementsLazy.Value;

        public static event Action? LanguageChanged;

        static Lng()
        {
            // Only set default language during type init; defer heavy file parsing until needed via AllLanguageElements property.
            SetDefaultLanguage();
        }

        private static void SetDefaultLanguage()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var languageName = currentCulture.EnglishName.Split(' ').First();
            try
            {
                DefaultLanguage = Enum.TryParse<Language>(languageName, out var language) ? language : Language.English;
            }
            catch
            {
                DefaultLanguage = Language.English;
            }
        }

        /// <summary>
        /// Returns the translated text for the specified element identifier 
        /// using the current default language.
        /// </summary>
        /// <param name="elementIdentifier">
        /// The identifier of the element to translate. 
        /// The identifier must correspond to the base (English) key.
        /// </param>
        /// <param name="index">
        /// Optional index of the translation variant. 
        /// If not specified, the first available translation is returned.
        /// </param>
        /// <returns>
        /// The translated string if found; otherwise the original 
        /// <paramref name="elementIdentifier"/> value.
        /// </returns>
        public static string Elem(string elementIdentifier, int index = 0)
        {
            return Elem(DefaultLanguage, elementIdentifier, index);
        }

        /// <summary>
        /// Returns a formatted translated string using the current default language.
        /// </summary>
        /// <param name="elementIdentifier">
        /// The identifier of the element to translate. 
        /// The identifier must correspond to the base (English) key.
        /// </param>
        /// <param name="args">
        /// Formatting arguments that will be applied using 
        /// <see cref="string.Format(string, object[])"/>.
        /// </param>
        /// <returns>
        /// The formatted translated string if found; otherwise the formatted 
        /// <paramref name="elementIdentifier"/>.
        /// </returns>
        public static string FormattedElem(string elementIdentifier, params object[] args)
        {
            var elem = Elem(DefaultLanguage, elementIdentifier);
            return String.Format(elem, args);
        }

        /// <summary>
        /// Returns a formatted translated string using the current default language.
        /// </summary>
        /// <param name="elementIdentifier">
        /// The identifier of the element to translate. 
        /// The identifier must correspond to the base (English) key.
        /// </param>
        /// <param name="index">
        /// Optional index of the translation variant. 
        /// If not specified, the first available translation is returned.
        /// </param>
        /// <param name="args">
        /// Formatting arguments that will be applied using <see cref="string.Format(string, object[])"/>.
        /// </param>
        /// <returns>
        /// The formatted translated string if found; otherwise the formatted 
        /// <paramref name="elementIdentifier"/>.
        /// </returns>
        public static string FormattedElem(string elementIdentifier, int index = 0, params object[] args)
        {
            var elem = Elem(DefaultLanguage, elementIdentifier, index);
            return String.Format(elem, args);
        }

        /// <summary>
        /// Returns a formatted translated string in the specified language.
        /// </summary>
        /// <param name="toLanguage">
        /// The target language of the translation.
        /// </param>
        /// <param name="elementIdentifier">
        /// The identifier of the element to translate. 
        /// The identifier must correspond to the base (English) key.
        /// </param>
        /// <param name="index">
        /// Optional index of the translation variant. 
        /// If not specified, the first available translation is returned.
        /// </param>
        /// <param name="args">
        /// Formatting arguments that will be applied using <see cref="string.Format(string, object[])"/>.
        /// </param>
        /// <returns>
        /// The formatted translated string if found; otherwise the formatted 
        /// <paramref name="elementIdentifier"/>.
        /// </returns>
        public static string FormattedElem(Language toLanguage, string elementIdentifier, int index = 0, params object[] args)
        {
            var elem = Elem(toLanguage, elementIdentifier, index);
            return String.Format(elem, args);
        }

        /// <summary>
        /// Returns the translated text for the specified element identifier 
        /// in the requested language.
        /// </summary>
        /// <param name="toLanguage">
        /// The target language of the translation.
        /// </param>
        /// <param name="elementIdentifier">
        /// The identifier of the element to translate. 
        /// The identifier must correspond to the base (English) key.
        /// </param>
        /// <param name="index">
        /// Optional index of the translation variant. 
        /// If not specified, the first available translation is returned.
        /// </param>
        /// <returns>
        /// The translated string if found in the requested language; 
        /// if not available, falls back to the default language; 
        /// if still not found, returns the original <paramref name="elementIdentifier"/>.
        /// </returns>
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
        /// <param name="text">The text, which is needed to be translated.</param>
        /// <param name="toLanguage">The translation destination language.</param>
        /// <returns>The translated element if it's translation exists, otherwise the language element itself is returned.</returns>
        public static string Translate(Language fromLanguage, string text, Language toLanguage)
        {
            foreach (var keyValuePair in AllLanguageElements.Where(elem => elem.Key.Language == fromLanguage))
            {
                if (keyValuePair.Value.Any(elem => elem == text))
                {
                    if (toLanguage == Language.English)
                    {
                        return keyValuePair.Key.ElementIdentifier;
                    }

                    return GetLanguageElement(keyValuePair.Key.ElementIdentifier, 0, toLanguage);
                }
            }

            return text;
        }

        private static string GetLanguageElement(string elementIdentifier, int index, Language language = Language.English)
        {
            var key = new Translation(language, elementIdentifier);
            return AllLanguageElements != null && AllLanguageElements.TryGetValue(key, out var value) ? value[index] : String.Empty;
        }
    }
}
