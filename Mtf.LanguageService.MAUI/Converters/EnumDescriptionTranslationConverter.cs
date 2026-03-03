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

        if (isFlags)
        {
            var enumValues = Enum.GetValues(type).Cast<Enum>()
                .Where(ev => ev.HasFlag((Enum)value))
                .ToList();

            var descriptions = enumValues.Select(ev =>
            {
                var member = type.GetMember(ev.ToString() ?? string.Empty).FirstOrDefault();
                var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
                return Lng.Elem(descAttr?.Description ?? ev.ToString() ?? string.Empty);
            });

            return String.Join(", ", descriptions);
        }

        var member = type.GetMember(name).FirstOrDefault();
        var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
        var description = descAttr?.Description ?? name;
        return Lng.Elem(description);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}
