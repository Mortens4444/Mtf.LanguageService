# `Translator` Class Documentation

The `Translator` class provides functionality to translate Windows Forms controls, forms, and their children recursively. It integrates with the `Lng` class to provide multilingual support by updating the `Text` property and other string-based properties of controls and related UI elements.

---

## **Namespace**
`Mtf.LanguageService.Windows.Forms`

---

## **Methods**

### **Translate(Form form, ToolTip toolTip = null)**

Translates a `Form` and all its child controls.

- **Parameters:**
  - `form` (`Form`): The `Form` to be translated.
  - `toolTip` (`ToolTip`, optional): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each control and its original text value.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `form` is `null`.
- **Description:**
  - Updates the form's `Text` property and recursively translates all child controls.
  - Returns original text values to allow restoration if needed.

---

### **Translate(UserControl userControl, ToolTip toolTip = null)**

Translates a `UserControl` and all its child controls.

- **Parameters:**
  - `userControl` (`UserControl`): The `UserControl` to be translated.
  - `toolTip` (`ToolTip`, optional): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each control and its original text value.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `userControl` is `null`.
- **Description:**
  - Updates the user control's `Text` property and recursively translates all child controls.

---

### **Translate(Control.ControlCollection controls, ToolTip toolTip = null)**

Recursively translates a collection of controls.

- **Parameters:**
  - `controls` (`Control.ControlCollection`): The collection of controls to be translated.
  - `toolTip` (`ToolTip`, optional): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each control and its original text value.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `controls` is `null`.

---

### **Translate(DataGridView dataGridView)**

Translates the column headers of a `DataGridView`.

- **Parameters:**
  - `dataGridView` (`DataGridView`): The `DataGridView` to be translated.
- **Returns:**
  - `Dictionary<object, string>`: A dictionary containing each column and its original header text.
- **Description:**
  - Translates the `HeaderText` of all columns in the grid.

---

### **Translate(TreeNode node, ToolTip toolTip = null)**

Recursively translates a `TreeNode` and its child nodes.

- **Parameters:**
  - `node` (`TreeNode`): The `TreeNode` to be translated.
  - `toolTip` (`ToolTip`, optional): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `List<KeyValuePair<object, string>>`: A list of tree nodes and their original text values.

---

### **Translate(ComboBox comboBox)**

Translates the string items in a `ComboBox`.

- **Parameters:**
  - `comboBox` (`ComboBox`): The `ComboBox` whose items will be translated.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `comboBox` is `null` or if its `Items` collection is `null`.
- **Description:**
  - Translates only string items in the combobox's `Items` collection using `Lng.Elem`.
  - Non-string items (objects) are left unchanged.
  - Preserves the currently selected index after translation.
  - Clears and repopulates the items collection to apply translations.

---

### **Translate(ToolStripItemCollection toolStripItems, ToolTip toolTip = null)**

Translates a collection of `ToolStripItem` objects (e.g., menu items, toolbar buttons).

- **Parameters:**
  - `toolStripItems` (`ToolStripItemCollection`): The collection of tool strip items to be translated.
  - `toolTip` (`ToolTip`, optional): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `List<KeyValuePair<object, string>>`: A list of tool strip items and their original text values.
- **Exceptions:**
  - `ArgumentNullException`: Thrown if `toolStripItems` is `null`.

---

### **Translate(Menu.MenuItemCollection items, ToolTip toolTip)** _(Only for .NET Framework 4.8.1 and above)_

Translates a collection of `MenuItem` objects.

- **Parameters:**
  - `items` (`Menu.MenuItemCollection`): The collection of menu items to be translated.
  - `toolTip` (`ToolTip`): A `ToolTip` component whose tooltips should also be translated.
- **Returns:**
  - `List<KeyValuePair<object, string>>`: A list of menu items and their original text values.
- **Note:** This method is only available when targeting `.NET Framework 4.8.1` or above.

---

## **Features**

1. **Control Types Supported:**
   - `Label`, `Button`, `TextBox`, `ListView`, `TreeView`, `MenuStrip`, `StatusStrip`, `ComboBox`, `ContextMenuStrip`, `DataGridView`, `CheckBox`, `RadioButton`, and more.

2. **Recursive Translation:**
   - Child elements of controls like `TreeView.Nodes`, `MenuStrip.Items`, and nested controls are translated recursively.

3. **ComboBox String Items:**
   - Only translates items that are strings; object items are preserved unchanged.
   - Maintains the selected index across translations.

4. **Tooltip Support:**
   - Optional `ToolTip` parameter allows translation of control tooltips alongside the controls.

5. **Integration with `Lng` Class:**
   - Uses `Lng.Elem` to translate strings based on the application's current language.

6. **Framework Compatibility:**
   - Conditional compilation directives ensure compatibility with different .NET versions.

---

## **Usage Examples**

### Basic Form Translation

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

class Program
{
	static void Main()
	{
		// Create a simple form with controls
		var form = new Form
		{
			Text = "Main Application"
		};

		// Add button
		var button = new Button
		{
			Text = "Click Me",
			Location = new System.Drawing.Point(10, 10)
		};

		// Add label
		var label = new Label
		{
			Text = "Hello World",
			Location = new System.Drawing.Point(10, 50)
		};

		// Add textbox
		var textbox = new TextBox
		{
			Text = "Enter text here",
			Location = new System.Drawing.Point(10, 90)
		};

		form.Controls.Add(button);
		form.Controls.Add(label);
		form.Controls.Add(textbox);

		// Translate the form and all its controls
		var originals = Translator.Translate(form);

		Application.Run(form);
	}
}
```

### ComboBox String Items Translation

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		// Create a ComboBox with string items
		var comboBox = new ComboBox
		{
			Location = new System.Drawing.Point(10, 10),
			DropDownStyle = ComboBoxStyle.DropDownList
		};

		// Add string items to translate
		comboBox.Items.AddRange(new object[]
		{
			"Option One",
			"Option Two",
			"Option Three"
		});

		comboBox.SelectedIndex = 0;
		this.Controls.Add(comboBox);

		// Translate the combobox items
		Translator.Translate(comboBox);
	}
}
```

### ComboBox with Mixed Content (Strings and Objects)

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public class Item
{
	public int Id { get; set; }
	public string Name { get; set; }

	public override string ToString() => Name;
}

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		var comboBox = new ComboBox
		{
			Location = new System.Drawing.Point(10, 10),
			DropDownStyle = ComboBoxStyle.DropDownList
		};

		// Add mixed content: only strings will be translated
		comboBox.Items.AddRange(new object[]
		{
			"Select an option",
			new Item { Id = 1, Name = "John" },
			new Item { Id = 2, Name = "Jane" },
			"Custom Entry"
		});

		comboBox.SelectedIndex = 0;
		this.Controls.Add(comboBox);

		// Translate only the string items
		Translator.Translate(comboBox);
	}
}
```

### Translation with MenuStrip

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		var menuStrip = new MenuStrip();

		var fileMenu = new ToolStripMenuItem { Text = "File" };
		fileMenu.DropDownItems.Add(new ToolStripMenuItem { Text = "Open" });
		fileMenu.DropDownItems.Add(new ToolStripMenuItem { Text = "Save" });
		fileMenu.DropDownItems.Add(new ToolStripSeparator());
		fileMenu.DropDownItems.Add(new ToolStripMenuItem { Text = "Exit" });

		var editMenu = new ToolStripMenuItem { Text = "Edit" };
		editMenu.DropDownItems.Add(new ToolStripMenuItem { Text = "Copy" });
		editMenu.DropDownItems.Add(new ToolStripMenuItem { Text = "Paste" });

		menuStrip.Items.Add(fileMenu);
		menuStrip.Items.Add(editMenu);

		MainMenuStrip = menuStrip;
		Controls.Add(menuStrip);

		// Translate all menu items
		var originals = Translator.Translate(menuStrip.Items);
	}
}
```

### Translation with ToolTips

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		var toolTip = new ToolTip();

		var button = new Button
		{
			Text = "Save",
			Location = new System.Drawing.Point(10, 10)
		};

		toolTip.SetToolTip(button, "Save the current document");

		this.Controls.Add(button);

		// Translate the form with tooltip support
		Translator.Translate(this, toolTip);
	}
}
```

### TreeView Translation

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		var treeView = new TreeView();

		// Add nodes
		var rootNode = treeView.Nodes.Add("Projects");
		rootNode.Nodes.Add("Project One");
		rootNode.Nodes.Add("Project Two");

		var filesNode = treeView.Nodes.Add("Files");
		filesNode.Nodes.Add("Document.txt");
		filesNode.Nodes.Add("Image.jpg");

		this.Controls.Add(treeView);

		// Translate all tree nodes
		foreach (TreeNode node in treeView.Nodes)
		{
			var originals = Translator.Translate(node);
		}
	}
}
```

### DataGridView Column Headers Translation

```csharp
using System.Windows.Forms;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	public MyForm()
	{
		InitializeComponent();

		var dataGridView = new DataGridView();
		dataGridView.Columns.Add("Name", "Full Name");
		dataGridView.Columns.Add("Email", "Email Address");
		dataGridView.Columns.Add("Phone", "Phone Number");

		this.Controls.Add(dataGridView);

		// Translate column headers
		var originals = Translator.Translate(dataGridView);
	}
}
```

### Language Switching

```csharp
using System.Windows.Forms;
using Mtf.LanguageService;
using Mtf.LanguageService.Windows.Forms;

public partial class MyForm : Form
{
	private Dictionary<object, string> _originalTexts;

	public MyForm()
	{
		InitializeComponent();

		// Translate and store originals
		_originalTexts = Translator.Translate(this);
	}

	private void SwitchToHungarian()
	{
		Lng.DefaultLanguage = Language.Hungarian;
		Translator.Translate(this);
	}

	private void SwitchToEnglish()
	{
		Lng.DefaultLanguage = Language.English;
		Translator.Translate(this);
	}

	private void RevertToOriginal()
	{
		// No direct restore method; re-translate with original language
		Lng.DefaultLanguage = Language.English;
		Translator.Translate(this);
	}
}
```

---

## **Best Practices**

1. **Translate After Control Creation:**
   - Call `Translate` after all controls have been added to the form or container.

2. **Store Originals for Language Switching:**
   - Capture the returned dictionary if you plan to switch languages later.

3. **ComboBox Considerations:**
   - Only string items are translated; ensure data-bound items or objects are handled separately if needed.
   - Call `Translate` after populating the ComboBox.

4. **ToolTip Integration:**
   - Pass the `ToolTip` component to ensure tooltips are translated along with controls.

5. **Nested Controls:**
   - No need to manually translate parent and child controls separately; the recursive methods handle the entire hierarchy.

6. **Non-Translatable Elements:**
   - Elements like `WebBrowser` are explicitly skipped during translation to avoid issues.

---

## **Notes**

1. **Error Handling:**
   - All public methods include null checks to prevent runtime errors.

2. **Non-Translatable Elements:**
   - Certain controls like `WebBrowser` are skipped during automatic translation.

3. **Customization:**
   - Extend translation behavior by creating wrapper methods or by modifying the `Lng.Elem` method.

4. **Conditional Compilation:**
   - `MenuItemCollection` translation is only available for `.NET Framework 4.8.1` and above.

5. **ComboBox Behavior:**
   - Non-string items are preserved unchanged to support object binding scenarios.
   - Selected index is maintained after translation to preserve user selection.

```
