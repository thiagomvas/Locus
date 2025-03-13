using Cocona;

namespace Locus.Cli;

public class InitCommand
{
    [Command(Description = "Initialize the locale folder with a default language file.")]
    public void Init([Option('o', Description = "The folder that will contain the localization files.")] string output = "locales", [Option('l', Description = "The default language file name (without extension)")] string defaultLanguage = "en")
    {
        // Create the output directory wherever the program is being ran from
        Directory.CreateDirectory(output);
        
        File.WriteAllText(Path.Combine(output, $"{defaultLanguage}.json"), "{}");
        
        Console.WriteLine("Initialized locale folder in '{0}' with default language '{1}'", output, defaultLanguage);
    }
}