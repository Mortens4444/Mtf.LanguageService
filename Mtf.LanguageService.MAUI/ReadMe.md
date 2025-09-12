# `Translator` Class Documentation  

The `Translator` class provides functionality to translate WPF `Window` objects and their visual elements recursively. It integrates with the `Lng` class to support multilingual applications by updating relevant properties of WPF controls and elements.

---

## **Namespace**  
`Mtf.LanguageService.WPF`

## **Methods**

### **Translate(Window window)**  
Translates a `Window` and its visual elements.

- **Parameters:**  
  - `window` (`Window`): The `Window` to be translated.
- **Exceptions:**  
  - `ArgumentNullException`: Thrown if `window` is `null`.  
- **Description:**  
  Updates the `Title` of the `Window` and recursively translates all child visual elements.

---

### **TranslateVisual(Visual myVisual)**  
Recursively translates a visual tree starting from the provided visual element.  

- **Parameters:**  
  - `myVisual` (`Visual`): The starting visual element for translation.  
- **Description:**  
  - Iterates through both logical and visual trees.  
  - Checks for specific properties like `Header`, `Text`, and `Content` to translate their values.

---

### **TranslateObjectWithProperty(object childVisual, string propertyName)**  
Translates a specific property of a given object if it exists and is a `string`.  

- **Parameters:**  
  - `childVisual` (`object`): The object to check for the property.  
  - `propertyName` (`string`): The name of the property to translate.  
- **Description:**  
  Uses reflection to identify and translate properties such as `Header`, `Text`, or `Content`.

---

## **Features**  

1. **Translation Integration:**  
   - Utilizes the `Lng.Elem` method to fetch translations for strings.  

2. **Recursive Translation:**  
   - Handles nested visual elements by traversing both the logical and visual trees.  

3. **Targeted Properties:**  
   - Specifically translates properties like `Header`, `Text`, and `Content`, making it versatile for WPF applications.  

4. **Null Handling:**  
   - Includes checks to prevent runtime exceptions when encountering null objects or properties.  

---

## **Usage Example**  

```csharp
using System.Windows;
using Mtf.LanguageService.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Set Window Title and control contents
        Title = "Main Window";

        var label = new Label
        {
            Content = "Hello World"
        };

        var button = new Button
        {
            Content = "Click Me"
        };

        var stackPanel = new StackPanel();
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(button);

        Content = stackPanel;

        // Translate the Window
        Translator.Translate(this);
    }
}
```

```
using Mtf.LanguageService;
using Mtf.LanguageService.WPF;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		lv.ItemsSource = new[] { Lng.Elem("Users"), Lng.Elem("Groups") };
		rtb.Document.Blocks.Add(new Paragraph(new Run(Lng.Elem("File name"))));

		Translator.Translate(this);
	}
}
```

If you need to translate to Hungarian, just use this code snippet.
```
Lng.DefaultLanguage = Mtf.LanguageService.Language.Hungarian;
Translator.Translate(this);
```

You can translate from any language to any other language like this.
```
Lng.Translate(Language.Hungarian, "Ismétlődés", Language.English);
```

---

## **Notes**  

1. **Logical vs. Visual Tree:**  
   - The method traverses both logical and visual trees to ensure all relevant elements are translated.

2. **Extendability:**  
   - Can be extended to handle custom properties by modifying the `TranslateObjectWithProperty` method.

3. **Compatibility:**  
   - Designed for WPF applications using `System.Windows` and `System.Windows.Media`.  

4. **Performance:**  
   - Efficient traversal of visual and logical trees ensures minimal overhead.

**Links**

[GitHub URL](https://github.com/Mortens4444/LanguageService)
[Developer's URL](http://w3.hdsnet.hu/mortens/)
[Facebook Page](https://www.facebook.com/developersdream)