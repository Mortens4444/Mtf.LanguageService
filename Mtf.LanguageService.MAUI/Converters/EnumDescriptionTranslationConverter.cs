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
            var numericValue = System.Convert.ToUInt64((Enum)value);

            if (numericValue == 0)
            {
                // A 0 értékű enum tagot kezeljük (pl. None)
                var member0 = type.GetMember(name).FirstOrDefault();
                var descAttr0 = member0?.GetCustomAttribute<DescriptionAttribute>();
                return Lng.Elem(descAttr0?.Description ?? name);
            }

            var descriptions = Enum.GetValues(type)
                .Cast<Enum>()
                .Where(ev =>
                {
                    var flagValue = System.Convert.ToUInt64(ev);
                    return flagValue != 0
                        && (flagValue & (flagValue - 1)) == 0   // csak 2-hatvány (atomi) flagek
                        && (numericValue & flagValue) == flagValue;
                })
                .Select(ev =>
                {
                    var member = type.GetMember(ev.ToString() ?? string.Empty).FirstOrDefault();
                    var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
                    return Lng.Elem(descAttr?.Description ?? ev.ToString() ?? string.Empty);
                });

            //var enumValues = Enum.GetValues(type).Cast<Enum>()
            //    .Where(ev => ev.HasFlag((Enum)value))
            //    .ToList();

            //var descriptions = enumValues.Select(ev =>
            //{
            //    var member = type.GetMember(ev.ToString() ?? string.Empty).FirstOrDefault();
            //    var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
            //    return Lng.Elem(descAttr?.Description ?? ev.ToString() ?? string.Empty);
            //});
            return String.Join(", ", descriptions);
        }

        var member = type.GetMember(name).FirstOrDefault();
        var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
        var description = descAttr?.Description ?? name;
        return Lng.Elem(description);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}
