using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class BookingsPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly HouseService _houseService;
    private readonly SessionService _session;

    private List<BookingDisplay> _allBookings = new();
    private bool _showUpcoming = true;

    public BookingsPage(DatabaseService db,
                        HouseService houseService,
                        SessionService session)
    {
        InitializeComponent();
        _db = db;
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_session.IsLoggedIn)
        {
            NotLoggedInView.IsVisible = true;
            BookingsView.IsVisible = false;
            EmptyView.IsVisible = false;
            return;
        }

        NotLoggedInView.IsVisible = false;
        await LoadBookings();
    }

    private async Task LoadBookings()
    {
        await _db.InitAsync();

        var bookings = await _db.GetBookingsByUserAsync(
            _session.CurrentUser!.Id);

        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        _allBookings = bookings.Select(b =>
        {
            var house = houses.FirstOrDefault(h => h.Id == b.HouseId);
            return new BookingDisplay(b, house, lang);
        }).OrderByDescending(b => b.Booking.StartDateTime).ToList();

        ShowTab(_showUpcoming);
    }

    private void ShowTab(bool upcoming)
    {
        _showUpcoming = upcoming;

        // Tab styles
        UpcomingTab.BackgroundColor = upcoming
            ? Color.FromArgb("#FFFFFF")
            : Colors.Transparent;
        UpcomingTab.HasShadow = upcoming;

        PastTab.BackgroundColor = !upcoming
            ? Color.FromArgb("#FFFFFF")
            : Colors.Transparent;
        PastTab.HasShadow = !upcoming;

        var now = DateTime.Now;
        var filtered = upcoming
            ? _allBookings.Where(b =>
                b.Booking.StartDateTime >= now &&
                b.Booking.Status != "Cancelled").ToList()
            : _allBookings.Where(b =>
                b.Booking.StartDateTime < now ||
                b.Booking.Status == "Cancelled").ToList();

        if (filtered.Count == 0)
        {
            BookingsView.IsVisible = false;
            EmptyView.IsVisible = true;
        }
        else
        {
            EmptyView.IsVisible = false;
            BookingsView.IsVisible = true;
            BookingsView.ItemsSource = filtered;
        }
    }

    private void UpcomingTab_Tapped(object sender, TappedEventArgs e)
        => ShowTab(true);

    private void PastTab_Tapped(object sender, TappedEventArgs e)
        => ShowTab(false);

    private async void Cancel_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not int bookingId) return;

        bool confirm = await DisplayAlert(
            "Tühista broneering",
            "Kas oled kindel, et soovid broneeringu tühistada?",
            "Jah, tühista",
            "Ei");

        if (!confirm) return;

        await _db.CancelBookingAsync(bookingId);
        await LoadBookings();
    }

    private async void Login_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(LoginPage));

    private async void Browse_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//HomePage");
}

// Display model для списка броней
public class BookingDisplay
{
    public Booking Booking { get; }
    public int Id => Booking.Id;
    public string HouseTitle { get; }
    public string HouseImage { get; }
    public string DateRange { get; }
    public string GuestText { get; }
    public string TotalText { get; }
    public string StatusText { get; }
    public Color StatusColor { get; }
    public bool CanCancel { get; }

    public BookingDisplay(Booking booking, House? house, string lang)
    {
        Booking = booking;
        HouseTitle = house?.GetTitle(lang) ?? booking.HouseId;
        HouseImage = house?.Image ?? "";
        DateRange = $"{booking.StartDateTime:dd.MM.yy} – " +
                    $"{booking.EndDateTime:dd.MM.yy}";
        GuestText = $"{booking.GuestCount} külalist";
        TotalText = $"€{booking.TotalPrice:F0}";

        var now = DateTime.Now;
        StatusText = booking.Status switch
        {
            "Cancelled" => "Tühistatud",
            _ when booking.StartDateTime > now => "Kinnitatud",
            _ when booking.EndDateTime < now => "Lõppenud",
            _ => "Käimasolev"
        };

        StatusColor = booking.Status switch
        {
            "Cancelled" => Color.FromArgb("#C94D4D"),
            _ when booking.StartDateTime > now => Color.FromArgb("#5A7C5E"),
            _ => Color.FromArgb("#7A8A7D")
        };

        CanCancel = booking.Status == "Confirmed" &&
                    booking.StartDateTime > now;
    }
}