using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.ViewModels;

public class BookingViewModel : BaseViewModel
{
    private readonly HouseService _houseService;
    private readonly SupabaseService _supabaseService;
    private readonly DatabaseService _databaseService;
    private readonly SessionService _sessionService;
    private readonly NotificationService _notificationService;

    private House? _house;
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(1);
    private int _guestCount = 2;
    private bool _isVip;
    private string _notes = string.Empty;
    private string _statusMessage = string.Empty;
    private decimal _houseTotal;
    private decimal _addonsTotal;
    private string _nightsText = string.Empty;
    private string _subtotalText = "€0";
    private string _addonsTotalText = "€0";
    private string _totalText = "€0";

    public ObservableCollection<BookingAddonItem> Addons { get; } = new();
    public ObservableCollection<SupabaseReservation> MyReservations { get; } = new();

    public IAsyncRelayCommand ConfirmBookingCommand { get; }
    public IRelayCommand IncreaseGuestsCommand { get; }
    public IRelayCommand DecreaseGuestsCommand { get; }
    public IRelayCommand<BookingAddonItem> AddAddonCommand { get; }
    public IRelayCommand<BookingAddonItem> RemoveAddonCommand { get; }
    public IAsyncRelayCommand BackCommand { get; }

    public BookingViewModel(
        HouseService houseService,
        SupabaseService supabaseService,
        DatabaseService databaseService,
        SessionService sessionService,
        NotificationService notificationService)
    {
        _houseService = houseService;
        _supabaseService = supabaseService;
        _databaseService = databaseService;
        _sessionService = sessionService;
        _notificationService = notificationService;

        ConfirmBookingCommand = new AsyncRelayCommand(ConfirmBookingAsync);
        IncreaseGuestsCommand = new RelayCommand(IncreaseGuests);
        DecreaseGuestsCommand = new RelayCommand(DecreaseGuests);
        AddAddonCommand = new RelayCommand<BookingAddonItem>(AddAddon);
        RemoveAddonCommand = new RelayCommand<BookingAddonItem>(RemoveAddon);
        BackCommand = new AsyncRelayCommand(GoBackAsync);

        LoadDefaultAddons();
    }

    public string BookingTitleText => _sessionService.L("Booking_Title");
    public string TripText => _sessionService.L("Booking_Trip");
    public string CheckInText => _sessionService.L("Booking_CheckIn");
    public string CheckOutText => _sessionService.L("Booking_CheckOut");
    public string GuestsHeaderText => _sessionService.L("Booking_Guests");
    public string NotesText => _sessionService.L("Booking_Notes");
    public string NotesPlaceholderText => _sessionService.L("Booking_NotesPH");
    public string AddonsHeaderText => _sessionService.L("Booking_Addons");
    public string PriceDetailsText => _sessionService.L("Booking_PriceDetails");
    public string TotalHeaderText => _sessionService.L("Booking_Total");
    public string ConfirmButtonText => _sessionService.L("Booking_Confirm");
    public string CancellationText => _sessionService.L("Booking_Cancel");

    public string HouseTitle => _house?.GetTitle(_sessionService.Language) ?? string.Empty;
    public string HouseImage => _house?.Image ?? string.Empty;
    public string HousePriceText => _house is null ? string.Empty : $"€{_house.PricePerHour}/h  |  €{_house.Price24h}/24h";
    public string GuestsText => $"{GuestCount} {GuestsHeaderText.ToLower()}";
    public string VipBannerText => _sessionService.Language switch
    {
        "ru" => "VIP: Веники бесплатно!",
        "en" => "VIP: Free whisks included!",
        "fi" => "VIP: Vihdat ilmaiseksi!",
        _ => "VIP: Viht tasuta!"
    };

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (!SetProperty(ref _startDate, value.Date))
                return;

            if (_endDate <= _startDate)
                EndDate = _startDate.AddDays(1);

            UpdatePrice();
        }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            var nextValue = value.Date <= _startDate ? _startDate.AddDays(1) : value.Date;
            if (SetProperty(ref _endDate, nextValue))
                UpdatePrice();
        }
    }

    public int GuestCount
    {
        get => _guestCount;
        private set
        {
            if (SetProperty(ref _guestCount, value))
                OnPropertyChanged(nameof(GuestsText));
        }
    }

    public bool IsVip
    {
        get => _isVip;
        private set
        {
            if (SetProperty(ref _isVip, value))
                OnPropertyChanged(nameof(VipBannerText));
        }
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (SetProperty(ref _statusMessage, value))
                OnPropertyChanged(nameof(HasStatusMessage));
        }
    }

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public string NightsText
    {
        get => _nightsText;
        private set => SetProperty(ref _nightsText, value);
    }

    public string SubtotalText
    {
        get => _subtotalText;
        private set => SetProperty(ref _subtotalText, value);
    }

    public string AddonsTotalText
    {
        get => _addonsTotalText;
        private set => SetProperty(ref _addonsTotalText, value);
    }

    public string TotalText
    {
        get => _totalText;
        private set => SetProperty(ref _totalText, value);
    }

    public bool CanConfirm => _house is not null && _houseTotal > 0 && !IsBusy;

    public async Task InitializeAsync(string houseId)
    {
        if (string.IsNullOrWhiteSpace(houseId))
            return;

        try
        {
            IsBusy = true;
            StatusMessage = string.Empty;

            await _databaseService.InitAsync();
            _house = await _houseService.GetByIdAsync(houseId);

            if (_house is null)
            {
                StatusMessage = "House could not be loaded from central database.";
                return;
            }

            if (_sessionService.IsLoggedIn)
            {
                var (_, isVip) = await _databaseService.CheckAndUpdateVipAsync(_sessionService.CurrentUser!.Id);
                IsVip = isVip;
            }

            foreach (var addon in Addons)
            {
                addon.SetLanguage(_sessionService.Language);
                addon.SetVip(IsVip);
            }

            OnPropertyChanged(nameof(HouseTitle));
            OnPropertyChanged(nameof(HouseImage));
            OnPropertyChanged(nameof(HousePriceText));
            OnPropertyChanged(nameof(GuestsText));
            UpdatePrice();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Booking page could not be initialized: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(CanConfirm));
        }
    }

    private void IncreaseGuests()
    {
        if (_house is null)
            return;

        if (GuestCount < _house.MaxGuests)
            GuestCount++;
    }

    private void DecreaseGuests()
    {
        if (GuestCount > 1)
            GuestCount--;
    }

    private void AddAddon(BookingAddonItem? addon)
    {
        if (addon is null)
            return;

        addon.Count++;
        UpdatePrice();
    }

    private void RemoveAddon(BookingAddonItem? addon)
    {
        if (addon is null || addon.Count == 0)
            return;

        addon.Count--;
        UpdatePrice();
    }

    private async Task ConfirmBookingAsync()
    {
        if (_house is null)
        {
            StatusMessage = "House is not loaded yet.";
            return;
        }

        if (!_sessionService.IsLoggedIn)
        {
            StatusMessage = "Please log in before booking.";
            return;
        }

        try
        {
            IsBusy = true;
            OnPropertyChanged(nameof(CanConfirm));
            StatusMessage = "Checking central database...";

            var selectedAddons = Addons.Where(addon => addon.Count > 0).ToList();
            var totalAmount = _houseTotal + _addonsTotal;

            var createdInCentralDatabase = await _supabaseService.BookHouseAsync(
                _house.Id,
                _sessionService.CurrentUser!.FullName,
                StartDate,
                EndDate,
                GuestCount,
                totalAmount,
                Notes ?? string.Empty);

            if (!createdInCentralDatabase)
            {
                StatusMessage = "This house is already booked for the selected dates.";
                return;
            }

            var booking = BuildLocalBooking(selectedAddons, totalAmount);
            await _databaseService.InsertBookingAsync(booking);

            var houseTitle = _house.GetTitle(_sessionService.Language);
            await _notificationService.RequestPermissionAsync();
            await _notificationService.SendBookingConfirmedAsync(booking, houseTitle, _sessionService.Language);
            await _notificationService.ScheduleArrivalReminderAsync(booking, houseTitle, _sessionService.Language);

            StatusMessage = "Booking confirmed in central database.";
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Booking failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(CanConfirm));
        }
    }

    private Booking BuildLocalBooking(IReadOnlyList<BookingAddonItem> selectedAddons, decimal totalAmount)
    {
        var addonsJson = JsonSerializer.Serialize(
            selectedAddons.Select(addon => new { addon.Id, addon.Count, Price = addon.EffectivePrice }));

        var addonsDisplay = selectedAddons.Count == 0
            ? string.Empty
            : string.Join(", ", selectedAddons.Select(addon =>
                addon.IsVipFree
                    ? $"{addon.DisplayName} ×{addon.Count} (VIP)"
                    : $"{addon.DisplayName} ×{addon.Count}"));

        return new Booking
        {
            UserId = _sessionService.CurrentUser!.Id,
            HouseId = _house!.Id,
            StartDateTime = StartDate,
            EndDateTime = EndDate,
            GuestCount = GuestCount,
            TotalPrice = totalAmount,
            AddonsJson = addonsJson,
            AddonsTotal = _addonsTotal,
            AddonsDisplay = addonsDisplay,
            Notes = Notes ?? string.Empty,
            Status = "Confirmed",
            CreatedAt = DateTime.Now
        };
    }

    private Task GoBackAsync()
        => Shell.Current.GoToAsync("..");

    private void UpdatePrice()
    {
        if (_house is null)
        {
            _houseTotal = 0;
            _addonsTotal = 0;
            NightsText = string.Empty;
            SubtotalText = "€0";
            AddonsTotalText = "€0";
            TotalText = "€0";
            OnPropertyChanged(nameof(CanConfirm));
            return;
        }

        var hours = (EndDate - StartDate).TotalHours;
        var days = (EndDate - StartDate).TotalDays;

        _houseTotal = days >= 1
            ? (decimal)Math.Ceiling(days) * _house.Price24h
            : (decimal)Math.Ceiling(hours) * _house.PricePerHour;

        _addonsTotal = Addons.Sum(addon => addon.EffectivePrice * addon.Count);

        NightsText = days >= 1
            ? $"€{_house.Price24h} × {(int)Math.Ceiling(days)} ööd"
            : $"€{_house.PricePerHour} × {(int)Math.Ceiling(hours)}h";

        SubtotalText = $"€{_houseTotal:F0}";
        AddonsTotalText = _addonsTotal > 0 ? $"€{_addonsTotal:F0}" : "€0";
        TotalText = $"€{(_houseTotal + _addonsTotal):F0}";
        OnPropertyChanged(nameof(CanConfirm));
    }

    private void LoadDefaultAddons()
    {
        Addons.Clear();
        Addons.Add(new BookingAddonItem("viht_kase", "Kaaseviht", "Birch whisk", "Koivuvihta", "Берёзовый веник", "🌿", 6, true));
        Addons.Add(new BookingAddonItem("viht_tamm", "Tammeviht", "Oak whisk", "Tammivihta", "Дубовый веник", "🍂", 7, true));
        Addons.Add(new BookingAddonItem("lina", "Saunalina", "Sauna towel", "Saunapyyhe", "Полотенце", "🛁", 5, false));
        Addons.Add(new BookingAddonItem("aroom", "Saunaaroom", "Sauna aroma", "Saunaaromi", "Аромат для сауны", "🌸", 7, false));
        Addons.Add(new BookingAddonItem("susi", "Grillsüsi", "Charcoal", "Grillihiili", "Уголь для гриля", "🔥", 7, false));
        Addons.Add(new BookingAddonItem("sytik", "Süütevedelik", "Lighter fluid", "Sytytysaine", "Жидкость для розжига", "💧", 6, false));
        Addons.Add(new BookingAddonItem("tunn", "Kümblustünn", "Hot tub", "Kylpytynnyri", "Купель", "🛁", 140, false));
    }
}

public class BookingAddonItem : ObservableObject
{
    private int _count;
    private bool _isVipFree;
    private string _currentLanguage = "et";

    private readonly bool _freeForVip;
    private readonly string _nameEt;
    private readonly string _nameEn;
    private readonly string _nameFi;
    private readonly string _nameRu;

    public BookingAddonItem(
        string id,
        string nameEt,
        string nameEn,
        string nameFi,
        string nameRu,
        string icon,
        decimal price,
        bool freeForVip)
    {
        Id = id;
        _nameEt = nameEt;
        _nameEn = nameEn;
        _nameFi = nameFi;
        _nameRu = nameRu;
        Icon = icon;
        Price = price;
        _freeForVip = freeForVip;
    }

    public string Id { get; }
    public string Icon { get; }
    public decimal Price { get; }

    public int Count
    {
        get => _count;
        set
        {
            if (!SetProperty(ref _count, value))
                return;

            OnPropertyChanged(nameof(CountText));
            OnPropertyChanged(nameof(DisplayPrice));
            OnPropertyChanged(nameof(CanRemove));
        }
    }

    public bool IsVipFree
    {
        get => _isVipFree;
        private set
        {
            if (!SetProperty(ref _isVipFree, value))
                return;

            OnPropertyChanged(nameof(EffectivePrice));
            OnPropertyChanged(nameof(DisplayPrice));
        }
    }

    public decimal EffectivePrice => IsVipFree ? 0 : Price;
    public bool CanRemove => Count > 0;
    public string CountText => Count.ToString();

    public string DisplayName => _currentLanguage switch
    {
        "ru" => _nameRu,
        "en" => _nameEn,
        "fi" => _nameFi,
        _ => _nameEt
    };

    public string DisplayPrice
    {
        get
        {
            if (IsVipFree)
                return Count > 1 ? $"€0 × {Count} (VIP)" : "€0 (VIP)";

            return Count > 1 ? $"€{Price} × {Count} = €{Price * Count}" : $"€{Price}";
        }
    }

    public void SetLanguage(string language)
    {
        _currentLanguage = language;
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(DisplayPrice));
    }

    public void SetVip(bool isVip)
    {
        IsVipFree = isVip && _freeForVip;
    }
}
