using System.Globalization;
using System.Text;

namespace Mtf.LanguageService.MAUI.Converters;

public class TranslationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return String.Empty;
        }

        var key = value is string s ? s : value.ToString();
        if (String.IsNullOrEmpty(key))
        {
            return String.Empty;
        }

        try
        {
            return Lng.Elem(key);
        }
        catch
        {
            return key;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Binding.DoNothing;
        }

        var key = value is string s ? s : value.ToString();
        if (String.IsNullOrEmpty(key))
        {
            return Binding.DoNothing;
        }

        try
        {
            var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (enumType == null || !enumType.IsEnum)
            {
                return Binding.DoNothing;
            }

            string englishText;
            try
            {
                englishText = Lng.Elem(Enums.Language.English, key) ?? key;
            }
            catch
            {
                englishText = key;
            }
            var enumMember = ToPascalCase(englishText);
            if (String.IsNullOrWhiteSpace(enumMember))
            {
                return Binding.DoNothing;
            }

            var parsed = Enum.Parse(enumType, enumMember, ignoreCase: true);
            return parsed;
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    public static string ToPascalCase(string input)
    {
        if (String.IsNullOrWhiteSpace(input))
        {
            return String.Empty;
        }

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length == 1)
            {
                result.Append(Char.ToUpperInvariant(word[0]));
            }
            else
            {
                result.Append(Char.ToUpperInvariant(word[0]));
                result.Append(word.Substring(1).ToLowerInvariant());
            }
        }

        return result.ToString();
    }
}
