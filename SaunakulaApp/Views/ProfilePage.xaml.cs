using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly SessionService _session;
    private readonly DatabaseService _db;

    public ProfilePage(SessionService session, DatabaseService db)
    {
        InitializeComponent();
        _session = session;
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_session.IsLoggedIn)
        {
            LoggedInView.IsVisible = false;
            NotLoggedInView.IsVisible = true;
            return;
        }

        NotLoggedInView.IsVisible = false;
        LoggedInView.IsVisible = true;

        var user = _session.CurrentUser!;

        var parts = user.FullName.Split(' ');
        AvatarLabel.Text = parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}"
            : user.FullName[..1].ToUpper();

        NameLabel.Text = user.FullName;
        EmailLabel.Text = user.Email;
        PhoneLabel.Text = string.IsNullOrEmpty(user.Phone) ? "" : user.Phone;
        MemberSinceLabel.Text = user.CreatedAt.ToString("MM.yyyy");

        await _db.InitAsync();
        var bookings = await _db.GetBookingsByUserAsync(user.Id);
        BookingsCountLabel.Text = bookings.Count.ToString();

        UpdateLanguageUI(_session.Language);
        DarkModeSwitch.IsToggled = _session.IsDarkMode;
    }

    // ── Dark mode ─────────────────────────────────────────────

    private void DarkMode_Toggled(object sender, ToggledEventArgs e)
    {
        _session.SetDarkMode(e.Value);
    }

    // ── Language ──────────────────────────────────────────────

    private void UpdateLanguageUI(string lang)
    {
        var active = Color.FromArgb("#5A7C5E");
        var inactive = Color.FromArgb("#E8EDE7");

        LangEt.BackgroundColor = lang == "et" ? active : inactive;
        LangRu.BackgroundColor = lang == "ru" ? active : inactive;
        LangEn.BackgroundColor = lang == "en" ? active : inactive;
        LangFi.BackgroundColor = lang == "fi" ? active : inactive;

        SetLangTextColor(LangEt, lang == "et");
        SetLangTextColor(LangRu, lang == "ru");
        SetLangTextColor(LangEn, lang == "en");
        SetLangTextColor(LangFi, lang == "fi");
    }

    private static void SetLangTextColor(Frame frame, bool isActive)
    {
        if (frame.Content is VerticalStackLayout vsl)
        {
            foreach (var child in vsl.Children)
            {
                if (child is Label lbl && lbl.FontSize <= 13)
                    lbl.TextColor = isActive
                        ? Colors.White
                        : Color.FromArgb("#2D3B2F");
            }
        }
    }

    private void LangEt_Tapped(object sender, TappedEventArgs e) => SetLanguage("et");
    private void LangRu_Tapped(object sender, TappedEventArgs e) => SetLanguage("ru");
    private void LangEn_Tapped(object sender, TappedEventArgs e) => SetLanguage("en");
    private void LangFi_Tapped(object sender, TappedEventArgs e) => SetLanguage("fi");

    private void SetLanguage(string lang)
    {
        _session.SetLanguage(lang);
        UpdateLanguageUI(lang);
        DisplayAlert("✅", "Keel muudetud / Язык изменён", "OK");
    }

    // ── Menu actions ──────────────────────────────────────────

    private async void MyBookings_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//BookingsPage");

    private void Call_Tapped(object sender, TappedEventArgs e)
    {
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("+37255000075");
    }

    private async void Website_Tapped(object sender, TappedEventArgs e)
        => await Browser.Default.OpenAsync(
            "https://saunakula.ee",
            BrowserLaunchMode.SystemPreferred);

    private async void Email_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            var message = new EmailMessage
            {
                Subject = "Küsimus / Вопрос",
                Body = "",
                To = new List<string> { "sauna@saunamaailm.ee" }
            };
            await Email.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Viga", "E-post ei ole toetatud", "OK");
        }
    }

    private async void Login_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(LoginPage));

    private async void Register_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(RegisterPage));

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Logi välja",
            "Kas oled kindel, et soovid välja logida?",
            "Jah",
            "Ei");

        if (!confirm) return;

        _session.Logout();
        LoggedInView.IsVisible = false;
        NotLoggedInView.IsVisible = true;
    }
}
