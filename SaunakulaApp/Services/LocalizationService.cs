using System.Resources;
using System.Globalization;

namespace SaunakulaApp.Services;

public class LocalizationService
{
    private ResourceManager? _rm;
    private string _lang = "et";

    public void SetLanguage(string lang)
    {
        _lang = lang;
        _rm = null; // сбросим кеш
    }

    private ResourceManager GetManager()
    {
        if (_rm != null) return _rm;

        // Базовый файл AppResources.resx — эстонский
        // Остальные языки: AppResources.ru.resx, .en.resx, .fi.resx
        _rm = new ResourceManager(
            "SaunakulaApp.Resources.Languages.AppResources",
            typeof(LocalizationService).Assembly);

        return _rm;
    }

    public string Get(string key)
    {
        try
        {
            var culture = _lang switch
            {
                "ru" => new CultureInfo("ru"),
                "en" => new CultureInfo("en"),
                "fi" => new CultureInfo("fi"),
                _ => CultureInfo.InvariantCulture
            };

            var value = GetManager().GetString(key, culture);
            return string.IsNullOrEmpty(value) ? key : value;
        }
        catch
        {
            return key;
        }
    }

    public string Get(string key, params object[] args)
    {
        var template = Get(key);
        try { return string.Format(template, args); }
        catch { return template; }
    }
}
