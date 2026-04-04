using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class BookingPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly DatabaseService _db;
    private readonly SessionService _session;

    private House? _house;
    private int _guests = 2;

    public string HouseId { get; set; } = "";

    public BookingPage(HouseService houseService,
                       DatabaseService db,
                       SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _db = db;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _db.InitAsync();

        _house = await _houseService.GetByIdAsync(HouseId);
        if (_house is null) return;

        var lang = _session.Language;
        HouseTitleLabel.Text = _house.GetTitle(lang);
        HouseImage.Source = _house.Image;
        HousePriceLabel.Text = $"€{_house.PricePerHour}/h  |  €{_house.Price24h}/24h";

        // Начальные даты
        CheckInPicker.Date = DateTime.Today;
        CheckOutPicker.Date = DateTime.Today.AddDays(1);

        UpdatePrice();
    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        // Check-out не может быть раньше check-in
        if (CheckOutPicker.Date <= CheckInPicker.Date)
            CheckOutPicker.Date = CheckInPicker.Date.AddDays(1);

        UpdatePrice();
    }

    private void IncreaseGuests_Tapped(object sender, TappedEventArgs e)
    {
        if (_house is null) return;
        if (_guests < _house.MaxGuests) _guests++;
        GuestsLabel.Text = $"{_guests} külalist";
    }

    private void DecreaseGuests_Tapped(object sender, TappedEventArgs e)
    {
        if (_guests > 1) _guests--;
        GuestsLabel.Text = $"{_guests} külalist";
    }

    private void UpdatePrice()
    {
        if (_house is null) return;

        var start = CheckInPicker.Date;
        var end = CheckOutPicker.Date;
        var hours = (end - start).TotalHours;
        var days = (end - start).TotalDays;

        decimal total;

        if (days >= 1)
        {
            // Считаем по суткам
            total = (decimal)Math.Ceiling(days) * _house.Price24h;
            NightsLabel.Text =
                $"€{_house.Price24h} × {(int)Math.Ceiling(days)} ööd";
        }
        else
        {
            // Считаем по часам
            total = (decimal)Math.Ceiling(hours) * _house.PricePerHour;
            NightsLabel.Text =
                $"€{_house.PricePerHour} × {(int)Math.Ceiling(hours)}h";
        }

        SubtotalLabel.Text = $"€{total:F0}";
        TotalLabel.Text = $"€{total:F0}";
        ConfirmButton.IsEnabled = total > 0;
    }

    private async void Confirm_Clicked(object sender, EventArgs e)
    {
        if (_house is null || !_session.IsLoggedIn) return;

        var start = CheckInPicker.Date;
        var end = CheckOutPicker.Date;
        var days = (end - start).TotalDays;
        var hours = (end - start).TotalHours;

        decimal total = days >= 1
            ? (decimal)Math.Ceiling(days) * _house.Price24h
            : (decimal)Math.Ceiling(hours) * _house.PricePerHour;

        var booking = new Booking
        {
            UserId = _session.CurrentUser!.Id,
            HouseId = _house.Id,
            StartDateTime = start,
            EndDateTime = end,
            GuestCount = _guests,
            TotalPrice = total,
            Notes = NotesEditor.Text ?? "",
            Status = "Confirmed",
            CreatedAt = DateTime.Now
        };

        await _db.InsertBookingAsync(booking);

        await DisplayAlert(
            "✅ Broneering kinnitatud!",
            $"{_house.GetTitle(_session.Language)}\n" +
            $"{start:dd.MM.yyyy} – {end:dd.MM.yyyy}\n" +
            $"Külalisi: {_guests}\n" +
            $"Kokku: €{total:F0}",
            "OK");

        // Возвращаемся на главную
        await Shell.Current.GoToAsync("//HomePage");
    }

    private void Back_Tapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("..");
}