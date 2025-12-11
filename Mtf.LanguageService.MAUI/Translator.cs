using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        private static readonly ConditionalWeakTable<object, string> PropertyMap = new();
        private static readonly string[] CommonProperties = new[] { "Text", "Title", "Header", "Placeholder", "Label", "Content" };

        /// <summary>
        /// Translates the given Page and all of its descendants.  
        /// Returns a dictionary that contains each object and its original text value.
        /// </summary>
        /// <param name="page">The Page to translate.</param>
        /// <returns>
        /// A dictionary where the key is the object and the value is its original text.
        /// </returns>
        /// <remarks>
        /// WARNING: This process may break existing data bindings.  
        /// It modifies string-based UI properties directly, which can override or detach bindings
        /// applied to those properties.
        /// </remarks>
        public static Dictionary<object, string> Translate(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            var originals = new Dictionary<object, string>();
            TranslateElement(page, originals);
            TryTranslateToolbarItems(page, originals);
            return originals;
        }

        /// <summary>
        /// Restores the original text values based on the dictionary previously returned by Translate.
        /// </summary>
        public static void SetOriginalTexts(Dictionary<object, string> originalTexts)
        {
            ArgumentNullException.ThrowIfNull(originalTexts);

            foreach (var kv in originalTexts)
            {
                var target = kv.Key;
                var originalValue = kv.Value;

                if (target == null)
                {
                    continue;
                }

                if (PropertyMap.TryGetValue(target, out var propName))
                {
                    TrySetProperty(target, propName, originalValue);
                    continue;
                }

                foreach (var p in CommonProperties)
                {
                    if (TrySetProperty(target, p, originalValue))
                    {
                        break;
                    }
                }

                TryRestoreSpecialCases(target, originalValue);
            }
        }

        #region Implementation

        private static void TranslateElement(object? element, Dictionary<object, string> originals)
        {
            if (element == null)
            {
                return;
            }

            foreach (var prop in CommonProperties)
            {
                _ = TryTranslateProperty(element, prop, originals);
            }

            if (element is IContentView contentView)
            {
                TranslateElement(contentView.Content, originals);
                return;
            }

            if (element is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    TranslateElement(child, originals);
                }

                return;
            }

            if (element is ItemsView itemsView)
            {
                _ = TryTranslateProperty(itemsView, "Header", originals);
                _ = TryTranslateProperty(itemsView, "Footer", originals);

                if (itemsView.ItemsSource is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        TranslateElement(item, originals);
                    }
                }
                return;
            }

            if (element is IEnumerable enumerableElement && element is not string)
            {
                foreach (var item in enumerableElement)
                {
                    TranslateElement(item, originals);
                }

                return;
            }

            if (element is Page page)
            {
                TryTranslateToolbarItems(page, originals);
            }
        }

        private static bool TryTranslateProperty(object target, string propertyName, Dictionary<object, string> originals)
        {
            if (target == null)
            {
                return false;
            }

            try
            {
                var type = target.GetType();
                var prop = type.GetRuntimeProperty(propertyName);
                if (prop == null)
                {
                    return false;
                }

                if (prop.PropertyType != typeof(string) || !prop.CanRead || !prop.CanWrite)
                {
                    return false;
                }

                var val = prop.GetValue(target) as string;
                if (String.IsNullOrEmpty(val))
                {
                    return false;
                }

                Translate(target, propertyName, originals, prop, val);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void Translate(object target, string propertyName, Dictionary<object, string> originals, PropertyInfo prop, string originalText)
        {
            var translated = Lng.Elem(originalText);
            if (translated != originalText)
            {
                originals.TryAdd(target, originalText);

                try
                {
                    PropertyMap.Remove(target);
                }
                catch { }
                PropertyMap.Add(target, propertyName);

                prop.SetValue(target, translated);
            }
        }

        private static void TryTranslateToolbarItems(Page page, Dictionary<object, string> originals)
        {
            if (page?.ToolbarItems == null)
            {
                return;
            }

            foreach (var ti in page.ToolbarItems)
            {
                try
                {
                    if (!String.IsNullOrEmpty(ti.Text))
                    {
                        if (!originals.ContainsKey(ti))
                        {
                            originals.Add(ti, ti.Text);
                        }

                        PropertyMap.Remove(ti);
                        PropertyMap.Add(ti, "Text");
                        ti.Text = Lng.Elem(ti.Text);
                    }
                }
                catch { }
            }
        }

        private static bool TrySetProperty(object target, string propertyName, string originalValue)
        {
            if (target == null)
            {
                return false;
            }

            try
            {
                var type = target.GetType();
                var prop = type.GetRuntimeProperty(propertyName);
                if (prop == null || !prop.CanWrite)
                {
                    return false;
                }

                if (prop.PropertyType != typeof(string))
                {
                    return false;
                }

                prop.SetValue(target, originalValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void TryRestoreSpecialCases(object target, string originalValue)
        {
            if (target == null)
            {
                return;
            }

            if (target is ToolbarItem ti)
            {
                try { ti.Text = originalValue; } catch { }
                return;
            }

            var tName = target.GetType().Name;
            if (tName.Contains("Menu") || tName.Contains("Flyout"))
            {
                TrySetProperty(target, "Text", originalValue);
                TrySetProperty(target, "Title", originalValue);
                TrySetProperty(target, "Label", originalValue);
            }
        }

        #endregion
    }
}
