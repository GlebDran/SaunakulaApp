using System.Resources;
using System.Globalization;

namespace SaunakulaApp.Services;

public class LocalizationService
{
    private readonly Dictionary<string, ResourceManager> _managers = new();
    private string _currentLang = "et";

    public LocalizationService()
    {
        // Инициализируем менеджеры для каждого языка
        foreach (var lang in new[] { "et", "ru", "en", "fi" })
        {
            _managers[lang] = new ResourceManager(
                $"SaunakulaApp.Resources.Languages.AppResources.{lang}",
                typeof(LocalizationService).Assembly);
        }
    }

    public void SetLanguage(string lang)
    {
        _currentLang = lang;
    }

    public string Get(string key)
    {
        try
        {
            return _managers[_currentLang].GetString(key)
                   ?? _managers["et"].GetString(key)
                   ?? key;
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