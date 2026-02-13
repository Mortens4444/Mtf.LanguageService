using Mtf.LanguageService.Enums;
using Mtf.LanguageService.MAUI.Ods;
using Mtf.LanguageService.Models;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Mtf.LanguageService.MAUI;

public static class Lng
{
    private const string LanguageFile = "Languages.ods";

    public static Language DefaultLanguage;

    public static readonly Dictionary<Translation, List<string>> AllLanguageElements;

    private static readonly OdsLanguageElementLoader languageElementLoader = new();

    static Lng()
    {
        SetDefaultLanguage();

        try
        {
            var asm = typeof(Lng).Assembly;
            var names = asm.GetManifestResourceNames();

            var resourceName = names
                .FirstOrDefault(n => n.EndsWith(LanguageFile, StringComparison.OrdinalIgnoreCase) || n.Contains("Languages.ods", StringComparison.OrdinalIgnoreCase));

            if (resourceName != null)
            {
                using var stream = asm.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Resource {resourceName} found but stream is null.");
                AllLanguageElements = languageElementLoader.LoadElements(stream);
                return;
            }

            var languageFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, LanguageFile);
            if (languageFiles.Length != 0)
            {
                AllLanguageElements = languageElementLoader.LoadElements(languageFiles.First());
                return;
            }

            throw new InvalidOperationException($"Cannot find {LanguageFile} file as embedded resource or in directory {AppDomain.CurrentDomain.BaseDirectory}.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Lng static init failed: " + ex);
            throw;
        }
    }

    private static void SetDefaultLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var languageName = currentCulture.EnglishName.Split(' ').First();
        try
        {
            DefaultLanguage = Enum.Parse<Language>(languageName);
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
