namespace Locus;
public class LocalizationProvider
{
    private readonly Dictionary<string, LocaleNode> _locales = new();
    private readonly Dictionary<string, List<string>> _localeChains = new();
    private string _currentLocale;
    private readonly string _defaultLocale = "en";
    private readonly Lazy<Dictionary<string, LocaleNode>> _localeCache;

    public LocalizationProvider(string defaultLocale = "en")
    {
        _currentLocale = defaultLocale;
        _localeCache = new Lazy<Dictionary<string, LocaleNode>>(LoadLocales);
    }

    public LocaleNode? this[string key]
    {
        get
        {
            var localeNode = TryGetLocaleNode(_currentLocale, key);
            return localeNode ?? new LocaleNode("Missing localization for key: " + key);
        }
    }

    public void SetLocale(string locale)
    {
        _currentLocale = locale;
    }

    private Dictionary<string, LocaleNode> LoadLocales()
    {
        var locales = new Dictionary<string, LocaleNode>();
        var localeFiles = Directory.GetFiles(LocusConstants.LOCALES_PATH, "*.json");

        foreach (var file in localeFiles)
        {
            var localeName = Path.GetFileNameWithoutExtension(file);
            if (localeName == null) continue;

            var jsonContent = File.ReadAllText(file);
            var localeNode = LocaleNode.FromJson(jsonContent);
            locales[localeName] = localeNode;
        }
        
        foreach(var node in locales.Values)
        {
            node.RecursivePopulateWithDefaults(locales[_defaultLocale]);
        }
        
        return locales;
    }

    private LocaleNode? TryGetLocaleNode(string locale, string key)
    {
        var localeChain = GetLocaleChain(locale);

        foreach (var localeToTry in localeChain)
        {
            if (_localeCache.Value.ContainsKey(localeToTry))
            {
                var localeNode = _localeCache.Value[localeToTry][key];
                if (localeNode != null)
                {
                    return localeNode;
                }
            }
        }
        
        return null;
    }

    private List<string> GetLocaleChain(string locale)
    {
        if (_localeChains.ContainsKey(locale))
        {
            return _localeChains[locale];
        }

        var parts = locale.Split('-');
        var chain = new List<string> { locale };

        if (parts.Length > 1)
        {
            chain.Add(parts[0]);
        }

        chain.Add(_defaultLocale);
        _localeChains[locale] = chain;

        return chain;
    }

    public string GetCurrentLocale() => _currentLocale;
}