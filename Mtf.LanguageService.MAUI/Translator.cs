using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        private static readonly ConditionalWeakTable<object, List<string>> PropertyMap = new();
        private static readonly string[] CommonProperties = new[] { "Text", "Title", "Header", "Placeholder", "Label", "Content", "Caption", "Description", "HeaderText", "LabelText", "ButtonText", "TitleText" };
        private const string ToolTipPropertyTag = "__ToolTipText";

        private const char RecordSep = '\u001E';
        private const char UnitSep = '\u001F';

        private static void AddOriginal(IDictionary<object, string> originals, object target, string propertyName, string originalValue)
        {
            if (originals == null || target == null || propertyName == null) return;
            var entry = propertyName + UnitSep + (originalValue ?? string.Empty);
            if (originals.TryGetValue(target, out var existing))
            {
                originals[target] = existing + RecordSep + entry;
            }
            else
            {
                originals[target] = entry;
            }

        }

        private static IEnumerable<KeyValuePair<string,string>> ParseOriginalEntries(string packed)
        {
            if (string.IsNullOrEmpty(packed)) yield break;
            var entries = packed.Split(RecordSep);
            foreach (var e in entries)
            {
                var parts = e.Split(UnitSep, 2);
                if (parts.Length == 2)
                    yield return new KeyValuePair<string,string>(parts[0], parts[1]);
            }
        }
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
        /// Translates the given View and all of its descendants.
        /// </summary>
        public static Dictionary<object, string> Translate(View view)
        {
            ArgumentNullException.ThrowIfNull(view);

            var originals = new Dictionary<object, string>();
            TranslateElement(view, originals);
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
                var packed = kv.Value;

                if (target == null)
                {
                    continue;
                }

                // If PropertyMap lists specific properties, restore each from packed data if present.
                if (PropertyMap.TryGetValue(target, out var props))
                {
                    var parsed = ParseOriginalEntries(packed).ToDictionary(p => p.Key, p => p.Value);
                    foreach (var propName in props)
                    {
                        if (parsed.TryGetValue(propName, out var origVal))
                        {
                            if (propName == ToolTipPropertyTag && target is BindableObject bindable)
                            {
                                ToolTipProperties.SetText(bindable, origVal);
                            }
                            else
                            {
                                TrySetProperty(target, propName, origVal);
                            }
                        }
                        else
                        {
                            TrySetProperty(target, propName, packed);
                        }
                    }
                    continue;
                }

                var entries = ParseOriginalEntries(packed).ToList();
                if (entries.Count > 0)
                {
                    foreach (var e in entries)
                    {
                        if (e.Key == ToolTipPropertyTag && target is BindableObject bindable)
                        {
                            ToolTipProperties.SetText(bindable, e.Value);
                        }
                        else
                        {
                            TrySetProperty(target, e.Key, e.Value);
                        }
                    }
                    continue;
                }

                // legacy: single original value -> try common properties
                var originalValue = packed;
                var restored = false;
                foreach (var p in CommonProperties)
                {
                    if (TrySetProperty(target, p, originalValue))
                    {
                        restored = true;
                        break;
                    }
                }

                if (!restored)
                {
                    TryRestoreSpecialCases(target, originalValue);
                }
            }
        }

        #region Implementation

        private static void TranslateElement(object? element, Dictionary<object, string> originals)
        {
            if (element == null)
            {
                return;
            }
            
            TryTranslateToolTip(element, originals);

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

            if (element is CollectionView collectionView)
            {
                _ = TryTranslateProperty(collectionView, "Header", originals);
                _ = TryTranslateProperty(collectionView, "Footer", originals);

                if (collectionView.Header is View headerView)
                {
                    TranslateElement(headerView, originals);
                }

                if (collectionView.Footer is View footerView)
                {
                    TranslateElement(footerView, originals);
                }

                if (collectionView.ItemsSource is IEnumerable cvEnumerable)
                {
                    foreach (var item in cvEnumerable)
                    {
                        TranslateElement(item, originals);
                    }
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

            if (element is IVisualTreeElement vte)
            {
                foreach (var child in vte.GetVisualChildren())
                {
                    TranslateElement(child as Element, originals);
                }

                return;
            }
        }

        private static void TryTranslateToolTip(object target, Dictionary<object, string> originals)
        {
            try
            {
                if (target is not BindableObject bindable)
                {
                    return;
                }

                var text = ToolTipProperties.GetText(bindable) as string;
                if (String.IsNullOrEmpty(text))
                {
                    return;
                }

                var translated = Lng.Elem(text);
                if (translated == text)
                {
                    return;
                }

                // Recorded under a distinct tag (not "Text") so SetOriginalTexts can tell a tooltip
                // restore apart from a regular Text-property restore for the same target - they used
                // to share the "Text" tag, which made every ordinary Label/Button Text restore wrongly
                // write to the (invisible) tooltip instead of the visible property.
                AddOriginal(originals, bindable, ToolTipPropertyTag, text);

                try { if (PropertyMap.TryGetValue(bindable, out var list)) { if (!list.Contains(ToolTipPropertyTag)) list.Add(ToolTipPropertyTag); } else { PropertyMap.Add(bindable, new List<string>{ ToolTipPropertyTag }); } } catch { }

                ToolTipProperties.SetText(bindable, translated);
            }
            catch
            {
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

                if (prop.PropertyType == typeof(FormattedString) && prop.CanRead && prop.CanWrite)
                {
                    if (prop.GetValue(target) is FormattedString fs)
                    {
                        var originalCombined = string.Join("\n", fs.Spans.Select(s => s.Text));
                        var translated = false;
                        foreach (var span in fs.Spans)
                        {
                            if (!string.IsNullOrEmpty(span.Text))
                            {
                                var t = Lng.Elem(span.Text);
                                if (t != span.Text)
                                {
                                    AddOriginal(originals, target, propertyName, originalCombined);
                                    span.Text = t;
                                    translated = true;
                                }
                            }
                        }
                        return translated;
                    }
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
                AddOriginal(originals, target, propertyName, originalText);

                try
                {
                    if (PropertyMap.TryGetValue(target, out var existing))
                    {
                        if (!existing.Contains(propertyName)) existing.Add(propertyName);
                    }
                    else
                    {
                        PropertyMap.Add(target, new List<string> { propertyName });
                    }
                }
                catch { }

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
                            AddOriginal(originals, ti, "Text", ti.Text);
                        }

                        try { if (PropertyMap.TryGetValue(ti, out var list)) { if (!list.Contains("Text")) list.Add("Text"); } else { PropertyMap.Add(ti, new List<string>{ "Text" }); } } catch { }
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
