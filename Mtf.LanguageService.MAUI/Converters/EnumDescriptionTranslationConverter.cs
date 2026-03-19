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
            return string.Empty;
        }

        var type = value.GetType();
        var name = value.ToString() ?? string.Empty;

        if (!type.IsEnum)
        {
            return Lng.Elem(name);
        }

        var isFlags = type.GetCustomAttribute<FlagsAttribute>() != null;
        if (!isFlags)
        {
            return TranslateSingle(type, name);
        }

        // A value reinterpretálása unsigned longként (nem dob OverFlowException-t, még ha
        // a signed underlying value negatív is — csak bit-reinterpretáció történik).
        var enumValue = (Enum)value;
        var numericValue = UncheckedToUInt64(enumValue);

        // Ha 0, akkor a "None/Unknown" nevét fordítjuk rendesen
        if (numericValue == 0)
        {
            return TranslateSingle(type, name);
        }

        var exactMatch = Enum.GetValues(type)
            .Cast<Enum>()
            .FirstOrDefault(ev => System.Convert.ToUInt64(ev) == numericValue);

        if (exactMatch != null)
        {
            return TranslateSingle(type, exactMatch.ToString() ?? String.Empty);
        }

        var values = Enum.GetValues(type).Cast<Enum>();

        var descriptions = values
            .Where(ev =>
            {
                var flagValue = UncheckedToUInt64(ev);

                // kihagyjuk a 0-t és a nem "atomos" (összetett) értékeket
                if (flagValue == 0 || !IsPowerOfTwo(flagValue)) return false;

                // benne van-e ez az atom-flag a numerikus értékben?
                return (numericValue & flagValue) == flagValue;
            })
            .Select(ev => TranslateSingle(type, ev.ToString() ?? String.Empty));

        return String.Join(", ", descriptions);
    }

    private static ulong UncheckedToUInt64(Enum ev)
    {
        // Olvassuk ki Int64-ként, majd reinterpretáljuk ulong-ra (nem dob OverflowException-t)
        var signed = System.Convert.ToInt64(ev);
        return unchecked((ulong)signed);
    }

    private static bool IsPowerOfTwo(ulong value) => (value & (value - 1)) == 0;

    private static string TranslateSingle(Type type, string name)
    {
        var member = type.GetMember(name).FirstOrDefault();
        var descAttr = member?.GetCustomAttribute<DescriptionAttribute>();
        var description = descAttr?.Description ?? name;
        return Lng.Elem(description);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}