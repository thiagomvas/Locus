using Cocona;

namespace Locus.Cli;

public class InitCommand
{
    public void Init([Option('o')] string output = "locales", [Option('l')] string defaultLanguage = "en")
    {
        // Create the output directory wherever the program is being ran from
        Directory.CreateDirectory(output);
        
        File.WriteAllText(Path.Combine(output, $"{defaultLanguage}.json"), "{}");
        
        Console.WriteLine("Initialized locale folder in '{0}' with default language '{1}'", output, defaultLanguage);
    }
}