using CommunityToolkit.Mvvm.Messaging;
using Mtf.Maui.Controls.Messages;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mtf.LanguageService.MAUI.Views;

public partial class NotifierPage : ContentPage
{
    public bool AutoTranslate { get; init; }

    public NotifierPage(bool autoTranslate = true)
    {
        AutoTranslate = autoTranslate;
        WeakReferenceMessenger.Default.Register<ShowPage>(this, (sender, message) =>
        {
            _ = ShowPageHandler(message);
        });
        WeakReferenceMessenger.Default.Register<ShowInfoMessage>(this, (sender, message) =>
        {
            _ = DisplayAlert(message);
        });
        WeakReferenceMessenger.Default.Register<ShowErrorMessage>(this, (sender, message) =>
        {
            _ = DisplayError(message);
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (AutoTranslate)
            {
                _ = Translator.Translate(this);
            }
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new ShowErrorMessage(ex));
        }
    }

    private Task ShowPageHandler(ShowPage message)
    {
        return Navigation.PushAsync(message.Page);
    }

    private Task DisplayAlert(ShowInfoMessage message)
    {
        return Dispatcher.DispatchAsync(() =>
        {
            return DisplayAlert(message.Title, message.Message, "OK");
        });
    }

    private Task<ConfiguredTaskAwaitable> DisplayError(ShowErrorMessage message)
    {
        return Dispatcher.DispatchAsync(() =>
        {
            var exceptionDetails = message.Value;
            if (message.Exception != null)
            {
                var stackTrace = new StackTrace(message.Exception, true);
                var exception = message.Exception;
                var frame = new StackTrace(exception, true).GetFrame(0);

                string callingMethod = frame?.GetMethod()?.Name ?? "N/A";
                string callingClass = frame?.GetMethod()?.DeclaringType?.FullName ?? "N/A";
                int lineNumber = frame?.GetFileLineNumber() ?? 0;

                string title = "ERROR: " + callingMethod;
                string fullMessage = $"{exceptionDetails}\n\n" +
                                     $"Source Class: {callingClass}\n" +
                                     $"Method: {callingMethod}\n" +
                                     $"Line: {lineNumber}\n\n" +
                                     $"Full Stack Trace:\n{exception.StackTrace}";
                try
                {
                    Clipboard.SetTextAsync(fullMessage).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch { }
                return DisplayAlert(title, fullMessage, "OK").ConfigureAwait(false);
            }

            return DisplayAlert("Error", exceptionDetails, "OK").ConfigureAwait(false);
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
