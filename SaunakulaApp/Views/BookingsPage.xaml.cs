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

    public BookingsPage(DatabaseService db, HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _db = db;
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();

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

    private void ApplyLocalization()
    {
        PageTitleLabel.Text = _session.L("Bookings_Title");
        PageSubtitleLabel.Text = _session.L("Bookings_Subtitle");
        UpcomingTabLabel.Text = _session.L("Bookings_Upcoming");
        PastTabLabel.Text = _session.L("Bookings_Past");
        EmptyTitleLabel.Text = _session.L("Bookings_Empty");
        EmptySubLabel.Text = _session.L("Bookings_EmptySub");
        BrowseButton.Text = _session.L("Bookings_Browse");
        NotLoggedInLabel.Text = _session.L("Bookings_Login");
        LoginButton.Text = _session.L("Profile_Login");
    }

    private async Task LoadBookings()
    {
        await _db.InitAsync();
        var bookings = await _db.GetBookingsByUserAsync(_session.CurrentUser!.Id);
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        _allBookings = bookings.Select(b =>
        {
            var house = houses.FirstOrDefault(h => h.Id == b.HouseId);
            return new BookingDisplay(b, house, lang, _session);
        }).OrderByDescending(b => b.Booking.StartDateTime).ToList();

        ShowTab(_showUpcoming);
    }

    private void ShowTab(bool upcoming)
    {
        _showUpcoming = upcoming;

        UpcomingTab.BackgroundColor = upcoming ? Color.FromArgb("#FFFFFF") : Colors.Transparent;
        UpcomingTab.HasShadow = upcoming;
        PastTab.BackgroundColor = !upcoming ? Color.FromArgb("#FFFFFF") : Colors.Transparent;
        PastTab.HasShadow = !upcoming;

        var now = DateTime.Now;
        var filtered = upcoming
            ? _allBookings.Where(b => b.Booking.StartDateTime >= now && b.Booking.Status != "Cancelled").ToList()
            : _allBookings.Where(b => b.Booking.StartDateTime < now || b.Booking.Status == "Cancelled").ToList();

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

    private void UpcomingTab_Tapped(object sender, TappedEventArgs e) => ShowTab(true);
    private void PastTab_Tapped(object sender, TappedEventArgs e) => ShowTab(false);

    private async void Cancel_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not int bookingId) return;

        string q = _session.Language switch
        {
            "ru" => "Вы уверены, что хотите отменить бронирование?",
            "en" => "Are you sure you want to cancel this booking?",
            "fi" => "Haluatko varmasti peruuttaa varauksen?",
            _ => "Kas oled kindel, et soovid broneeringu tühistada?"
        };

        if (!await DisplayAlert(_session.L("Bookings_Cancel"), q,
            _session.L("Common_Yes"), _session.L("Common_No"))) return;

        await _db.CancelBookingAsync(bookingId);
        await LoadBookings();
    }

    private async void Login_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(LoginPage));

    private async void Browse_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//HomePage");
}

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
    public string CancelText { get; }
    public Color StatusColor { get; }
    public bool CanCancel { get; }

    public BookingDisplay(Booking booking, House? house, string lang, SessionService session)
    {
        Booking = booking;
        HouseTitle = house?.GetTitle(lang) ?? booking.HouseId;
        HouseImage = house?.Image ?? "";
        DateRange = $"{booking.StartDateTime:dd.MM.yy} – {booking.EndDateTime:dd.MM.yy}";

        GuestText = lang switch
        {
            "ru" => $"{booking.GuestCount} гостей",
            "en" => $"{booking.GuestCount} guests",
            "fi" => $"{booking.GuestCount} vierasta",
            _ => $"{booking.GuestCount} külalist"
        };

        TotalText = $"€{booking.TotalPrice:F0}";

        CancelText = session.L("Bookings_Cancel");

        var now = DateTime.Now;
        StatusText = booking.Status switch
        {
            "Cancelled" => session.L("Bookings_Cancelled"),
            _ when booking.StartDateTime > now => session.L("Bookings_Confirmed"),
            _ when booking.EndDateTime < now => session.L("Bookings_Finished"),
            _ => session.L("Bookings_Confirmed")
        };

        StatusColor = booking.Status switch
        {
            "Cancelled" => Color.FromArgb("#C94D4D"),
            _ when booking.StartDateTime > now => Color.FromArgb("#5A7C5E"),
            _ => Color.FromArgb("#7A8A7D")
        };

        CanCancel = booking.Status == "Confirmed" && booking.StartDateTime > now;
    }
}
