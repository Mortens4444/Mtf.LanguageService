using System.Reflection;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        public static void Translate(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            if (!string.IsNullOrEmpty(page.Title))
            {
                page.Title = Lng.Elem(page.Title);
            }

            if (page is ContentPage cp)
            {
                TranslateElement(cp.Content);
            }

            if (page is Shell shell)
            {
                foreach (var item in shell.Items)
                {
                    if (!String.IsNullOrEmpty(item.Title))
                    {
                        item.Title = Lng.Elem(item.Title);
                    }
                }
            }
        }

        private static void TranslateElement(object element)
        {
            if (element == null)
            {
                return;
            }

            switch (element)
            {
                case Layout layout:
                    foreach (var child in layout.Children)
                        TranslateElement(child);
                    break;

                case ContentView contentView:
                    TranslateElement(contentView.Content);
                    break;

                case ScrollView scrollView:
                    TranslateElement(scrollView.Content);
                    break;

                case ContentPresenter presenter:
                    TranslateElement(presenter.Content);
                    break;
            }

            TryTranslateProperty(element, "Text");
            TryTranslateProperty(element, "Content");
            TryTranslateProperty(element, "Title");
            TryTranslateProperty(element, "Header");
            TryTranslateProperty(element, "Placeholder");
            TryTranslateProperty(element, "Label");

            if (element is MenuItem menuItem && !String.IsNullOrEmpty(menuItem.Text))
            {
                menuItem.Text = Lng.Elem(menuItem.Text);
            }

            if (element is ToolbarItem toolbarItem && !string.IsNullOrEmpty(toolbarItem.Text))
            {
                toolbarItem.Text = Lng.Elem(toolbarItem.Text);
            }

            if (element is ItemsView itemsView)
            {
                TryTranslateProperty(itemsView, "Header");
                TryTranslateProperty(itemsView, "Footer");
            }
        }

        private static void TryTranslateProperty(object target, string propertyName)
        {
            if (target == null)
            {
                return;
            }

            var type = target.GetType();
            var prop = type.GetRuntimeProperty(propertyName);
            if (prop == null)
            {
                return;
            }

            if (prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite)
            {
                try
                {
                    var val = prop.GetValue(target) as string;
                    if (!String.IsNullOrEmpty(val))
                    {
                        prop.SetValue(target, Lng.Elem(val));
                    }
                }
                catch { }
            }
        }
    }
}
