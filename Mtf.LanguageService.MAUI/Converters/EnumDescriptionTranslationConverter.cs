using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Mtf.LanguageService.MAUI.Converters;

public class EnumDescriptionTranslationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return String.Empty;
        }

        var type = value.GetType();
        var name = value.ToString() ?? String.Empty;

        if (!type.IsEnum)
        {
            return Lng.Elem(name);
        }

        var isFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

        if (!isFlags)
        {
            return TranslateSingle(type, name);
        }

        var enumValue = (Enum)value;
        var numericValue = System.Convert.ToUInt64(enumValue);

        if (numericValue == 0)
        {
            return TranslateSingle(type, name);
        }

        var values = Enum.GetValues(type).Cast<Enum>();

        var descriptions = values
            .Where(ev =>
            {
                var flagValue = System.Convert.ToUInt64(ev);
                if (flagValue == 0 || !IsPowerOfTwo(flagValue))
                {
                    return false;
                }

                return (numericValue & flagValue) == flagValue;
            })
            .Select(ev => TranslateSingle(type, ev.ToString() ?? String.Empty));

        return String.Join(", ", descriptions);
    }

    private static bool IsPowerOfTwo(ulong value)
    {
        return (value & (value - 1)) == 0;
    }

    private static string TranslateSingle(Type type, string name)
    {
        var member = type.GetMember(name).FirstOrDefault();
        var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
        var description = descAttr?.Description ?? name;
        return Lng.Elem(description);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}