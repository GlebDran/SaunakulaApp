using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly SessionService _session;
    private readonly DatabaseService _db;
    private readonly HouseService _houseService;

    public ProfilePage(SessionService session, DatabaseService db, HouseService houseService)
    {
        InitializeComponent();
        _session = session;
        _db = db;
        _houseService = houseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();

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
        await LoadFavourites();
    }

    private void ApplyLocalization()
    {
        ProfileTitleLabel.Text = _session.L("Profile_Title");
        ProfileSubtitleLabel.Text = _session.L("Profile_Subtitle");
        LoginPromptLabel.Text = _session.L("Profile_LoginPrompt");
        LoginButton.Text = _session.L("Profile_Login");
        NoAccountLabel.Text = _session.L("Profile_NoAccount");
        RegisterLabel.Text = _session.L("Profile_Register");
        BookingsLabel.Text = _session.L("Profile_Bookings");
        MemberSinceTextLabel.Text = _session.L("Profile_Since");
        LanguageSectionLabel.Text = _session.L("Profile_Language");
        KontoSectionLabel.Text = _session.L("Profile_Account");
        MyBookingsLabel.Text = _session.L("Profile_MyBookings");
        ContactMenuLabel.Text = _session.L("Profile_Contact");
        EmailMenuLabel.Text = _session.L("Details_Email");
        WebsiteMenuLabel.Text = _session.L("Profile_Website");
        FavouritesSectionLabel.Text = _session.L("Profile_Favourites");
        NoFavouritesLabel.Text = _session.L("Profile_NoFavourites");
        LogoutButton.Text = _session.L("Profile_Logout");
    }

    // ── Favourites ────────────────────────────────────────────

    private async Task LoadFavourites()
    {
        if (!_session.IsLoggedIn) return;
        var favourites = await _db.GetFavouritesByUserAsync(_session.CurrentUser!.Id);
        var allHouses = await _houseService.GetAllAsync();
        var lang = _session.Language;
        var favHouses = favourites
            .Select(f => allHouses.FirstOrDefault(h => h.Id == f.HouseId))
            .Where(h => h != null)
            .Select(h => new FavDisplay(h!, lang))
            .ToList();
        FavouritesView.ItemsSource = favHouses;
    }

    private async void Favourite_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not FavDisplay fav) return;
        FavouritesView.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId={fav.HouseId}");
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
            foreach (var child in vsl.Children)
                if (child is Label lbl && lbl.FontSize <= 13)
                    lbl.TextColor = isActive ? Colors.White : Color.FromArgb("#2D3B2F");
    }

    private void LangEt_Tapped(object sender, TappedEventArgs e) => SetLanguage("et");
    private void LangRu_Tapped(object sender, TappedEventArgs e) => SetLanguage("ru");
    private void LangEn_Tapped(object sender, TappedEventArgs e) => SetLanguage("en");
    private void LangFi_Tapped(object sender, TappedEventArgs e) => SetLanguage("fi");

    private void SetLanguage(string lang)
    {
        _session.SetLanguage(lang);
        UpdateLanguageUI(lang);
        ApplyLocalization();
        DisplayAlert("✅", "Keel muudetud / Язык изменён", "OK");
    }

    // ── Menu actions ──────────────────────────────────────────

    private async void MyBookings_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//BookingsPage");

    private async void Call_Tapped(object sender, TappedEventArgs e)
    {
        if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open("+37255000075");
        else await DisplayAlert("Telefon", "+372 5500 075", "OK");
    }

    private async void Website_Tapped(object sender, TappedEventArgs e)
        => await Browser.Default.OpenAsync("https://saunakula.ee", BrowserLaunchMode.SystemPreferred);

    private async void Instagram_Tapped(object sender, TappedEventArgs e)
        => await Browser.Default.OpenAsync("https://www.instagram.com/sauna.kula/", BrowserLaunchMode.SystemPreferred);

    private async void Email_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (!Email.Default.IsComposeSupported) { await DisplayAlert("E-post", "sauna@saunamaailm.ee", "OK"); return; }
            await Email.Default.ComposeAsync(new EmailMessage
            {
                Subject = "Küsimus / Вопрос",
                Body = "",
                To = new List<string> { "sauna@saunamaailm.ee" }
            });
        }
        catch (FeatureNotSupportedException) { await DisplayAlert("E-post", "sauna@saunamaailm.ee", "OK"); }
    }

    private async void Login_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(LoginPage));

    private async void Register_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(RegisterPage));

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        string q = _session.Language switch
        {
            "ru" => "Вы уверены, что хотите выйти?",
            "en" => "Are you sure you want to log out?",
            "fi" => "Haluatko varmasti kirjautua ulos?",
            _ => "Kas oled kindel, et soovid välja logida?"
        };
        if (!await DisplayAlert(_session.L("Profile_Logout"), q,
            _session.L("Common_Yes"), _session.L("Common_No"))) return;
        _session.Logout();
        LoggedInView.IsVisible = false;
        NotLoggedInView.IsVisible = true;
        ApplyLocalization();
    }
}

public class FavDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image { get; }
    public FavDisplay(SaunakulaApp.Models.House house, string lang)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        DisplayPrice = $"€{house.Price24h}";
        Image = house.Image;
    }
}
