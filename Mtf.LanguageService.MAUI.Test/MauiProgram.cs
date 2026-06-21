using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Mtf.LanguageService.Core;

namespace Mtf.LanguageService.MAUI.Test
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //var x = new EnumDescriptionTranslationConverter();
            //var y = x.Convert(TerrainType.Order, typeof(string), null, CultureInfo.CurrentCulture)?.ToString() ?? String.Empty;
            //global::System.Console.WriteLine(y);

            var result = Lng.Elem("Undead");
            if (result != "Élőholtak")
            {
                throw new InvalidDataException("Translation is not correct");
            }

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
