using System.Text.Json;
using Cocona;

namespace Locus.Cli;

public class ValidateCommand
{
    public void Validate([Option('s')] string source = "locales", [Option('l')] string @default = "en")
    {
        var localeFiles = Directory.GetFiles(source, "*.json");
        
        // Validate the default language file first
        var defaultLanguageFile = Path.Combine(source, $"{@default}.json");
        if (!File.Exists(defaultLanguageFile))
        {
            Console.WriteLine("Default language file '{0}' does not exist", defaultLanguageFile);
            return;
        }
        
        var defaultLanguageJson = File.ReadAllText(defaultLanguageFile);
        Dictionary<string, string> defaultLangMap;
        try
        {
            defaultLangMap = JsonSerializer.Deserialize<Dictionary<string, string>>(defaultLanguageJson);
        }
        catch (JsonException)
        {
            Console.WriteLine("Invalid JSON in default language file '{0}'", defaultLanguageFile);
            return;
        }
        
        foreach (var file in localeFiles)
        {
            var locale = File.ReadAllText(file);
            try
            {
                var lang = JsonSerializer.Deserialize<Dictionary<string, string>>(locale);
                // Check if the keys in the default language file are present in the current language file
                foreach (var key in defaultLangMap.Keys)
                {
                    if (!lang.ContainsKey(key))
                    {
                        Console.WriteLine("Key '{0}' is missing in file '{1}'", key, file);
                    }
                }
                
                // Check if it has any extra keys
                foreach (var key in lang.Keys)
                {
                    if (!defaultLangMap.ContainsKey(key))
                    {
                        Console.WriteLine("Key '{0}' is extra in file '{1}'", key, file);
                    }
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("Invalid JSON in file '{0}'", file);
            }
        }
        
    }
}