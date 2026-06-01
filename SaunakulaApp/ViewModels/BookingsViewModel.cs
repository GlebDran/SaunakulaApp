using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SaunakulaApp.Models;
using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp.ViewModels;

public class BookingsViewModel : BaseViewModel
{
    private static readonly Color ActiveTabBackground = Colors.White;
    private static readonly Color InactiveTabBackground = Colors.Transparent;
    private static readonly Color ActiveTabText = Color.FromArgb("#5A7C5E");
    private static readonly Color InactiveTabText = Color.FromArgb("#7A8A7D");

    private readonly DatabaseService _databaseService;
    private readonly HouseService _houseService;
    private readonly SessionService _sessionService;
    private readonly List<BookingDisplay> _allBookings = new();

    private bool _showUpcoming = true;
    private bool _isNotLoggedInVisible;
    private bool _isEmptyVisible;
    private bool _isBookingsVisible;
    private string _pageTitleText = string.Empty;
    private string _pageSubtitleText = string.Empty;
    private string _upcomingTabText = string.Empty;
    private string _pastTabText = string.Empty;
    private string _notLoggedInText = string.Empty;
    private string _loginText = string.Empty;
    private string _emptyTitleText = string.Empty;
    private string _emptySubtitleText = string.Empty;
    private string _browseText = string.Empty;
    private Color _upcomingTabBackground = ActiveTabBackground;
    private Color _pastTabBackground = InactiveTabBackground;
    private Color _upcomingTabTextColor = ActiveTabText;
    private Color _pastTabTextColor = InactiveTabText;
    private bool _upcomingTabHasShadow = true;
    private bool _pastTabHasShadow;

    public ObservableCollection<BookingDisplay> Bookings { get; } = new();

    public IAsyncRelayCommand InitializeCommand { get; }
    public IRelayCommand ShowUpcomingCommand { get; }
    public IRelayCommand ShowPastCommand { get; }
    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand BrowseCommand { get; }
    public IAsyncRelayCommand<int> CancelBookingCommand { get; }

    public BookingsViewModel(
        DatabaseService databaseService,
        HouseService houseService,
        SessionService sessionService)
    {
        _databaseService = databaseService;
        _houseService = houseService;
        _sessionService = sessionService;

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        ShowUpcomingCommand = new RelayCommand(() => ShowTab(true));
        ShowPastCommand = new RelayCommand(() => ShowTab(false));
        LoginCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(LoginPage)));
        BrowseCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync("//HomePage"));
        CancelBookingCommand = new AsyncRelayCommand<int>(CancelBookingAsync);
    }

    public bool IsNotLoggedInVisible
    {
        get => _isNotLoggedInVisible;
        private set => SetProperty(ref _isNotLoggedInVisible, value);
    }

    public bool IsEmptyVisible
    {
        get => _isEmptyVisible;
        private set => SetProperty(ref _isEmptyVisible, value);
    }

    public bool IsBookingsVisible
    {
        get => _isBookingsVisible;
        private set => SetProperty(ref _isBookingsVisible, value);
    }

    public string PageTitleText
    {
        get => _pageTitleText;
        private set => SetProperty(ref _pageTitleText, value);
    }

    public string PageSubtitleText
    {
        get => _pageSubtitleText;
        private set => SetProperty(ref _pageSubtitleText, value);
    }

    public string UpcomingTabText
    {
        get => _upcomingTabText;
        private set => SetProperty(ref _upcomingTabText, value);
    }

    public string PastTabText
    {
        get => _pastTabText;
        private set => SetProperty(ref _pastTabText, value);
    }

    public string NotLoggedInText
    {
        get => _notLoggedInText;
        private set => SetProperty(ref _notLoggedInText, value);
    }

    public string LoginText
    {
        get => _loginText;
        private set => SetProperty(ref _loginText, value);
    }

    public string EmptyTitleText
    {
        get => _emptyTitleText;
        private set => SetProperty(ref _emptyTitleText, value);
    }

    public string EmptySubtitleText
    {
        get => _emptySubtitleText;
        private set => SetProperty(ref _emptySubtitleText, value);
    }

    public string BrowseText
    {
        get => _browseText;
        private set => SetProperty(ref _browseText, value);
    }

    public Color UpcomingTabBackground
    {
        get => _upcomingTabBackground;
        private set => SetProperty(ref _upcomingTabBackground, value);
    }

    public Color PastTabBackground
    {
        get => _pastTabBackground;
        private set => SetProperty(ref _pastTabBackground, value);
    }

    public Color UpcomingTabTextColor
    {
        get => _upcomingTabTextColor;
        private set => SetProperty(ref _upcomingTabTextColor, value);
    }

    public Color PastTabTextColor
    {
        get => _pastTabTextColor;
        private set => SetProperty(ref _pastTabTextColor, value);
    }

    public bool UpcomingTabHasShadow
    {
        get => _upcomingTabHasShadow;
        private set => SetProperty(ref _upcomingTabHasShadow, value);
    }

    public bool PastTabHasShadow
    {
        get => _pastTabHasShadow;
        private set => SetProperty(ref _pastTabHasShadow, value);
    }

    public async Task InitializeAsync()
    {
        ApplyLocalization();

        if (!_sessionService.IsLoggedIn)
        {
            IsNotLoggedInVisible = true;
            IsBookingsVisible = false;
            IsEmptyVisible = false;
            Bookings.Clear();
            return;
        }

        IsNotLoggedInVisible = false;
        await LoadBookingsAsync();
    }

    private void ApplyLocalization()
    {
        PageTitleText = _sessionService.L("Bookings_Title");
        PageSubtitleText = _sessionService.L("Bookings_Subtitle");
        UpcomingTabText = _sessionService.L("Bookings_Upcoming");
        PastTabText = _sessionService.L("Bookings_Past");
        EmptyTitleText = _sessionService.L("Bookings_Empty");
        EmptySubtitleText = _sessionService.L("Bookings_EmptySub");
        BrowseText = _sessionService.L("Bookings_Browse");
        NotLoggedInText = _sessionService.L("Bookings_Login");
        LoginText = _sessionService.L("Profile_Login");
    }

    private async Task LoadBookingsAsync()
    {
        try
        {
            IsBusy = true;

            await _databaseService.InitAsync();
            var bookings = await _databaseService.GetBookingsByUserAsync(_sessionService.CurrentUser!.Id);
            var houses = await _houseService.GetAllAsync();
            var language = _sessionService.Language;

            _allBookings.Clear();
            _allBookings.AddRange(bookings
                .Select(booking =>
                {
                    var house = houses.FirstOrDefault(item => item.Id == booking.HouseId);
                    return new BookingDisplay(booking, house, language, _sessionService);
                })
                .OrderByDescending(item => item.Booking.StartDateTime));

            ShowTab(_showUpcoming);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowTab(bool upcoming)
    {
        _showUpcoming = upcoming;
        UpdateTabStyles();

        var now = DateTime.Now;
        var filtered = upcoming
            ? _allBookings.Where(item => item.Booking.StartDateTime >= now && item.Booking.Status != "Cancelled").ToList()
            : _allBookings.Where(item => item.Booking.StartDateTime < now || item.Booking.Status == "Cancelled").ToList();

        Bookings.Clear();
        foreach (var booking in filtered)
            Bookings.Add(booking);

        IsBookingsVisible = filtered.Count > 0;
        IsEmptyVisible = filtered.Count == 0 && !IsNotLoggedInVisible;
    }

    private void UpdateTabStyles()
    {
        UpcomingTabBackground = _showUpcoming ? ActiveTabBackground : InactiveTabBackground;
        PastTabBackground = !_showUpcoming ? ActiveTabBackground : InactiveTabBackground;
        UpcomingTabTextColor = _showUpcoming ? ActiveTabText : InactiveTabText;
        PastTabTextColor = !_showUpcoming ? ActiveTabText : InactiveTabText;
        UpcomingTabHasShadow = _showUpcoming;
        PastTabHasShadow = !_showUpcoming;
    }

    private async Task CancelBookingAsync(int bookingId)
    {
        if (bookingId <= 0)
            return;

        var question = _sessionService.Language switch
        {
            "ru" => "Вы уверены, что хотите отменить бронирование?",
            "en" => "Are you sure you want to cancel this booking?",
            "fi" => "Haluatko varmasti peruuttaa varauksen?",
            _ => "Kas oled kindel, et soovid broneeringu tühistada?"
        };

        var confirmed = await Shell.Current.DisplayAlert(
            _sessionService.L("Bookings_Cancel"),
            question,
            _sessionService.L("Common_Yes"),
            _sessionService.L("Common_No"));

        if (!confirmed)
            return;

        await _databaseService.CancelBookingAsync(bookingId);
        await LoadBookingsAsync();
    }
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

    public BookingDisplay(Booking booking, House? house, string language, SessionService session)
    {
        Booking = booking;
        HouseTitle = house?.GetTitle(language) ?? booking.HouseId;
        HouseImage = house?.Image ?? string.Empty;
        DateRange = $"{booking.StartDateTime:dd.MM.yy} - {booking.EndDateTime:dd.MM.yy}";

        GuestText = language switch
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
