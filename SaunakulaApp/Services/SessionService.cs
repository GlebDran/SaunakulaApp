using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

// Хранит текущего залогиненного пользователя и язык
public class SessionService
{
    public User? CurrentUser { get; private set; }
    public string Language { get; private set; } = "et";
    public bool IsLoggedIn => CurrentUser != null;

    public void Login(User user) => CurrentUser = user;
    public void Logout() => CurrentUser = null;
    public void SetLanguage(string lang) => Language = lang;

    public bool IsDarkMode { get; private set; } = false;

    public void SetDarkMode(bool isDark)
    {
        IsDarkMode = isDark;
        Application.Current!.UserAppTheme = isDark
            ? AppTheme.Dark
            : AppTheme.Light;
    }
}