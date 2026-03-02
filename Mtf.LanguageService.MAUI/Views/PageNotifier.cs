using CommunityToolkit.Mvvm.Messaging;
using Mtf.Maui.Controls.Messages;

namespace Mtf.LanguageService.MAUI.Views;

public sealed class PageNotifier(Page page, bool autoTranslate)
{
    private readonly Page page = page;
    private readonly bool autoTranslate = autoTranslate;

    public void Register()
    {
        WeakReferenceMessenger.Default.Register<ShowPage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = page.Navigation.PushAsync(message.Page)));

        WeakReferenceMessenger.Default.Register<ShowInfoMessage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = page.DisplayAlertAsync(message.Title, message.Message, Lng.Elem("OK"))));

        WeakReferenceMessenger.Default.Register<ShowErrorMessage>(page, (_, message) =>
            page.Dispatcher.Dispatch(() => _ = DisplayError(message)));

        TryTranslate();
    }

    public void Unregister()
        => WeakReferenceMessenger.Default.UnregisterAll(page);

    private void TryTranslate()
    {
        if (!autoTranslate)
            return;

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

        var title = Lng.Elem("Error");
        var displayMessage = Lng.Elem(finalException?.Message ?? message.Value ?? String.Empty);

#if DEBUG
        if (finalException != null)
        {
            var frame = new StackTrace(finalException, true).GetFrame(0);

            var callingMethod = frame?.GetMethod()?.Name ?? "N/A";
            var callingClass = frame?.GetMethod()?.DeclaringType?.FullName ?? "N/A";
            var lineNumber = frame?.GetFileLineNumber() ?? 0;

            title = $"{title} (DEBUG): {callingMethod}";

            displayMessage =
                $"{displayMessage}\n\n----------------------------\n" +
                $"Source Class: {callingClass}\n" +
                $"Method: {callingMethod}\n" +
                $"Line: {lineNumber}\n\n" +
                $"Full Stack Trace:\n{finalException.StackTrace}";
        }
#endif

        return page.DisplayAlertAsync(title, displayMessage, Lng.Elem("OK"));
    }
}