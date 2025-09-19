using Mtf.LanguageService.Enums;

namespace Mtf.LanguageService.MAUI.Test
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Lng.DefaultLanguage = Language.Hungarian;
            Translator.Translate(this);
        }
    }
}
