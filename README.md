# Locus - A JSON-Based localization library

Locus is a C# library for managing and accessing locale-specific resources. It supports hierarchical localization structures, dynamic locale fallback, and is designed to be flexible and efficient for use in .NET applications.

## Features

- **JSON-based localization**: Store your localized resources in easy-to-edit JSON files.
- **Dynamic locale fallback**: Automatically fall back to the default locale or a parent locale if a specific translation is missing.
- **Deep localization nesting**: Supports deeply nested keys for more complex localization structures, for example, `provider["Greetings"]["Hello"]`.
  
## Usage
For an example app, refer to the Locus.SampleApp project.

### Basic Usage
```csharp
using Locus;

var provider = new LocalizationProvider(new LocaleConfiguration());

Console.WriteLine(provider["Greetings"]["Hello"]);  // Expected output: "Hello!"
Console.WriteLine(provider["Messages"]["Success"]); // Expected output: "Operation successful"

// Set locale to Brazilian Portuguese
provider.SetLocale("pt-BR");

// Test retrieving localized strings from pt-PT
Console.WriteLine(provider["Greetings"]["Hello"]);  // Expected output: "Oi!"
Console.WriteLine(provider["Messages"]["Error"]);   // Expected output: "Ocorreu um erro"
```

#### Locale Files
`locales/en.json`
```json
{
  "Greetings": {
    "Hello": "Hello!"
  },
  "Messages": {
    "Success": "Operation successful",
    "Error": "An error occurred"
  }
}
```
`locales/pt-BR.json`
```json
{
  "Greetings": {
    "Hello": "Oi!"
  },
  "Messages": {
    "Success": "Operação bem-sucedida",
    "Error": "Ocorreu um erro"
  }
}
```

