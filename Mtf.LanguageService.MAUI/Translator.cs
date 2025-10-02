﻿using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        private static readonly ConditionalWeakTable<object, string> PropertyMap = new ConditionalWeakTable<object, string>();

        private static readonly string[] CommonProperties = new[] { "Text", "Title", "Header", "Placeholder", "Label", "Content" };

        /// <summary>
        /// Lefordítja a Page-et és alárendeltjeit. Visszatér egy Dictionary-dal,
        /// amely tartalmazza az objektumokat és az eredeti szövegüket.
        /// </summary>
        /// <param name="page">A lefordítandó Page.</param>
        /// <returns>Dictionary, ahol a kulcs az objektum, az érték az eredeti szöveg.</returns>
        public static Dictionary<object, string> Translate(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            var originals = new Dictionary<object, string>();
            TranslateElement(page, originals);
            // ToolbarItems külön: Page.ToolbarItems gyakori hely a szövegeknek (ToolBarItem.Text)
            TryTranslateToolbarItems(page, originals);
            return originals;
        }

        /// <summary>
        /// Visszaállítja az eredeti szövegeket a korábban Translate által visszaadott dictionary alapján.
        /// </summary>
        public static void SetOriginalTexts(Dictionary<object, string> originalTexts)
        {
            if (originalTexts == null) throw new ArgumentNullException(nameof(originalTexts));

            foreach (var kv in originalTexts)
            {
                var target = kv.Key;
                var originalValue = kv.Value;

                if (target == null) continue;

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

        private static void TranslateElement(object element, Dictionary<object, string> originals)
        {
            if (element == null) return;

            foreach (var prop in CommonProperties)
            {
                if (TryTranslateProperty(element, prop, originals))
                {
                    return;
                }
            }

            if (element is IContentView contentView)
            {
                TranslateElement(contentView.Content, originals);
                return;
            }

            if (element is Layout layout)
            {
                foreach (var child in layout.Children)
                    TranslateElement(child, originals);
                return;
            }

            if (element is ItemsView itemsView)
            {
                TryTranslateProperty(itemsView, "Header", originals);
                TryTranslateProperty(itemsView, "Footer", originals);

                if (itemsView.ItemsSource is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                        TranslateElement(item, originals);
                }
                return;
            }

            if (element is IEnumerable enumerableElement && !(element is string))
            {
                foreach (var item in enumerableElement)
                    TranslateElement(item, originals);
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
                if (String.IsNullOrEmpty(val)) return false;

                originals.TryAdd(target, val);

                try
                {
                    PropertyMap.Remove(target);
                }
                catch { }
                PropertyMap.Add(target, propertyName);

                var translated = Lng.Elem(val);
                prop.SetValue(target, translated);
                return true;
            }
            catch
            {
                return false;
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
            if (target == null) return;

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
