using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Mtf.LanguageService.MAUI.Converters;

internal class EnumDescriptionTranslationConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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

        var member = type.GetMember(name).FirstOrDefault();
        if (member == null)
        {
            return Lng.Elem(name);
        }

        var descAttr = member.GetCustomAttribute<DescriptionAttribute>();
        var description = descAttr?.Description ?? name;

        return Lng.Elem(description);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}
