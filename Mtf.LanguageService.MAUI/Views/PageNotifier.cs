using CommunityToolkit.Mvvm.Messaging;
using Mtf.LanguageService.Core;
using Mtf.Maui.Controls.Messages;
using System.Diagnostics;

namespace Mtf.LanguageService.MAUI.Views;

public sealed class PageNotifier(Page page, bool autoTranslate)
{
    private readonly Page page = page;
    private readonly bool autoTranslate = autoTranslate;
    private bool registered;

    public void Register()
    {
        if (registered)
        {
            return;
        }

        WeakReferenceMessenger.Default.Register<ShowPage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = page.Navigation.PushAsync(message.Page)));

        WeakReferenceMessenger.Default.Register<ShowInfoMessage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = page.DisplayAlertAsync(message.Title, message.Message, Lng.Elem("OK"))));

        WeakReferenceMessenger.Default.Register<ShowErrorMessage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = DisplayError(message)));

        registered = true;

        TryTranslate();
    }

    public void Unregister()
    {
        if (!registered)
        {
            return;
        }

        WeakReferenceMessenger.Default.UnregisterAll(page);
        registered = false;
    }

    private void TryTranslate()
    {
        if (!autoTranslate)
        {
            return;
        }

        try
        {
            _ = Translator.Translate(page);
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new ShowErrorMessage(ex));
        }
    }

    private Task DisplayError(ShowErrorMessage message)
    {
        var finalException = message.Exception;

        if (finalException != null && message.ShowInnerException)
        {
            while (finalException.InnerException != null)
            {
                finalException = finalException.InnerException;
            }
        }

        var source = "Unknown";

        if (finalException?.StackTrace != null)
        {
            var frame = new StackTrace(finalException, true).GetFrame(0);
            var method = frame?.GetMethod();

            var className = method?.DeclaringType?.Name ?? "N/A";
            var methodName = method?.Name ?? "N/A";
            var line = frame?.GetFileLineNumber();

            source = line > 0
                ? $"{className}.{methodName}:{line}"
                : $"{className}.{methodName}";
        }

        var title = $"{Lng.Elem("Error")} - {source}";

        var displayMessage = Lng.Elem(finalException?.Message ?? message.Value ?? String.Empty);

#if DEBUG
        if (finalException != null)
        {
            displayMessage =
                $"{displayMessage}\n\n----------------------------\n" +
                $"Full Stack Trace:\n{finalException.StackTrace}";
        }
#endif

        return page.DisplayAlertAsync(title, displayMessage, Lng.Elem("OK"));
    }
}