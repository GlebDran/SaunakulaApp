using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class HomePage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;
    private List<HomeHouseDisplay> _allHouses = new();
    private House? _featuredHouse;

    // Категории — amenities ключевые слова для фильтрации
    private static readonly Dictionary<string, string> CategoryKeywords = new()
    {
        { "saun", "saun" },
        { "bassein", "bassein" },
        { "karaoke", "karaoke" },
        { "suur", "" } // фильтр по maxGuests >= 30
    };

    public HomePage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
        UpdateGreeting();
    }

    private async Task LoadData()
    {
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        _allHouses = houses
            .Select(h => new HomeHouseDisplay(h, lang))
            .ToList();

        // Featured — Soome maja как "Külaliste lemmik"
        _featuredHouse = houses.FirstOrDefault(h => h.Id == "soome")
                         ?? houses.FirstOrDefault();

        if (_featuredHouse != null)
        {
            FeaturedImage.Source = _featuredHouse.Image;
            FeaturedTitle.Text = _featuredHouse.GetTitle(lang);
            FeaturedPrice.Text = $"€{_featuredHouse.Price24h}";
            FeaturedGuests.Text = $"kuni {_featuredHouse.MaxGuests}";
        }

        // Список — все кроме featured
        HousesView.ItemsSource = _allHouses
            .Where(h => h.HouseId != "soome")
            .ToList();
    }

    private void UpdateGreeting()
    {
        if (_session.IsLoggedIn)
        {
            var name = _session.CurrentUser!.FullName.Split(' ')[0];
            GreetingLabel.Text = $"Tere, {name}! 👋";
            AvatarLabel.Text = name[..1].ToUpper();
        }
        else
        {
            GreetingLabel.Text = "Tere tulemast! 👋";
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
        HousesView.ItemsSource = _allHouses
            .Where(h => h.HouseId != "soome").ToList();
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
        // Большая группа — дома с maxGuests >= 30
        HousesView.ItemsSource = _allHouses
            .Where(h => h.MaxGuests >= 30 && h.HouseId != "soome")
            .ToList();
    }

    private void FilterHouses(string keyword)
    {
        var lang = _session.Language;
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
            {
                foreach (var child in hsl.Children)
                {
                    if (child is Label lbl && lbl.FontSize <= 14)
                        lbl.TextColor = isActive
                            ? Colors.White
                            : Color.FromArgb("#2D3B2F");
                }
            }
        }
    }
}

// ── Display model ─────────────────────────────────────────────
public class HomeHouseDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image { get; }
    public string GuestsText { get; }
    public string AmenitiesText { get; }
    public int MaxGuests { get; }

    public HomeHouseDisplay(House house, string lang)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        DisplayPrice = $"€{house.Price24h}";
        Image = house.Image;
        MaxGuests = house.MaxGuests;
        GuestsText = $"kuni {house.MaxGuests} külalist";
        AmenitiesText = string.Join(" ", house.GetAmenities(lang));
    }
}
