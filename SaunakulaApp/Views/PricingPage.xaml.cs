using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class PricingPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;

    public PricingPage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        PricingList.ItemsSource = houses.Select(h => new PricingDisplay(h, lang)).ToList();
    }

    private async void Details_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string houseId)
            await Shell.Current.GoToAsync(
                $"{nameof(HouseDetailsPage)}?houseId={houseId}");
    }

    private async void Book_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string houseId)
        {
            if (_session.IsLoggedIn)
                await Shell.Current.GoToAsync(
                    $"{nameof(BookingPage)}?houseId={houseId}");
            else
                await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}

public class PricingDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string Image { get; }
    public string GuestsText { get; }
    public string HourlyText { get; }
    public string DayPriceText { get; }
    public string RegularPriceText { get; }
    public string MinHoursText { get; }

    public PricingDisplay(Models.House house, string lang)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        Image = house.Image;
        GuestsText = $"Kuni {house.MaxGuests} külalist · {house.SizeM2} m²";
        HourlyText = $"€{house.PricePerHour} / tund";
        DayPriceText = $"€{house.Price24h}";
        RegularPriceText = $"€{house.Price24hRegular}";
        MinHoursText = $"Minimaalne broneerimisaeg {house.MinHours}h";
    }
}