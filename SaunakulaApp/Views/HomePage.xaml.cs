using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class HomePage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;
    private List<HomeHouseDisplay> _allHouses = new();
    private House? _featuredHouse;

    public HomePage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        await LoadData();
        UpdateGreeting();
    }

    private void ApplyLocalization()
    {
        var lang = _session.Language;

        HomeSubtitleLabel.Text = _session.L("Home_Subtitle");
        HomeTitleLabel.Text = _session.L("Home_Title");
        ActivitiesLabel.Text = _session.L("Home_Activities");
        FeaturedLabel.Text = _session.L("Home_Featured");
        AllHousesLabel.Text = _session.L("Home_AllHouses");
        FeaturedBadgeLabel.Text = _session.L("Home_FeaturedBadge");
        FeaturedDetailsButton.Text = _session.L("Pricing_Details");
        FeaturedBookButton.Text = _session.L("Details_Book");

        // Категории
        SetCategoryLabel(CatAll, _session.L("Home_AllHouses"));
        SetCategoryLabel(CatSaun, "Saun");
        SetCategoryLabel(CatBassein, "Bassein");
        SetCategoryLabel(CatKaraoke, "Karaoke");
        SetCategoryLabel(CatSuur, lang switch
        {
            "ru" => "Большая группа",
            "en" => "Large group",
            "fi" => "Suuri ryhmä",
            _ => "Suur grupp"
        });

        // Баннер PäevaSPA
        BannerTitleLabel.Text = "PäevaSPA -40%";
        BannerSubLabel.Text = lang switch
        {
            "ru" => "Пн–Чт 10:00–17:00 · Дом охотника & Русский дом",
            "en" => "Mon–Thu 10:00–17:00 · Hunter's Lodge & Russian House",
            "fi" => "Ma–To 10:00–17:00 · Metsästäjän talo & Venäläinen talo",
            _ => "E–N kl 10:00–17:00 · Jahimehe & Vene maja"
        };
    }

    private static void SetCategoryLabel(Frame cat, string text)
    {
        if (cat.Content is HorizontalStackLayout hsl)
            foreach (var child in hsl.Children)
                if (child is Label lbl && lbl.FontSize <= 13)
                    lbl.Text = text;
    }

    private async Task LoadData()
    {
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        _allHouses = houses
            .Select(h => new HomeHouseDisplay(h, lang, _session))
            .ToList();

        _featuredHouse = houses.FirstOrDefault(h => h.Id == "soome")
                         ?? houses.FirstOrDefault();

        if (_featuredHouse != null)
        {
            FeaturedImage.Source = _featuredHouse.Image;
            FeaturedTitle.Text = _featuredHouse.GetTitle(lang);
            FeaturedPrice.Text = $"€{_featuredHouse.Price24h}";
            FeaturedGuests.Text = _session.L("Home_GuestsUpTo", _featuredHouse.MaxGuests);
        }

        HousesView.ItemsSource = _allHouses
            .Where(h => h.HouseId != "soome")
            .ToList();
    }

    private void UpdateGreeting()
    {
        if (_session.IsLoggedIn)
        {
            var name = _session.CurrentUser!.FullName.Split(' ')[0];
            var greeting = _session.L("Home_Greeting").Replace("👋", "").Trim();
            GreetingLabel.Text = $"{greeting}, {name}! 👋";
            AvatarLabel.Text = name[..1].ToUpper();
        }
        else
        {
            GreetingLabel.Text = _session.L("Home_Greeting");
            AvatarLabel.Text = "👤";
        }
    }

    // ── Avatar ────────────────────────────────────────────────

    private async void Avatar_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//ProfilePage");

    // ── Featured ──────────────────────────────────────────────

    private async void FeaturedDetails_Clicked(object sender, EventArgs e)
    {
        if (_featuredHouse is null) return;
        await Shell.Current.GoToAsync(
            $"{nameof(HouseDetailsPage)}?houseId={_featuredHouse.Id}");
    }

    private async void FeaturedBook_Clicked(object sender, EventArgs e)
    {
        if (_featuredHouse is null) return;
        if (_session.IsLoggedIn)
            await Shell.Current.GoToAsync(
                $"{nameof(BookingPage)}?houseId={_featuredHouse.Id}");
        else
            await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    // ── House list ────────────────────────────────────────────

    private async void HouseDetails_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string houseId)
            await Shell.Current.GoToAsync(
                $"{nameof(HouseDetailsPage)}?houseId={houseId}");
    }

    // ── Promo banner ──────────────────────────────────────────

    private async void PromoBanner_Tapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(
            $"{nameof(HouseDetailsPage)}?houseId=vene");

    // ── Categories ────────────────────────────────────────────

    private void CatAll_Tapped(object sender, TappedEventArgs e)
    {
        SetActiveCategory(CatAll);
        HousesView.ItemsSource = _allHouses.Where(h => h.HouseId != "soome").ToList();
    }

    private void CatSaun_Tapped(object sender, TappedEventArgs e)
    {
        SetActiveCategory(CatSaun);
        FilterHouses("saun");
    }

    private void CatBassein_Tapped(object sender, TappedEventArgs e)
    {
        SetActiveCategory(CatBassein);
        FilterHouses("bassein");
    }

    private void CatKaraoke_Tapped(object sender, TappedEventArgs e)
    {
        SetActiveCategory(CatKaraoke);
        FilterHouses("karaoke");
    }

    private void CatSuur_Tapped(object sender, TappedEventArgs e)
    {
        SetActiveCategory(CatSuur);
        HousesView.ItemsSource = _allHouses
            .Where(h => h.MaxGuests >= 30 && h.HouseId != "soome").ToList();
    }

    private void FilterHouses(string keyword)
    {
        HousesView.ItemsSource = _allHouses
            .Where(h => h.HouseId != "soome" &&
                        h.AmenitiesText.ToLower().Contains(keyword))
            .ToList();
    }

    private void SetActiveCategory(Frame active)
    {
        var all = new[] { CatAll, CatSaun, CatBassein, CatKaraoke, CatSuur };
        foreach (var cat in all)
        {
            var isActive = cat == active;
            cat.BackgroundColor = isActive
                ? Color.FromArgb("#5A7C5E")
                : Color.FromArgb("#E8EDE7");

            if (cat.Content is HorizontalStackLayout hsl)
                foreach (var child in hsl.Children)
                    if (child is Label lbl && lbl.FontSize <= 14)
                        lbl.TextColor = isActive ? Colors.White : Color.FromArgb("#2D3B2F");
        }
    }
}

public class HomeHouseDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image { get; }
    public string GuestsText { get; }
    public string AmenitiesText { get; }
    public string ViewButtonText { get; }
    public int MaxGuests { get; }

    public HomeHouseDisplay(House house, string lang, SessionService session)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        DisplayPrice = $"€{house.Price24h}";
        Image = house.Image;
        MaxGuests = house.MaxGuests;
        AmenitiesText = string.Join(" ", house.GetAmenities(lang));
        ViewButtonText = session.L("Home_ViewMore");

        GuestsText = lang switch
        {
            "ru" => $"до {house.MaxGuests} гостей",
            "en" => $"up to {house.MaxGuests} guests",
            "fi" => $"enintään {house.MaxGuests} vierasta",
            _ => $"kuni {house.MaxGuests} külalist"
        };
    }
}
