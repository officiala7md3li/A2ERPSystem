# Localization Guide

This guide provides instructions on how to manage localization in the DomainDrivenERP system.

## Adding a New Language

1. **Add the Language**:
   - Use the `AddLanguage` method in the `LocalizationRepository` to add a new language.

2. **Create a JSON File**:
   - Create a JSON file in the `Resources/Translations` directory with the language code as the filename (e.g., `fr.json` for French).

3. **Add Translations**:
   - Use the `AddOrUpdateResource` method to add translations to the JSON file.

## Example

To add French as a new language:

1. Add the language:
   ```csharp
   var language = new Language
   {
       Code = "fr",
       Name = "French",
       NativeName = "Français",
       IsRTL = false,
       IsDefault = false,
       IsEnabled = true,
       FlagIcon = "🇫🇷",
       DateFormat = "dd/MM/yyyy",
       TimeFormat = "HH:mm",
       NumberFormat = "1,234.56",
       CurrencyFormat = "€1,234.56",
       DisplayOrder = 2
   };
   await localizationRepository.AddLanguage(language);
   ```

2. Create the JSON file `fr.json` in the `Resources/Translations` directory:
   ```json
   {
     "General": {
       "Hello": {
         "value": "Bonjour",
         "module": "General",
         "group": "Greetings"
       },
       "Goodbye": {
         "value": "Au revoir",
         "module": "General",
         "group": "Greetings"
       }
     }
   }
   ```

3. Add translations:
   ```csharp
   var resource = new LanguageResource
   {
       LanguageId = language.Id,
       ResourceKey = "General.Hello",
       ResourceValue = "Bonjour",
       Module = "General",
       Group = "Greetings"
   };
   await localizationRepository.AddOrUpdateResource(resource);
   ```

## Testing Localization

To test localization functionality, create unit tests for the `LocalizationRepository` methods.

### Example Test Case

```csharp
[Test]
public async Task AddLanguage_ShouldCreateLanguageAndJsonFile()
{
    // Arrange
    var language = new Language
    {
        Code = "fr",
        Name = "French",
        NativeName = "Français",
        IsRTL = false,
        IsDefault = false,
        IsEnabled = true,
        FlagIcon = "🇫🇷",
        DateFormat = "dd/MM/yyyy",
        TimeFormat = "HH:mm",
        NumberFormat = "1,234.56",
        CurrencyFormat = "€1,234.56",
        DisplayOrder = 2
    };

    // Act
    await localizationRepository.AddLanguage(language);

    // Assert
    var addedLanguage = await localizationRepository.GetLanguageByCode("fr");
    Assert.IsNotNull(addedLanguage);
    Assert.AreEqual("French", addedLanguage.Name);

    var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Translations", "fr.json");
    Assert.IsTrue(File.Exists(jsonPath));
}
```

Follow these steps to manage localization dynamically and ensure proper testing.