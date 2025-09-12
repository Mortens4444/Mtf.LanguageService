using System.Reflection;

namespace Mtf.LanguageService.MAUI
{
    public static class Translator
    {
        // fő belépőpont: bármilyen Page-t adhatsz meg (ContentPage, etc.)
        public static void Translate(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            if (!string.IsNullOrEmpty(page.Title))
            {
                page.Title = Lng.Elem(page.Title);
            }

            // Page.Content nincs minden Page-en, ezért ellenőrizzük a ContentPage-t
            if (page is ContentPage cp)
            {
                TranslateElement(cp.Content);
            }

            // Toolbars, Shell esetén külön kezelést adhatunk hozzá ha szükséges
            if (page is Shell shell)
            {
                foreach (var item in shell.Items)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                        item.Title = Lng.Elem(item.Title);
                }
            }
        }

        // általános elem-fordító: BindableObject-et / VisualElem.-et fogad
        private static void TranslateElement(object element)
        {
            if (element == null) return;

            // ha a típus BindableObject és Layout, akkor járjuk be a gyerekeket
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

            // általános reflexiós fordítás: gyakori string tulajdonságok
            TryTranslateProperty(element, "Text");
            TryTranslateProperty(element, "Content"); // ha string
            TryTranslateProperty(element, "Title");
            TryTranslateProperty(element, "Header");
            TryTranslateProperty(element, "Placeholder");
            TryTranslateProperty(element, "Label"); // ritkábban, de előfordulhat

            // speciális: ToolBarItem / MenuItem / Menu esetén a Text/Title mezők
            if (element is MenuItem menuItem && !string.IsNullOrEmpty(menuItem.Text))
                menuItem.Text = Lng.Elem(menuItem.Text);

            if (element is ToolbarItem toolbarItem && !string.IsNullOrEmpty(toolbarItem.Text))
                toolbarItem.Text = Lng.Elem(toolbarItem.Text);

            // néhány konténer további bejárása (CollectionView, ListView esetén item template-ekhez)
            if (element is Microsoft.Maui.Controls.ItemsView itemsView)
            {
                // nem járjuk be az ItemsSource elemeit, csak a header/footer, ha van
                TryTranslateProperty(itemsView, "Header");
                TryTranslateProperty(itemsView, "Footer");
            }
        }

        private static void TryTranslateProperty(object target, string propertyName)
        {
            if (target == null) return;

            var type = target.GetType();
            var prop = type.GetRuntimeProperty(propertyName);
            if (prop == null) return;

            // csak akkor fordítunk, ha a property string típusú és olvasható/írható
            if (prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite)
            {
                try
                {
                    var val = (string)prop.GetValue(target);
                    if (!string.IsNullOrEmpty(val))
                    {
                        prop.SetValue(target, Lng.Elem(val));
                    }
                }
                catch
                {
                    // ne dobjon kivételt UI bejárás közben — lenyelünk minden hibát
                }
            }
        }
    }
}
