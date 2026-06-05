using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using SaunakulaApp.Models;
using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private static readonly Color ActiveCategoryBackground = Color.FromArgb("#5A7C5E");
    private static readonly Color InactiveCategoryBackground = Color.FromArgb("#E8EDE7");
    private static readonly Color ActiveCategoryText = Colors.White;
    private static readonly Color InactiveCategoryText = Color.FromArgb("#2D3B2F");

    private readonly HouseService _houseService;
    private readonly SessionService _sessionService;
    private readonly List<HomeHouseDisplay> _allHouses = new();

    private House? _featuredHouse;
    private string _activeCategory = "all";
    private string _greetingText = string.Empty;
    private string _avatarText = string.Empty;
    private string _homeTitleText = string.Empty;
    private string _homeSubtitleText = string.Empty;
    private string _activitiesText = string.Empty;
    private string _featuredText = string.Empty;
    private string _allHousesText = string.Empty;
    private string _featuredBadgeText = string.Empty;
    private string _featuredDetailsText = string.Empty;
    private string _featuredBookText = string.Empty;
    private string _findHouseText = string.Empty;
    private string _bannerTitleText = "PäevaSPA -40%";
    private string _bannerSubtitleText = string.Empty;
    private string _catAllText = string.Empty;
    private string _catLargeText = string.Empty;
    private string _featuredImage = string.Empty;
    private string _featuredTitle = string.Empty;
    private string _featuredPrice = string.Empty;
    private string _featuredGuests = string.Empty;

    private Color _catAllBackground = ActiveCategoryBackground;
    private Color _catSaunBackground = InactiveCategoryBackground;
    private Color _catBasseinBackground = InactiveCategoryBackground;
    private Color _catKaraokeBackground = InactiveCategoryBackground;
    private Color _catLargeBackground = InactiveCategoryBackground;
    private Color _catAllTextColor = ActiveCategoryText;
    private Color _catSaunTextColor = InactiveCategoryText;
    private Color _catBasseinTextColor = InactiveCategoryText;
    private Color _catKaraokeTextColor = InactiveCategoryText;
    private Color _catLargeTextColor = InactiveCategoryText;

    public ObservableCollection<HomeHouseDisplay> Houses { get; } = new();

    public IAsyncRelayCommand InitializeCommand { get; }
    public IAsyncRelayCommand GoToProfileCommand { get; }
    public IAsyncRelayCommand GoToHouseFinderCommand { get; }
    public IAsyncRelayCommand GoToFeaturedDetailsCommand { get; }
    public IAsyncRelayCommand GoToFeaturedBookingCommand { get; }
    public IAsyncRelayCommand GoToPromoCommand { get; }
    public IAsyncRelayCommand<string> OpenHouseDetailsCommand { get; }
    public IRelayCommand<string> SelectCategoryCommand { get; }

    public HomeViewModel(HouseService houseService, SessionService sessionService)
    {
        _houseService = houseService;
        _sessionService = sessionService;

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        GoToProfileCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync("//ProfilePage"));
        GoToHouseFinderCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(HouseFinderPage)));
        GoToFeaturedDetailsCommand = new AsyncRelayCommand(GoToFeaturedDetailsAsync);
        GoToFeaturedBookingCommand = new AsyncRelayCommand(GoToFeaturedBookingAsync);
        GoToPromoCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId=vene"));
        OpenHouseDetailsCommand = new AsyncRelayCommand<string>(OpenHouseDetailsAsync);
        SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
    }

    public string GreetingText
    {
        get => _greetingText;
        private set => SetProperty(ref _greetingText, value);
    }

    public string AvatarText
    {
        get => _avatarText;
        private set => SetProperty(ref _avatarText, value);
    }

    public string HomeTitleText
    {
        get => _homeTitleText;
        private set => SetProperty(ref _homeTitleText, value);
    }

    public string HomeSubtitleText
    {
        get => _homeSubtitleText;
        private set => SetProperty(ref _homeSubtitleText, value);
    }

    public string ActivitiesText
    {
        get => _activitiesText;
        private set => SetProperty(ref _activitiesText, value);
    }

    public string FeaturedText
    {
        get => _featuredText;
        private set => SetProperty(ref _featuredText, value);
    }

    public string AllHousesText
    {
        get => _allHousesText;
        private set => SetProperty(ref _allHousesText, value);
    }

    public string FeaturedBadgeText
    {
        get => _featuredBadgeText;
        private set => SetProperty(ref _featuredBadgeText, value);
    }

    public string FeaturedDetailsText
    {
        get => _featuredDetailsText;
        private set => SetProperty(ref _featuredDetailsText, value);
    }

    public string FeaturedBookText
    {
        get => _featuredBookText;
        private set => SetProperty(ref _featuredBookText, value);
    }

    public string FindHouseText
    {
        get => _findHouseText;
        private set => SetProperty(ref _findHouseText, value);
    }

    public string BannerTitleText
    {
        get => _bannerTitleText;
        private set => SetProperty(ref _bannerTitleText, value);
    }

    public string BannerSubtitleText
    {
        get => _bannerSubtitleText;
        private set => SetProperty(ref _bannerSubtitleText, value);
    }

    public string CatAllText
    {
        get => _catAllText;
        private set => SetProperty(ref _catAllText, value);
    }

    public string CatLargeText
    {
        get => _catLargeText;
        private set => SetProperty(ref _catLargeText, value);
    }

    public string FeaturedImage
    {
        get => _featuredImage;
        private set => SetProperty(ref _featuredImage, value);
    }

    public string FeaturedTitle
    {
        get => _featuredTitle;
        private set => SetProperty(ref _featuredTitle, value);
    }

    public string FeaturedPrice
    {
        get => _featuredPrice;
        private set => SetProperty(ref _featuredPrice, value);
    }

    public string FeaturedGuests
    {
        get => _featuredGuests;
        private set => SetProperty(ref _featuredGuests, value);
    }

    public Color CatAllBackground
    {
        get => _catAllBackground;
        private set => SetProperty(ref _catAllBackground, value);
    }

    public Color CatSaunBackground
    {
        get => _catSaunBackground;
        private set => SetProperty(ref _catSaunBackground, value);
    }

    public Color CatBasseinBackground
    {
        get => _catBasseinBackground;
        private set => SetProperty(ref _catBasseinBackground, value);
    }

    public Color CatKaraokeBackground
    {
        get => _catKaraokeBackground;
        private set => SetProperty(ref _catKaraokeBackground, value);
    }

    public Color CatLargeBackground
    {
        get => _catLargeBackground;
        private set => SetProperty(ref _catLargeBackground, value);
    }

    public Color CatAllTextColor
    {
        get => _catAllTextColor;
        private set => SetProperty(ref _catAllTextColor, value);
    }

    public Color CatSaunTextColor
    {
        get => _catSaunTextColor;
        private set => SetProperty(ref _catSaunTextColor, value);
    }

    public Color CatBasseinTextColor
    {
        get => _catBasseinTextColor;
        private set => SetProperty(ref _catBasseinTextColor, value);
    }

    public Color CatKaraokeTextColor
    {
        get => _catKaraokeTextColor;
        private set => SetProperty(ref _catKaraokeTextColor, value);
    }

    public Color CatLargeTextColor
    {
        get => _catLargeTextColor;
        private set => SetProperty(ref _catLargeTextColor, value);
    }

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            ApplyLocalization();
            UpdateGreeting();

            var houses = await _houseService.GetAllAsync();
            var lang = _sessionService.Language;

            _allHouses.Clear();
            _allHouses.AddRange(houses.Select(house => new HomeHouseDisplay(house, lang, _sessionService)));

            _featuredHouse = houses.FirstOrDefault(house => house.Id == "soome")
                             ?? houses.FirstOrDefault();

            UpdateFeaturedHouse();
            ApplyCategoryFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyLocalization()
    {
        var lang = _sessionService.Language;

        HomeSubtitleText = _sessionService.L("Home_Subtitle");
        HomeTitleText = _sessionService.L("Home_Title");
        ActivitiesText = _sessionService.L("Home_Activities");
        FeaturedText = _sessionService.L("Home_Featured");
        AllHousesText = _sessionService.L("Home_AllHouses");
        FeaturedBadgeText = _sessionService.L("Home_FeaturedBadge");
        FeaturedDetailsText = _sessionService.L("Pricing_Details");
        FeaturedBookText = _sessionService.L("Details_Book");
        CatAllText = _sessionService.L("Home_AllHouses");
        BannerTitleText = "PäevaSPA -40%";

        FindHouseText = lang switch
        {
            "ru" => "🔍 Подобрать дом",
            "en" => "🔍 Find my house",
            "fi" => "🔍 Löydä sopiva talo",
            _ => "🔍 Leia sobiv maja"
        };

        CatLargeText = lang switch
        {
            "ru" => "Большая группа",
            "en" => "Large group",
            "fi" => "Suuri ryhmä",
            _ => "Suur grupp"
        };

        BannerSubtitleText = lang switch
        {
            "ru" => "Пн–Чт 10:00–17:00 · Дом охотника & Русский дом",
            "en" => "Mon–Thu 10:00–17:00 · Hunter's Lodge & Russian House",
            "fi" => "Ma–To 10:00–17:00 · Metsästäjän talo & Venäläinen talo",
            _ => "E–N kl 10:00–17:00 · Jahimehe & Vene maja"
        };
    }

    private void UpdateGreeting()
    {
        if (!_sessionService.IsLoggedIn)
        {
            GreetingText = _sessionService.L("Home_Greeting");
            AvatarText = "👤";
            return;
        }

        var firstName = GetFirstName(_sessionService.CurrentUser!.FullName, _sessionService.CurrentUser.Email);
        var greeting = _sessionService.L("Home_Greeting").Replace("👋", string.Empty).Trim();
        GreetingText = $"{greeting}, {firstName}! 👋";
        AvatarText = firstName[..1].ToUpperInvariant();
    }

    private void UpdateFeaturedHouse()
    {
        if (_featuredHouse is null)
        {
            FeaturedImage = string.Empty;
            FeaturedTitle = string.Empty;
            FeaturedPrice = string.Empty;
            FeaturedGuests = string.Empty;
            return;
        }

        FeaturedImage = _featuredHouse.Image;
        FeaturedTitle = _featuredHouse.GetTitle(_sessionService.Language);
        FeaturedPrice = $"€{_featuredHouse.Price24h}";
        FeaturedGuests = _sessionService.L("Home_GuestsUpTo", _featuredHouse.MaxGuests);
    }

    private void SelectCategory(string? category)
    {
        _activeCategory = string.IsNullOrWhiteSpace(category) ? "all" : category;
        UpdateCategoryStyles();
        ApplyCategoryFilter();
    }

    private void ApplyCategoryFilter()
    {
        IEnumerable<HomeHouseDisplay> filtered = _allHouses.Where(house => house.HouseId != "soome");

        filtered = _activeCategory switch
        {
            "saun" => filtered.Where(house => house.SearchText.Contains("saun")),
            "bassein" => filtered.Where(house => house.SearchText.Contains("bassein")),
            "karaoke" => filtered.Where(house => house.SearchText.Contains("karaoke")),
            "large" => filtered.Where(house => house.MaxGuests >= 30),
            _ => filtered
        };

        Houses.Clear();
        foreach (var house in filtered)
            Houses.Add(house);
    }

    private void UpdateCategoryStyles()
    {
        CatAllBackground = BackgroundFor("all");
        CatSaunBackground = BackgroundFor("saun");
        CatBasseinBackground = BackgroundFor("bassein");
        CatKaraokeBackground = BackgroundFor("karaoke");
        CatLargeBackground = BackgroundFor("large");

        CatAllTextColor = TextColorFor("all");
        CatSaunTextColor = TextColorFor("saun");
        CatBasseinTextColor = TextColorFor("bassein");
        CatKaraokeTextColor = TextColorFor("karaoke");
        CatLargeTextColor = TextColorFor("large");
    }

    private Color BackgroundFor(string category)
        => _activeCategory == category ? ActiveCategoryBackground : InactiveCategoryBackground;

    private Color TextColorFor(string category)
        => _activeCategory == category ? ActiveCategoryText : InactiveCategoryText;

    private Task GoToFeaturedDetailsAsync()
    {
        if (_featuredHouse is null)
            return Task.CompletedTask;

        return Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId={_featuredHouse.Id}");
    }

    private Task GoToFeaturedBookingAsync()
    {
        if (_featuredHouse is null)
            return Task.CompletedTask;

        return _sessionService.IsLoggedIn
            ? Shell.Current.GoToAsync($"{nameof(BookingPage)}?houseId={_featuredHouse.Id}")
            : Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private Task OpenHouseDetailsAsync(string? houseId)
    {
        if (string.IsNullOrWhiteSpace(houseId))
            return Task.CompletedTask;

        return Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId={houseId}");
    }

    private static string GetFirstName(string fullName, string email)
    {
        var candidate = string.IsNullOrWhiteSpace(fullName)
            ? email.Split('@')[0]
            : fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;

        return string.IsNullOrWhiteSpace(candidate) ? "U" : candidate;
    }
}

public class HomeHouseDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string DisplayPrice { get; }
    public string Image { get; }
    public string GuestsText { get; }
    public string AmenitiesText { get; }
    public string SearchText { get; }
    public string ViewButtonText { get; }
    public int MaxGuests { get; }

    public HomeHouseDisplay(House house, string lang, SessionService session)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        DisplayPrice = $"€{house.Price24h}";
        Image = house.Image;
        MaxGuests = house.MaxGuests;
        AmenitiesText = string.Join(" ", house.GetAmenities(lang));
        SearchText = string.Join(" ",
            house.AmenitiesEt.Concat(house.AmenitiesRu)
                .Concat(house.AmenitiesEn)
                .Concat(house.AmenitiesFi))
            .ToLowerInvariant();
        ViewButtonText = session.L("Home_ViewMore");

        GuestsText = lang switch
        {
            "ru" => $"до {house.MaxGuests} гостей",
            "en" => $"up to {house.MaxGuests} guests",
            "fi" => $"enintään {house.MaxGuests} vierasta",
            _ => $"kuni {house.MaxGuests} külalist"
        };
    }
}