# `Translator` Class Documentation

The `Translator` class provides functionality to translate MAUI `Page` and `View` objects and their visual elements recursively. It integrates with the `Lng` class to support multilingual applications by updating relevant properties of MAUI controls and elements.

---

## **Namespace**
`Mtf.LanguageService.MAUI`

---

## **Methods**

### **Translate(Page page)**

Translates a MAUI `Page` and all of its descendants, including toolbar items.

- **Parameters:**
  - `page` (`Page`): The MAUI `Page` to be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each object and its original text value before translation.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `page` is `null`.
- **Description:**
  - Recursively traverses all child views of the page and translates their string-based UI properties.
  - Translates toolbar items if present.
  - Returns original values to allow restoration if needed.
- **Remarks:**
  - **WARNING:** This process may break existing data bindings. It modifies string-based UI properties directly, which can override or detach bindings applied to those properties.

---

### **Translate(View view)**

Translates a MAUI `View` and all of its descendants.

- **Parameters:**
  - `view` (`View`): The MAUI `View` to be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each object and its original text value before translation.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `view` is `null`.
- **Description:**
  - Recursively traverses all child views and translates their string-based properties.
  - Returns original values for potential restoration.
- **Remarks:**
  - **WARNING:** This process may break existing data bindings. It modifies string-based UI properties directly, which can override or detach bindings applied to those properties.

---

### **SetOriginalTexts(Dictionary<object, string> originalTexts)**

Restores the original text values for MAUI controls and elements based on a dictionary previously returned by `Translate`.

- **Parameters:**
  - `originalTexts` (`Dictionary<object, string>`): A dictionary where keys are the objects and values are their original text values (typically returned from a prior `Translate` call).
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `originalTexts` is `null`.
- **Description:**
  - Iterates through the dictionary and restores each object's original property values.
  - Handles multiple properties per object and special cases like tooltip text.
  - Useful for reverting translated content back to the original language or text.

---

## **Features**

1. **Translation Integration:**
   - Utilizes the `Lng.Elem` method to fetch translations for strings based on the current language.

2. **Recursive Translation:**
   - Handles nested views by traversing the entire visual tree of the page or view.

3. **Multiple Property Types:**
   - Translates common MAUI properties such as `Text`, `Title`, `Header`, `Placeholder`, `Label`, `Content`, and more.

4. **Original Value Tracking:**
   - The `Translate` methods return a dictionary of original values, enabling language switching and restoration.

5. **Toolbar Support:**
   - The `Translate(Page)` method includes support for translating toolbar items.

6. **Null Handling:**
   - Includes checks to prevent runtime exceptions when encountering null objects or properties.

---

## **Usage Example**

### Basic Page Translation

```csharp
using Microsoft.Maui.Controls;
using Mtf.LanguageService;
using Mtf.LanguageService.MAUI;

namespace MyMauiApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		// Set up your MAUI controls with English text
		var label = new Label { Text = "Hello World" };
		var button = new Button { Text = "Click Me" };
		var entry = new Entry { Placeholder = "Enter your name" };

		var stackPanel = new VerticalStackLayout();
		stackPanel.Children.Add(label);
		stackPanel.Children.Add(entry);
		stackPanel.Children.Add(button);

		Content = stackPanel;

		// Translate the page to the current language
		var originals = Translator.Translate(this);
	}
}
```

### Translating Individual Views

```csharp
var customView = new MyCustomView();
var originals = Translator.Translate(customView);
```

### Changing Language and Restoring Originals

```csharp
// Store the originals when first translating
var originalTexts = Translator.Translate(myPage);

// Later, change to a different language
Lng.DefaultLanguage = Language.Hungarian;
Translator.Translate(myPage);

// To revert back to the original text
Translator.SetOriginalTexts(originalTexts);

// Then translate again with a different language
Lng.DefaultLanguage = Language.English;
Translator.Translate(myPage);
```

### Language Switching Workflow

```csharp
using Mtf.LanguageService;

public partial class MyPage : ContentPage
{
	private Dictionary<object, string> _originalTexts;

	public MyPage()
	{
		InitializeComponent();

		// Translate and store originals
		_originalTexts = Translator.Translate(this);
	}

	private async Task SwitchLanguage(Language newLanguage)
	{
		// Restore originals first
		Translator.SetOriginalTexts(_originalTexts);

		// Change the default language
		Lng.DefaultLanguage = newLanguage;

		// Re-translate with the new language
		_originalTexts = Translator.Translate(this);

		// Refresh UI if needed
		await Task.Delay(100);
	}
}
```

### Translate Between Two Languages Directly

```csharp
using Mtf.LanguageService;

// Translate a specific text from Hungarian to English
string english = Lng.Translate(Language.Hungarian, "Ismétlődés", Language.English);
```

---

## **Supported MAUI Properties**

The `Translator` class recognizes and translates the following common MAUI properties:

- `Text`
- `Title`
- `Header`
- `Placeholder`
- `Label`
- `Content`
- `Caption`
- `Description`
- `HeaderText`
- `LabelText`
- `ButtonText`
- `TitleText`

---

## **Best Practices**

1. **Store Original Values:**
   - Always capture the return value from `Translate(Page)` or `Translate(View)` to enable language switching.

2. **Aware of Data Binding:**
   - Be cautious when using data bindings on translated properties, as direct property modification may override bindings.

3. **Translate at Initialization:**
   - Call `Translate` after all UI elements are created and configured.

4. **Use SetOriginalTexts for Language Switching:**
   - When implementing language switching, always restore originals before translating to a new language to avoid compounding translations.

5. **Toolbar Support:**
   - If your page includes a toolbar, the `Translate(Page)` method will automatically handle toolbar item translations. Use `Translate(View)` only for view hierarchies without toolbars.

---

## **Notes**

1. **Extendability:**
   - The translator can be extended to handle custom properties by implementing custom translation logic in your MAUI controls.

2. **Compatibility:**
   - Designed for MAUI applications using `Microsoft.Maui.Controls`.

3. **Performance:**
   - Efficient traversal of the visual tree ensures minimal overhead, even for complex page layouts.

4. **Exception Handling:**
   - Always wrap `Translate` calls in try-catch blocks if you're handling potentially null or malformed page structures.

```
