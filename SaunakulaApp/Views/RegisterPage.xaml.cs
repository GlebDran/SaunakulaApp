using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class RegisterPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly SessionService _session;

    public RegisterPage(DatabaseService db, SessionService session)
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

    private async void Register_Clicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var phone = PhoneEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";
        var confirm = ConfirmPasswordEntry.Text ?? "";

        // Валидация
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            ShowError("Palun täida kõik kohustuslikud väljad.");
            return;
        }

        if (password != confirm)
        {
            ShowError("Paroolid ei ühti.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Parool peab olema vähemalt 6 tähemärki.");
            return;
        }

        // Проверка существующего пользователя
        var existing = await _db.GetUserByEmailAsync(email);
        if (existing != null)
        {
            ShowError("See e-posti aadress on juba kasutusel.");
            return;
        }

        // Создаём пользователя
        var user = new User
        {
            FullName = name,
            Email = email,
            Phone = phone,
            PasswordHash = LoginPage.HashPassword(password),
            CreatedAt = DateTime.Now
        };

        await _db.InsertUserAsync(user);

        // Сразу логиним
        var created = await _db.GetUserByEmailAsync(email);
        _session.Login(created!);

        await DisplayAlert("✅", "Konto loodud! Tere tulemast!", "OK");
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void GoToLogin_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");

    private void ShowError(string msg)
    {
        ErrorLabel.Text = msg;
        ErrorLabel.IsVisible = true;
    }
}