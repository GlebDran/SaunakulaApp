using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class HomePage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;
    private List<HouseDisplay> _allDisplayed = new();

    public HomePage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHouses();
    }

    private async Task LoadHouses()
    {
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        _allDisplayed = houses
            .Select(h => new HouseDisplay(h, lang))
            .ToList();

        FeaturedView.ItemsSource = _allDisplayed.Take(2).ToList();
        AllHousesView.ItemsSource = _allDisplayed;
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var q = e.NewTextValue?.ToLower() ?? "";
        AllHousesView.ItemsSource = string.IsNullOrWhiteSpace(q)
            ? _allDisplayed
            : _allDisplayed.Where(h =>
                h.DisplayTitle.ToLower().Contains(q)).ToList();
    }

    private async void House_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not HouseDisplay hd) return;
        AllHousesView.SelectedItem = null;
        await Shell.Current.GoToAsync(
            $"{nameof(HouseDetailsPage)}?houseId={hd.House.Id}");
    }

    private async void PromoBanner_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(HouseDetailsPage)}?houseId=vene");
    }
}

public class HouseDisplay
{
    public House House { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image => House.Image;

    public HouseDisplay(House house, string lang)
    {
        House = house;
        DisplayTitle = house.GetTitle(lang);
        DisplayPrice = $"€{house.Price24h}";
    }
}