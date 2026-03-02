namespace Mtf.LanguageService.MAUI.Views;

public partial class NotifierTabbedPage : TabbedPage
{
    private readonly PageNotifier notifier;

    public NotifierTabbedPage(bool autoTranslate = true)
    {
        notifier = new PageNotifier(this, autoTranslate);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        notifier.Register();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        notifier.Unregister();
    }
}
