using Microsoft.Extensions.Logging;
using Mtf.LanguageService.MAUI.Converters;
using Mtf.LanguageService.MAUI.Test.Enums;
using System.Globalization;

namespace Mtf.LanguageService.MAUI.Test
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //var x = new EnumDescriptionTranslationConverter();
            //var y = x.Convert(TerrainType.Anywhere, typeof(string), null, CultureInfo.CurrentCulture)?.ToString() ?? String.Empty;
            //global::System.Console.WriteLine(y);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
