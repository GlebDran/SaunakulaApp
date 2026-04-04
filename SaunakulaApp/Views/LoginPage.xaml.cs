using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly SessionService _session;

    public LoginPage(DatabaseService db, SessionService session)
    {
        InitializeComponent();
        _db = db;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _db.InitAsync();
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Palun täida kõik väljad.");
            return;
        }

        var user = await _db.GetUserByEmailAsync(email);

        if (user is null || user.PasswordHash != HashPassword(password))
        {
            ShowError("Vale e-post või parool.");
            return;
        }

        _session.Login(user);
        await Shell.Current.GoToAsync("..");
    }

    private async void GoToRegister_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(RegisterPage));

    private void ShowError(string msg)
    {
        ErrorLabel.Text = msg;
        ErrorLabel.IsVisible = true;
    }

    // Простое хеширование (для диплома достаточно)
    public static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}