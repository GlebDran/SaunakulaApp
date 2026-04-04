using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class HouseDetailsPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;
    private House? _house;

    public string HouseId { get; set; } = "";

    public HouseDetailsPage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _house = await _houseService.GetByIdAsync(HouseId);
        if (_house is null) return;

        var lang = _session.Language;

        // Основные поля
        TitleLabel.Text = _house.GetTitle(lang);
        DescLabel.Text = _house.GetDescription(lang);
        GuestsLabel.Text = $"{_house.MaxGuests} külast.";
        SizeLabel.Text = $"{_house.SizeM2} m²";
        MinHoursLabel.Text = $"{_house.MinHours}h";
        HourlyPriceLabel.Text = $"€{_house.PricePerHour}/h";
        DayPriceLabel.Text = $"€{_house.Price24h}";
        BottomPriceLabel.Text = $"€{_house.Price24h}";

        // Фото
        MainImage.Source = _house.Image;

        // Amenities
        AmenitiesView.ItemsSource = _house.Amenities;
    }

    private void Back_Tapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("..");

    private async void Book_Clicked(object sender, EventArgs e)
    {
        if (_session.IsLoggedIn)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(BookingPage)}?houseId={_house?.Id}");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}