using Mtf.LanguageService.MAUI.Test.ViewModels;
using Mtf.LanguageService.MAUI.Views;

namespace Mtf.LanguageService.MAUI.Test.Views;

internal partial class QualificationDetailsPage : NotifierPage
{
    public QualificationDetailsPage()
    {
        InitializeComponent();
        BindingContext = new QualificationDetailsViewModel();
    }
}