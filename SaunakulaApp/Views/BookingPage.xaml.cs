using System.Text.Json;
using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class BookingPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly DatabaseService _db;
    private readonly SessionService _session;
    private readonly NotificationService _notifications;

    private House? _house;
    private int _guests = 2;
    private decimal _houseTotal = 0;

    private readonly List<AddonDisplay> _addons = new()
    {
        new AddonDisplay("viht_kase",  "Kaaseviht",    "Birch whisk",   "Koivuvihta",  "Берёзовый веник",      "🌿", 6),
        new AddonDisplay("viht_tamm",  "Tammeviht",    "Oak whisk",     "Tammivihta",  "Дубовый веник",        "🍂", 7),
        new AddonDisplay("lina",       "Saunalina",    "Sauna towel",   "Saunapyyhe",  "Полотенце",            "🛁", 5),
        new AddonDisplay("aroom",      "Saunaaroom",   "Sauna aroma",   "Saunaaromi",  "Аромат для сауны",     "🌸", 7),
        new AddonDisplay("susi",       "Grillsüsi",    "Charcoal",      "Grillihiili", "Уголь для гриля",      "🔥", 7),
        new AddonDisplay("sytik",      "Süütevedelik", "Lighter fluid", "Sytytysaine", "Жидкость для розжига", "💧", 6),
        new AddonDisplay("tunn",       "Kümblustünn",  "Hot tub",       "Kylpytynnyri","Купель",               "🛁", 140),
    };

    public string HouseId { get; set; } = "";

    public BookingPage(HouseService houseService,
                       DatabaseService db,
                       SessionService session,
                       NotificationService notifications)
    {
        InitializeComponent();
        _houseService = houseService;
        _db = db;
        _session = session;
        _notifications = notifications;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _db.InitAsync();

        _house = await _houseService.GetByIdAsync(HouseId);
        if (_house is null) return;

        var lang = _session.Language;
        ApplyLocalization();

        HouseTitleLabel.Text = _house.GetTitle(lang);
        HouseImage.Source = _house.Image;
        HousePriceLabel.Text = $"€{_house.PricePerHour}/h  |  €{_house.Price24h}/24h";

        CheckInPicker.Date = DateTime.Today;
        CheckOutPicker.Date = DateTime.Today.AddDays(1);

        foreach (var a in _addons) a.SetLang(lang);
        AddonsView.ItemsSource = null;
        AddonsView.ItemsSource = _addons;

        UpdatePrice();
    }

    private void ApplyLocalization()
    {
        BookingTitleLabel.Text = _session.L("Booking_Title");
        TripLabel.Text = _session.L("Booking_Trip");
        CheckInLabel.Text = _session.L("Booking_CheckIn");
        CheckOutLabel.Text = _session.L("Booking_CheckOut");
        GuestsHeaderLabel.Text = _session.L("Booking_Guests");
        NotesLabel.Text = _session.L("Booking_Notes");
        NotesEditor.Placeholder = _session.L("Booking_NotesPH");
        AddonsHeaderLabel.Text = _session.L("Booking_Addons");
        PriceDetailsLabel.Text = _session.L("Booking_PriceDetails");
        TotalHeaderLabel.Text = _session.L("Booking_Total");
        ConfirmButton.Text = _session.L("Booking_Confirm");
        CancellationLabel.Text = _session.L("Booking_Cancel");
    }

    // ── Dates & Guests ────────────────────────────────────────

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (CheckOutPicker.Date <= CheckInPicker.Date)
            CheckOutPicker.Date = CheckInPicker.Date.AddDays(1);
        UpdatePrice();
    }

    private void IncreaseGuests_Tapped(object sender, TappedEventArgs e)
    {
        if (_house is null) return;
        if (_guests < _house.MaxGuests) _guests++;
        GuestsLabel.Text = $"{_guests} {_session.L("Booking_Guests").ToLower()}";
    }

    private void DecreaseGuests_Tapped(object sender, TappedEventArgs e)
    {
        if (_guests > 1) _guests--;
        GuestsLabel.Text = $"{_guests} {_session.L("Booking_Guests").ToLower()}";
    }

    // ── Addons ────────────────────────────────────────────────

    private void AddAddon_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string id) return;
        var addon = _addons.FirstOrDefault(a => a.Id == id);
        if (addon is null) return;
        addon.Count++;
        addon.IsSelected = addon.Count > 0;
        AddonsView.ItemsSource = null;
        AddonsView.ItemsSource = _addons;
        UpdatePrice();
    }

    private void RemoveAddon_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string id) return;
        var addon = _addons.FirstOrDefault(a => a.Id == id);
        if (addon is null) return;
        if (addon.Count > 0) addon.Count--;
        addon.IsSelected = addon.Count > 0;
        AddonsView.ItemsSource = null;
        AddonsView.ItemsSource = _addons;
        UpdatePrice();
    }

    // ── Price calculation ─────────────────────────────────────

    private void UpdatePrice()
    {
        if (_house is null) return;

        var start = CheckInPicker.Date;
        var end = CheckOutPicker.Date;
        var hours = (end - start).TotalHours;
        var days = (end - start).TotalDays;

        _houseTotal = days >= 1
            ? (decimal)Math.Ceiling(days) * _house.Price24h
            : (decimal)Math.Ceiling(hours) * _house.PricePerHour;

        var addonsTotal = _addons.Sum(a => a.Price * a.Count);

        NightsLabel.Text = days >= 1
            ? $"€{_house.Price24h} × {(int)Math.Ceiling(days)} ööd"
            : $"€{_house.PricePerHour} × {(int)Math.Ceiling(hours)}h";

        SubtotalLabel.Text = $"€{_houseTotal:F0}";
        AddonsLabel.Text = addonsTotal > 0 ? $"€{addonsTotal:F0}" : "€0";
        TotalLabel.Text = $"€{(_houseTotal + addonsTotal):F0}";
        ConfirmButton.IsEnabled = _houseTotal > 0;
    }

    // ── Confirm ───────────────────────────────────────────────

    private async void Confirm_Clicked(object sender, EventArgs e)
    {
        if (_house is null || !_session.IsLoggedIn) return;

        var start = CheckInPicker.Date;
        var end = CheckOutPicker.Date;
        var days = (end - start).TotalDays;
        var hours = (end - start).TotalHours;

        decimal houseTotal = days >= 1
            ? (decimal)Math.Ceiling(days) * _house.Price24h
            : (decimal)Math.Ceiling(hours) * _house.PricePerHour;

        var selectedAddons = _addons.Where(a => a.Count > 0).ToList();
        decimal addonsTotal = selectedAddons.Sum(a => a.Price * a.Count);

        var addonsJson = JsonSerializer.Serialize(
            selectedAddons.Select(a => new { a.Id, a.Count, a.Price }));

        var addonsDisplay = selectedAddons.Any()
            ? string.Join(", ", selectedAddons.Select(a => $"{a.DisplayName} ×{a.Count}"))
            : "";

        var booking = new Booking
        {
            UserId = _session.CurrentUser!.Id,
            HouseId = _house.Id,
            StartDateTime = start,
            EndDateTime = end,
            GuestCount = _guests,
            TotalPrice = houseTotal + addonsTotal,
            AddonsJson = addonsJson,
            AddonsTotal = addonsTotal,
            AddonsDisplay = addonsDisplay,
            Notes = NotesEditor.Text ?? "",
            Status = "Confirmed",
            CreatedAt = DateTime.Now
        };

        await _db.InsertBookingAsync(booking);

        var lang = _session.Language;
        var houseTitle = _house.GetTitle(lang);

        await _notifications.RequestPermissionAsync();
        await _notifications.SendBookingConfirmedAsync(booking, houseTitle, lang);
        await _notifications.ScheduleArrivalReminderAsync(booking, houseTitle, lang);

        var addonsSummary = !string.IsNullOrEmpty(addonsDisplay)
            ? $"\n{_session.L("Booking_Addons_label")}: {addonsDisplay}"
            : "";

        await DisplayAlert(
            $"✅ {_session.L("Booking_Confirm")}",
            $"{houseTitle}\n{start:dd.MM.yyyy} – {end:dd.MM.yyyy}\n" +
            $"{_session.L("Booking_Guests")}: {_guests}" +
            addonsSummary +
            $"\n{_session.L("Booking_Total")}: €{houseTotal + addonsTotal:F0}",
            _session.L("Common_OK"));

        await Shell.Current.GoToAsync("//HomePage");
    }

    private void Back_Tapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("..");
}

public class AddonDisplay
{
    public string Id { get; }
    public decimal Price { get; }
    public string Icon { get; }
    public int Count { get; set; } = 0;
    public bool IsSelected { get; set; } = false;

    private readonly string _nameEt, _nameEn, _nameRu, _nameFi;
    private string _currentLang = "et";

    public string DisplayName => _currentLang switch
    {
        "ru" => _nameRu,
        "en" => _nameEn,
        "fi" => _nameFi,
        _ => _nameEt
    };

    public string DisplayPrice => Count > 1
        ? $"€{Price} × {Count} = €{Price * Count}"
        : $"€{Price}";

    public string CountText => Count > 0 ? Count.ToString() : "0";

    public AddonDisplay(string id, string et, string en, string fi,
                        string ru, string icon, decimal price)
    {
        Id = id;
        _nameEt = et; _nameEn = en; _nameFi = fi; _nameRu = ru;
        Icon = icon;
        Price = price;
    }

    public void SetLang(string lang) => _currentLang = lang;
}
