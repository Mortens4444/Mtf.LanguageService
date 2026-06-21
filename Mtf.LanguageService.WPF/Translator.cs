using Mtf.LanguageService.Core;
using System;
using System.Windows;
using System.Windows.Media;

namespace Mtf.LanguageService.WPF;

public static class Translator
{
    public static void Translate(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.Title = Lng.Elem(window.Title);
        TranslateVisual(window);
    }

    private static void TranslateVisual(Visual myVisual)
    {
        if (myVisual == null)
        {
            return;
        }

        var logicalChildren = LogicalTreeHelper.GetChildren(myVisual);
        foreach (var logicalChild in logicalChildren)
        {
            TranslateVisual(logicalChild as Visual);
            TranslateObjectWithProperty(logicalChild, "Header");
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
        {
            var childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
            TranslateObjectWithProperty(childVisual, "Text");
            TranslateObjectWithProperty(childVisual, "Content");
            TranslateVisual(childVisual);
        }
    }

    private static void TranslateObjectWithProperty(object childVisual, string propertyName)
    {
        var type = childVisual.GetType();
        var property = type.GetProperty(propertyName);
        if (property != null)
        {
            if (property.GetValue(childVisual) is string str)
            {
                property.SetValue(childVisual, Lng.Elem(str));
            }
        }
    }
}
