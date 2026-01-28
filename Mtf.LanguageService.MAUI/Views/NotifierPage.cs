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
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WeakReferenceMessenger.Default.Register<ShowPage>(this, (sender, message) =>
        {
            Dispatcher.Dispatch(() => ShowPageHandler(message));
        });
        WeakReferenceMessenger.Default.Register<ShowInfoMessage>(this, (sender, message) =>
        {
            Dispatcher.Dispatch(() => DisplayAlert(message));
        });
        WeakReferenceMessenger.Default.Register<ShowErrorMessage>(this, (sender, message) =>
        {
            Dispatcher.Dispatch(() => DisplayError(message));
        });

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
            return DisplayAlertAsync(message.Title, message.Message, Lng.Elem("OK"));
        });
    }

    private Task<ConfiguredTaskAwaitable> DisplayError(ShowErrorMessage message)
    {
        return Dispatcher.DispatchAsync(() =>
        {
            var finalException = message.Exception;

            if (finalException != null && message.ShowInnerException)
            {
                while (finalException.InnerException != null)
                {
                    finalException = finalException.InnerException;
                }
            }

            var title = Lng.Elem("Error");
            var displayMessage = Lng.Elem(finalException?.Message ?? message.Value);

            if (finalException != null)
            {
#if DEBUG
            var frame = new StackTrace(finalException, true).GetFrame(0);
            string callingMethod = frame?.GetMethod()?.Name ?? "N/A";
            string callingClass = frame?.GetMethod()?.DeclaringType?.FullName ?? "N/A";
            int lineNumber = frame?.GetFileLineNumber() ?? 0;

            title = $"{Lng.Elem("Error")} (DEBUG): {callingMethod}";
            
            displayMessage = $"{displayMessage}\n\n" +
                             $"----------------------------\n" +
                             $"Source Class: {callingClass}\n" +
                             $"Method: {callingMethod}\n" +
                             $"Line: {lineNumber}\n\n" +
                             $"Full Stack Trace:\n{finalException.StackTrace}";

            try
            {
                Clipboard.SetTextAsync(displayMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch { }
#endif
            }

            return DisplayAlertAsync(title, displayMessage, Lng.Elem("OK")).ConfigureAwait(false);
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<ShowPage>(this);
        WeakReferenceMessenger.Default.Unregister<ShowInfoMessage>(this);
        WeakReferenceMessenger.Default.Unregister<ShowErrorMessage>(this);
    }
}
