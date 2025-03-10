namespace Locus;

/// <summary>
/// Provides localization support by loading and managing locale data.
/// </summary>
public class LocalizationProvider
{
    private readonly Dictionary<string, List<string>> _localeChains = new();
    private string _currentLocale;
    private readonly LocaleConfiguration _config;
    private readonly Lazy<Dictionary<string, LocaleNode>> _localeCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationProvider"/> class.
    /// </summary>
    /// <param name="defaultLocale">The default locale to use if none is specified.</param>
    public LocalizationProvider(LocaleConfiguration config)
    {
        _currentLocale = config.DefaultLocale;
        _localeCache = new Lazy<Dictionary<string, LocaleNode>>(LoadLocales);
        _config = config;
    }

    /// <summary>
    /// Gets the <see cref="LocaleNode"/> associated with the specified key for the current locale.
    /// </summary>
    /// <param name="key">The key to look up in the locale data.</param>
    /// <returns>The <see cref="LocaleNode"/> if found; otherwise, null.</returns>
    public LocaleNode? this[string key]
    {
        get
        {
            var localeNode = TryGetLocaleNode(_currentLocale, key);
            return localeNode;
        }
    }

    /// <summary>
    /// Sets the current locale.
    /// </summary>
    /// <param name="locale">The locale to set as the current locale.</param>
    public void SetLocale(string locale)
    {
        _currentLocale = locale;
    }

    /// <summary>
    /// Loads the locale data from JSON files.
    /// </summary>
    /// <returns>A dictionary containing the loaded locale data.</returns>
    private Dictionary<string, LocaleNode> LoadLocales()
    {
        var locales = new Dictionary<string, LocaleNode>();
        var localeFiles = Directory.GetFiles(_config.LocalesPath, "*.json");

        foreach (var file in localeFiles)
        {
            var localeName = Path.GetFileNameWithoutExtension(file);
            if (localeName == null) continue;

            var jsonContent = File.ReadAllText(file);
            var localeNode = LocaleNode.FromJson(jsonContent);
            locales[localeName] = localeNode;
        }

        foreach (var node in locales.Values)
        {
            node.RecursivePopulateWithDefaults(locales[_config.DefaultLocale]);
        }

        return locales;
    }

    /// <summary>
    /// Tries to get the <see cref="LocaleNode"/> for the specified locale and key.
    /// </summary>
    /// <param name="locale">The locale to look up.</param>
    /// <param name="key">The key to look up in the locale data.</param>
    /// <returns>The <see cref="LocaleNode"/> if found; otherwise, null.</returns>
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

    /// <summary>
    /// Gets the chain of locales to try for the specified locale.
    /// </summary>
    /// <param name="locale">The locale to get the chain for.</param>
    /// <returns>A list of locales to try.</returns>
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

        chain.Add(_config.DefaultLocale);
        _localeChains[locale] = chain;

        return chain;
    }

    /// <summary>
    /// Gets the current locale.
    /// </summary>
    /// <returns>The current locale.</returns>
    public string GetCurrentLocale() => _currentLocale;
}