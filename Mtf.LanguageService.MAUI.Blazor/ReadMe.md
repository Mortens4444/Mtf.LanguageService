# `Translator` Class Documentation

The `Translator` class provides translation support for Blazor components derived from `ComponentBase`. It integrates with the `Lng` class to translate string properties of a component dynamically at runtime and optionally restore their original values later.

---

## Namespace

`Mtf.LanguageService.MAUI.Blazor`

## Methods

### `Translate(ComponentBase component)`

Translates all writable string properties of a Blazor component.

* **Parameters:**

  * `component` (`ComponentBase`): The component whose properties should be translated.

* **Returns:**

  * `Dictionary<object, string>`: A collection containing the original values before translation.

* **Exceptions:**

  * `ArgumentNullException`: Thrown when `component` is `null`.

* **Description:**

  * Uses reflection to inspect all instance properties of the component.
  * Searches for writable string properties.
  * Translates property values through `Lng.Elem`.
  * Stores original values so they can be restored later with `SetOriginalTexts`.

---

### `SetOriginalTexts(Dictionary<object, string> originalTexts)`

Restores original values previously captured by `Translate`.

* **Parameters:**

  * `originalTexts` (`Dictionary<object, string>`): Dictionary returned by a previous call to `Translate`.

* **Exceptions:**

  * `ArgumentNullException`: Thrown when `originalTexts` is `null`.

* **Description:**

  * Restores translated property values to their original state.
  * Uses the internally stored property mapping to determine which property should be restored.
  * Supports fallback restoration for common text-related properties.

---

## Features

### Reflection-Based Translation

The translator automatically discovers string properties without requiring explicit configuration.

Supported property examples:

* `Text`
* `Title`
* `Header`
* `Placeholder`
* `Label`
* `Content`
* `Caption`
* `Description`
* `HeaderText`
* `LabelText`
* `ButtonText`
* `TitleText`

---

### Translation Integration

All translations are performed through:

```csharp
Lng.Elem(text);
```

This allows the translator to work with any language provider supported by the LanguageService library.

---

### Original Value Tracking

The translator stores original values before replacing them.

This enables:

```csharp
var originals = Translator.Translate(this);

// ...

Translator.SetOriginalTexts(originals);
```

---

### Safe Execution

The implementation includes:

* Null checking
* Reflection error handling
* Property validation
* Type checking

This prevents translation failures from breaking component rendering.

---

## Usage Example

### Basic Translation

```csharp
using Microsoft.AspNetCore.Components;
using Mtf.LanguageService.MAUI.Blazor;

public partial class EmployeeList : ComponentBase
{
    private string Title { get; set; } = "Employees";
    private string DeleteMessage { get; set; } = "Are you sure you want to delete this employee?";

    protected override void OnInitialized()
    {
        Translator.Translate(this);
    }
}
```

---

### Restoring Original Values

```csharp
private Dictionary<object, string> originals;

protected override void OnInitialized()
{
    originals = Translator.Translate(this);
}

private void Restore()
{
    Translator.SetOriginalTexts(originals);
}
```

---

### Switching Language

```csharp
Lng.DefaultLanguage = Language.Hungarian;

Translator.Translate(this);
```

---

### Direct Translation

```csharp
var translated = Lng.Translate(
    Language.Hungarian,
    "Alkalmazottak",
    Language.English);
```

---

## Notes

### Component Limitations

Blazor components do not expose a visual tree in the same way as MAUI or WPF controls.

Because of this limitation:

* Component fields and properties can be translated.
* Razor markup content cannot be automatically discovered and translated.
* Static HTML content must use explicit translation calls.

Example:

```razor
<h3>@Lng.Elem("Employees")</h3>

<button>
    @Lng.Elem("Add New Employee")
</button>
```

---

### Reflection Scope

The translator processes:

* Public properties
* Protected properties
* Private properties

Only writable string properties are considered.

---

### Performance

Translation occurs only when `Translate` is invoked.

Since reflection is used, it is recommended to perform translation during component initialization rather than during frequent rendering operations.

---

### Extendability

Additional property names can be supported by extending the `CommonProperties` collection.

Custom translation rules can also be implemented by modifying the reflection logic.
