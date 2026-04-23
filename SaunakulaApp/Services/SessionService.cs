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
            if (Application.Current is null) return;

            // Меняем системную тему
            Application.Current.UserAppTheme = isDark
                ? AppTheme.Dark
                : AppTheme.Light;

            // Меняем цвета через MergedDictionaries
            var dict = Application.Current.Resources;

            if (isDark)
            {
                dict["Background"] = Color.FromArgb("#1A2420");
                dict["CardBackground"] = Color.FromArgb("#2D3B2F");
                dict["SecondaryBackground"] = Color.FromArgb("#243028");
                dict["MutedBackground"] = Color.FromArgb("#1F2B28");
                dict["TextPrimary"] = Color.FromArgb("#E8EDE7");
                dict["TextSecondary"] = Color.FromArgb("#A0B09D");
                dict["Border"] = Color.FromArgb("#3A4E3D");
            }
            else
            {
                dict["Background"] = Color.FromArgb("#FAFAF8");
                dict["CardBackground"] = Color.FromArgb("#FFFFFF");
                dict["SecondaryBackground"] = Color.FromArgb("#E8EDE7");
                dict["MutedBackground"] = Color.FromArgb("#F5F5F3");
                dict["TextPrimary"] = Color.FromArgb("#2D3B2F");
                dict["TextSecondary"] = Color.FromArgb("#7A8A7D");
                dict["Border"] = Color.FromArgb("#E8EDE7");
            }
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
