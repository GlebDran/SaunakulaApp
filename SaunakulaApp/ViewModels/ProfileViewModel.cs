using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SaunakulaApp.Models;
using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private const int VipThreshold = 10;
    private const string ContactPhoneDisplay = "+372 5500 075";
    private const string ContactPhoneDial = "+37255000075";
    private const string ContactEmail = "sauna@saunamaailm.ee";
    private const string WebsiteUrl = "https://saunakula.ee";
    private const string InstagramUrl = "https://www.instagram.com/sauna.kula/";

    private static readonly Color LanguageActiveBackground = Color.FromArgb("#5A7C5E");
    private static readonly Color LanguageInactiveBackground = Color.FromArgb("#E8EDE7");
    private static readonly Color LanguageActiveText = Colors.White;
    private static readonly Color LanguageInactiveText = Color.FromArgb("#2D3B2F");

    private readonly SessionService _sessionService;
    private readonly DatabaseService _databaseService;
    private readonly HouseService _houseService;

    [ObservableProperty] private bool isLoggedInVisible;
    [ObservableProperty] private bool isNotLoggedInVisible;
    [ObservableProperty] private string profileTitleText = string.Empty;
    [ObservableProperty] private string profileSubtitleText = string.Empty;
    [ObservableProperty] private string loginPromptText = string.Empty;
    [ObservableProperty] private string loginText = string.Empty;
    [ObservableProperty] private string noAccountText = string.Empty;
    [ObservableProperty] private string registerText = string.Empty;
    [ObservableProperty] private string avatarText = string.Empty;
    [ObservableProperty] private string nameText = string.Empty;
    [ObservableProperty] private string emailText = string.Empty;
    [ObservableProperty] private string phoneText = string.Empty;
    [ObservableProperty] private string bookingsCountText = "0";
    [ObservableProperty] private string bookingsLabelText = string.Empty;
    [ObservableProperty] private string memberSinceText = string.Empty;
    [ObservableProperty] private string memberSinceLabelText = string.Empty;
    [ObservableProperty] private string vipBadgeText = "⭐";
    [ObservableProperty] private string vipStatusText = "VIP";
    [ObservableProperty] private Color vipProgressBackground = Color.FromArgb("#2D3B2F");
    [ObservableProperty] private string vipProgressTitleText = string.Empty;
    [ObservableProperty] private string vipProgressCountText = string.Empty;
    [ObservableProperty] private string vipProgressSubtitleText = string.Empty;
    [ObservableProperty] private double vipProgress;
    [ObservableProperty] private string languageSectionText = string.Empty;
    [ObservableProperty] private string accountSectionText = string.Empty;
    [ObservableProperty] private string myBookingsText = string.Empty;
    [ObservableProperty] private string contactText = string.Empty;
    [ObservableProperty] private string emailMenuText = string.Empty;
    [ObservableProperty] private string websiteText = string.Empty;
    [ObservableProperty] private string favouritesText = string.Empty;
    [ObservableProperty] private string noFavouritesText = string.Empty;
    [ObservableProperty] private string logoutText = string.Empty;
    [ObservableProperty] private Color langEtBackground = LanguageActiveBackground;
    [ObservableProperty] private Color langRuBackground = LanguageInactiveBackground;
    [ObservableProperty] private Color langEnBackground = LanguageInactiveBackground;
    [ObservableProperty] private Color langFiBackground = LanguageInactiveBackground;
    [ObservableProperty] private Color langEtTextColor = LanguageActiveText;
    [ObservableProperty] private Color langRuTextColor = LanguageInactiveText;
    [ObservableProperty] private Color langEnTextColor = LanguageInactiveText;
    [ObservableProperty] private Color langFiTextColor = LanguageInactiveText;

    public ObservableCollection<ProfileFavouriteDisplay> Favourites { get; } = new();

    public IAsyncRelayCommand InitializeCommand { get; }
    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand RegisterCommand { get; }
    public IAsyncRelayCommand MyBookingsCommand { get; }
    public IAsyncRelayCommand CallCommand { get; }
    public IAsyncRelayCommand EmailCommand { get; }
    public IAsyncRelayCommand WebsiteCommand { get; }
    public IAsyncRelayCommand InstagramCommand { get; }
    public IAsyncRelayCommand LogoutCommand { get; }
    public IAsyncRelayCommand<string> SelectLanguageCommand { get; }
    public IAsyncRelayCommand<string> OpenFavouriteCommand { get; }

    public ProfileViewModel(
        SessionService sessionService,
        DatabaseService databaseService,
        HouseService houseService)
    {
        _sessionService = sessionService;
        _databaseService = databaseService;
        _houseService = houseService;

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        LoginCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(LoginPage)));
        RegisterCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(RegisterPage)));
        MyBookingsCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync("//BookingsPage"));
        CallCommand = new AsyncRelayCommand(CallAsync);
        EmailCommand = new AsyncRelayCommand(EmailAsync);
        WebsiteCommand = new AsyncRelayCommand(() => Browser.Default.OpenAsync(WebsiteUrl, BrowserLaunchMode.SystemPreferred));
        InstagramCommand = new AsyncRelayCommand(() => Browser.Default.OpenAsync(InstagramUrl, BrowserLaunchMode.SystemPreferred));
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        SelectLanguageCommand = new AsyncRelayCommand<string>(SetLanguageAsync);
        OpenFavouriteCommand = new AsyncRelayCommand<string>(OpenFavouriteAsync);
    }

    public async Task InitializeAsync()
    {
        ApplyLocalization();
        UpdateLanguageUi(_sessionService.Language);

        if (!_sessionService.IsLoggedIn)
        {
            IsLoggedInVisible = false;
            IsNotLoggedInVisible = true;
            Favourites.Clear();
            return;
        }

        IsNotLoggedInVisible = false;
        IsLoggedInVisible = true;

        await LoadLoggedInProfileAsync();
    }

    private async Task LoadLoggedInProfileAsync()
    {
        try
        {
            IsBusy = true;

            var user = _sessionService.CurrentUser!;
            AvatarText = GetInitials(user.FullName, user.Email);
            NameText = user.FullName;
            EmailText = user.Email;
            PhoneText = string.IsNullOrWhiteSpace(user.Phone) ? string.Empty : user.Phone;
            MemberSinceText = user.CreatedAt.ToString("MM.yyyy");

            await _databaseService.InitAsync();
            var bookings = await _databaseService.GetBookingsByUserAsync(user.Id);
            BookingsCountText = bookings.Count.ToString();

            var (vipCount, isVip) = await _databaseService.CheckAndUpdateVipAsync(user.Id);
            UpdateVipUi(vipCount, isVip);

            await LoadFavouritesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyLocalization()
    {
        ProfileTitleText = _sessionService.L("Profile_Title");
        ProfileSubtitleText = _sessionService.L("Profile_Subtitle");
        LoginPromptText = _sessionService.L("Profile_LoginPrompt");
        LoginText = _sessionService.L("Profile_Login");
        NoAccountText = _sessionService.L("Profile_NoAccount");
        RegisterText = _sessionService.L("Profile_Register");
        BookingsLabelText = _sessionService.L("Profile_Bookings");
        MemberSinceLabelText = _sessionService.L("Profile_Since");
        LanguageSectionText = _sessionService.L("Profile_Language");
        AccountSectionText = _sessionService.L("Profile_Account");
        MyBookingsText = _sessionService.L("Profile_MyBookings");
        ContactText = _sessionService.L("Profile_Contact");
        EmailMenuText = _sessionService.L("Details_Email");
        WebsiteText = _sessionService.L("Profile_Website");
        FavouritesText = _sessionService.L("Profile_Favourites");
        NoFavouritesText = _sessionService.L("Profile_NoFavourites");
        LogoutText = _sessionService.L("Profile_Logout");
    }

    private void UpdateVipUi(int countThisYear, bool isVip)
    {
        var language = _sessionService.Language;

        if (isVip)
        {
            VipBadgeText = "👑";
            VipStatusText = "VIP";
            VipProgressBackground = Color.FromArgb("#3D5C41");
            VipProgress = 1;
            VipProgressTitleText = language switch
            {
                "ru" => "👑 VIP статус активен!",
                "en" => "👑 VIP status active!",
                "fi" => "👑 VIP-tila aktiivinen!",
                _ => "👑 VIP staatus aktiivne!"
            };
            VipProgressCountText = language switch
            {
                "ru" => $"{countThisYear} бронирований",
                "en" => $"{countThisYear} bookings",
                "fi" => $"{countThisYear} varausta",
                _ => $"{countThisYear} broneeringut"
            };
            VipProgressSubtitleText = language switch
            {
                "ru" => "🌿 Веники бесплатно при бронировании!",
                "en" => "🌿 Free whisks with every booking!",
                "fi" => "🌿 Ilmaiset vihdat jokaisessa varauksessa!",
                _ => "🌿 Viht tasuta iga broneeringu juures!"
            };
            return;
        }

        var clamped = Math.Min(countThisYear, VipThreshold);
        var remaining = VipThreshold - clamped;

        VipBadgeText = "⭐";
        VipStatusText = language switch
        {
            "ru" => "Обычный",
            "en" => "Regular",
            "fi" => "Tavallinen",
            _ => "Tavaline"
        };
        VipProgressBackground = Color.FromArgb("#2D3B2F");
        VipProgress = (double)clamped / VipThreshold;
        VipProgressTitleText = language switch
        {
            "ru" => "До VIP статуса",
            "en" => "Progress to VIP",
            "fi" => "Edistyminen VIP-tilaan",
            _ => "Progress VIP staatuseni"
        };
        VipProgressCountText = $"{clamped}/{VipThreshold}";
        VipProgressSubtitleText = remaining > 0
            ? language switch
            {
                "ru" => $"Ещё {remaining} бронирований за 12 месяцев для VIP",
                "en" => $"{remaining} more bookings in 12 months for VIP",
                "fi" => $"Vielä {remaining} varausta 12 kuukaudessa VIP-tilaan",
                _ => $"Veel {remaining} broneeringut 12 kuu jooksul VIP staatuse saamiseks"
            }
            : string.Empty;
    }

    private async Task LoadFavouritesAsync()
    {
        if (!_sessionService.IsLoggedIn)
            return;

        var favourites = await _databaseService.GetFavouritesByUserAsync(_sessionService.CurrentUser!.Id);
        var allHouses = await _houseService.GetAllAsync();
        var language = _sessionService.Language;

        Favourites.Clear();
        foreach (var favourite in favourites)
        {
            var house = allHouses.FirstOrDefault(item => item.Id == favourite.HouseId);
            if (house is not null)
                Favourites.Add(new ProfileFavouriteDisplay(house, language));
        }
    }

    private async Task SetLanguageAsync(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return;

        _sessionService.SetLanguage(language);
        ApplyLocalization();
        UpdateLanguageUi(language);

        if (_sessionService.IsLoggedIn)
            await LoadLoggedInProfileAsync();

        await Shell.Current.DisplayAlert("✅", "Keel muudetud / Язык изменён", "OK");
    }

    private void UpdateLanguageUi(string language)
    {
        LangEtBackground = BackgroundFor(language, "et");
        LangRuBackground = BackgroundFor(language, "ru");
        LangEnBackground = BackgroundFor(language, "en");
        LangFiBackground = BackgroundFor(language, "fi");
        LangEtTextColor = TextColorFor(language, "et");
        LangRuTextColor = TextColorFor(language, "ru");
        LangEnTextColor = TextColorFor(language, "en");
        LangFiTextColor = TextColorFor(language, "fi");
    }

    private static Color BackgroundFor(string current, string candidate)
        => current == candidate ? LanguageActiveBackground : LanguageInactiveBackground;

    private static Color TextColorFor(string current, string candidate)
        => current == candidate ? LanguageActiveText : LanguageInactiveText;

    private static async Task CallAsync()
    {
        if (PhoneDialer.Default.IsSupported)
        {
            PhoneDialer.Default.Open(ContactPhoneDial);
            return;
        }

        await Shell.Current.DisplayAlert("Telefon", ContactPhoneDisplay, "OK");
    }

    private static async Task EmailAsync()
    {
        try
        {
            if (!Email.Default.IsComposeSupported)
            {
                await Shell.Current.DisplayAlert("E-post", ContactEmail, "OK");
                return;
            }

            await Email.Default.ComposeAsync(new EmailMessage
            {
                Subject = "Küsimus / Вопрос",
                Body = string.Empty,
                To = new List<string> { ContactEmail }
            });
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlert("E-post", ContactEmail, "OK");
        }
    }

    private async Task OpenFavouriteAsync(string? houseId)
    {
        if (!string.IsNullOrWhiteSpace(houseId))
            await Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId={houseId}");
    }

    private async Task LogoutAsync()
    {
        var question = _sessionService.Language switch
        {
            "ru" => "Вы уверены, что хотите выйти?",
            "en" => "Are you sure you want to log out?",
            "fi" => "Haluatko varmasti kirjautua ulos?",
            _ => "Kas oled kindel, et soovid välja logida?"
        };

        var confirmed = await Shell.Current.DisplayAlert(
            _sessionService.L("Profile_Logout"),
            question,
            _sessionService.L("Common_Yes"),
            _sessionService.L("Common_No"));

        if (!confirmed)
            return;

        _sessionService.Logout();
        Favourites.Clear();
        IsLoggedInVisible = false;
        IsNotLoggedInVisible = true;
        ApplyLocalization();
    }

    private static string GetInitials(string fullName, string email)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();

        if (parts.Length == 1 && parts[0].Length > 0)
            return parts[0][..1].ToUpperInvariant();

        return string.IsNullOrWhiteSpace(email) ? "U" : email[..1].ToUpperInvariant();
    }
}

public class ProfileFavouriteDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image { get; }

    public ProfileFavouriteDisplay(House house, string language)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(language);
        DisplayPrice = $"€{house.Price24h}";
        Image = house.Image;
    }
}
