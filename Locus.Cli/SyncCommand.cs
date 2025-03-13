using System.Text.Json;
using Cocona;

namespace Locus.Cli;

public class SyncCommand
{
    public void Sync([Option('s')] string source = "locales", [Option('l')] string @default = "en")
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
                var lang = JsonSerializer.Deserialize<Dictionary<string, string?>>(locale);
                foreach (var key in defaultLangMap.Keys)
                {
                    if (!lang.ContainsKey(key))
                    {
                        Console.WriteLine("Added key '{0}' to '{1}'", key, file);
                        lang[key] = null;
                    }
                }
                // Save changes
                File.WriteAllText(file, JsonSerializer.Serialize(lang, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (JsonException)
            {
                Console.WriteLine("Invalid JSON in file '{0}'", file);
            }
        }
    }
}