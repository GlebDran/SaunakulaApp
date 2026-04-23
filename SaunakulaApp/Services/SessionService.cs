using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class SessionService
{
    private readonly LocalizationService _loc;

    public User? CurrentUser { get; private set; }
    public string Language { get; private set; } = "et";
    public bool IsLoggedIn => CurrentUser != null;
    public bool IsDarkMode { get; private set; } = false;

    public SessionService(LocalizationService loc)
    {
        _loc = loc;
    }

    public void Login(User user) => CurrentUser = user;
    public void Logout() => CurrentUser = null;

    public void SetLanguage(string lang)
    {
        Language = lang;
        _loc.SetLanguage(lang);
    }

    public void SetDarkMode(bool isDark)
    {
        IsDarkMode = isDark;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = isDark
                    ? AppTheme.Dark
                    : AppTheme.Light;
        });
    }

    public string L(string key)
    {
        return _loc.Get(key);
    }

    public string L(string key, params object[] args)
    {
        return _loc.Get(key, args);
    }
}
