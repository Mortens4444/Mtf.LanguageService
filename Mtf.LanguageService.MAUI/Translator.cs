using System.Collections;
using System.Reflection;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        public static void Translate(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);
            TranslateElement(page);
        }

        private static void TranslateElement(object element)
        {
            if (element == null)
            {
                return;
            }

            var commonProperties = new[] { "Text", "Title", "Header", "Placeholder", "Label", "Content" };
            foreach (var commonProperty in commonProperties)
            {
                TryTranslateProperty(element, commonProperty);
            }

            if (element is IContentView container)
            {
                TranslateElement(container.Content);
            }
            else if (element is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    TranslateElement(child);
                }
            }
            else if (element is ItemsView itemsView)
            {
                TryTranslateProperty(itemsView, "Header");
                TryTranslateProperty(itemsView, "Footer");
            }
            else if (element is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    TranslateElement(item);
                }
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
            if (prop == null || prop.PropertyType != typeof(string) || !prop.CanRead || !prop.CanWrite)
            {
                return;
            }

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