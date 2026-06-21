# `Translator` Class Documentation  

The `Translator` class provides functionality to translate Windows Forms controls, forms, and their children recursively. It integrates with the `Lng` class to provide multilingual support by updating the `Text` property of controls and related UI elements.

---

## **Namespace**  
`Mtf.LanguageService.Windows.Forms`

## **Methods**

### **Translate(Form form)**  
Translates a `Form` and all its child controls.

- **Parameters:**  
  - `form` (`Form`): The `Form` to be translated.
- **Exceptions:**  
  - `ArgumentNullException`: Thrown if `form` is `null`.  

---

### **Translate(UserControl userControl)**  
Translates a `UserControl` and all its child controls.

- **Parameters:**  
  - `userControl` (`UserControl`): The `UserControl` to be translated.
- **Exceptions:**  
  - `ArgumentNullException`: Thrown if `userControl` is `null`.  

---

### **Translate(Control.ControlCollection controls)**  
Recursively translates a collection of controls.

- **Parameters:**  
  - `controls` (`Control.ControlCollection`): The collection of controls to be translated.
- **Exceptions:**  
  - `ArgumentNullException`: Thrown if `controls` is `null`.  

---

### **Translate(DataGridView dataGridView)**  
Translates the headers of a `DataGridView`.

- **Parameters:**  
  - `dataGridView` (`DataGridView`): The `DataGridView` to be translated.  

---

### **Translate(TreeNode node)**  
Recursively translates a `TreeNode` and its child nodes.

- **Parameters:**  
  - `node` (`TreeNode`): The `TreeNode` to be translated.  

---

### **Translate(ComboBox comboBox, ComboBox.ObjectCollection items)**  
Translates the items in a `ComboBox`.

- **Parameters:**  
  - `comboBox` (`ComboBox`): The `ComboBox` whose items will be translated.  
  - `items` (`ComboBox.ObjectCollection`): The collection of items to be translated.  

---

### **Translate(ToolStripItemCollection toolStripItems)**  
Translates a collection of `ToolStripItem` objects.

- **Parameters:**  
  - `toolStripItems` (`ToolStripItemCollection`): The collection of tool strip items to be translated.
- **Exceptions:**  
  - `ArgumentNullException`: Thrown if `toolStripItems` is `null`.  

---

### **Translate(Menu.MenuItemCollection items)** (Only for .NET Framework 4.8.1 and below)  
Translates a collection of `MenuItem` objects.

- **Parameters:**  
  - `items` (`Menu.MenuItemCollection`): The collection of menu items to be translated.  

---

## **Features**  

1. **Control Types Supported:**  
   - `Label`, `Button`, `TextBox`, `ListView`, `TreeView`, `MenuStrip`, `StatusStrip`, `ComboBox`, `ContextMenuStrip`, `DataGridView`, and more.
   
2. **Recursive Translation:**  
   - Child elements of controls like `TreeView.Nodes` or `MenuStrip.Items` are translated recursively.

3. **Integration with `Lng` Class:**  
   - Uses `Lng.Elem` for translating strings based on the application's current language.

4. **Framework Compatibility:**  
   - Contains conditional compilation directives for compatibility with .NET Framework 4.8.1 (`#if NET481`).  

---

## **Usage Example**  

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

class Program
{
    static void Main()
    {
        // Example Form
        var form = new Form
        {
            Text = "Main Form"
        };

        // Example Controls
        var button = new Button
        {
            Text = "Click Me",
            Location = new System.Drawing.Point(10, 10)
        };

        var label = new Label
        {
            Text = "Hello World",
            Location = new System.Drawing.Point(10, 50)
        };

        form.Controls.Add(button);
        form.Controls.Add(label);

        // Translate the Form and all its controls
        Translator.Translate(form);

        Application.Run(form);
    }
}
```


```
using Mtf.LanguageService;
using Mtf.LanguageService.Windows.Forms;

public partial class MainForm : Form
{
	public MainForm()
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

1. **Error Handling:**  
   - Ensures null checks for all inputs to avoid runtime errors.  

2. **Non-Translatable Elements:**  
   - Elements like `WebBrowser` are explicitly skipped during translation.  

3. **Customization:**  
   - Extend or modify translation behavior by overriding the `Lng.Elem` method.  

4. **Conditional Compilation:**  
   - `MenuItemCollection` translation is only available for .NET Framework.  
