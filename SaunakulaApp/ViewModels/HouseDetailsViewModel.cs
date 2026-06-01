using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using SaunakulaApp.Models;
using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp.ViewModels;

public class HouseDetailsViewModel : BaseViewModel
{
    private const string ContactPhoneDisplay = "+372 5500 075";
    private const string ContactPhoneDial = "+37255000075";
    private const string ContactEmail = "sauna@saunamaailm.ee";
    private const string InstagramUrl = "https://www.instagram.com/sauna.kula/";

    private readonly HouseService _houseService;
    private readonly DatabaseService _databaseService;
    private readonly SessionService _sessionService;

    private House? _house;
    private int _activePhotoIndex;
    private string _houseTitle = string.Empty;
    private string _locationAddressText = string.Empty;
    private string _guestsUntilText = string.Empty;
    private string _guestsText = string.Empty;
    private string _sizeHeaderText = string.Empty;
    private string _sizeText = string.Empty;
    private string _minTimeText = string.Empty;
    private string _minHoursText = string.Empty;
    private string _descriptionHeaderText = string.Empty;
    private string _descriptionText = string.Empty;
    private string _amenitiesHeaderText = string.Empty;
    private string _pricingHeaderText = string.Empty;
    private string _hourlyPriceHeaderText = string.Empty;
    private string _hourlyPriceText = string.Empty;
    private string _dayPriceHeaderText = string.Empty;
    private string _dayPriceText = string.Empty;
    private string _regularPriceText = string.Empty;
    private string _minHoursNoteText = string.Empty;
    private string _locationHeaderText = string.Empty;
    private string _openMapsText = string.Empty;
    private string _contactHeaderText = string.Empty;
    private string _callText = string.Empty;
    private string _emailText = string.Empty;
    private string _bottomPriceHeaderText = string.Empty;
    private string _bottomPriceText = string.Empty;
    private string _bookButtonText = string.Empty;
    private string _favouriteIcon = "\ud83e\udd0d";
    private HtmlWebViewSource _mapSource = CreateMapSource();

    public ObservableCollection<string> Photos { get; } = new();
    public ObservableCollection<string> Amenities { get; } = new();
    public ObservableCollection<GalleryDot> PhotoDots { get; } = new();

    public IAsyncRelayCommand BackCommand { get; }
    public IAsyncRelayCommand ToggleFavouriteCommand { get; }
    public IAsyncRelayCommand OpenMapsCommand { get; }
    public IAsyncRelayCommand CallCommand { get; }
    public IAsyncRelayCommand EmailCommand { get; }
    public IAsyncRelayCommand InstagramCommand { get; }
    public IAsyncRelayCommand BookCommand { get; }

    public HouseDetailsViewModel(
        HouseService houseService,
        DatabaseService databaseService,
        SessionService sessionService)
    {
        _houseService = houseService;
        _databaseService = databaseService;
        _sessionService = sessionService;

        BackCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
        ToggleFavouriteCommand = new AsyncRelayCommand(ToggleFavouriteAsync);
        OpenMapsCommand = new AsyncRelayCommand(OpenMapsAsync);
        CallCommand = new AsyncRelayCommand(CallAsync);
        EmailCommand = new AsyncRelayCommand(EmailAsync);
        InstagramCommand = new AsyncRelayCommand(OpenInstagramAsync);
        BookCommand = new AsyncRelayCommand(BookAsync);
    }

    public int PhotoCount => Photos.Count;

    public string HouseTitle
    {
        get => _houseTitle;
        private set => SetProperty(ref _houseTitle, value);
    }

    public string LocationAddressText
    {
        get => _locationAddressText;
        private set => SetProperty(ref _locationAddressText, value);
    }

    public string GuestsUntilText
    {
        get => _guestsUntilText;
        private set => SetProperty(ref _guestsUntilText, value);
    }

    public string GuestsText
    {
        get => _guestsText;
        private set => SetProperty(ref _guestsText, value);
    }

    public string SizeHeaderText
    {
        get => _sizeHeaderText;
        private set => SetProperty(ref _sizeHeaderText, value);
    }

    public string SizeText
    {
        get => _sizeText;
        private set => SetProperty(ref _sizeText, value);
    }

    public string MinTimeText
    {
        get => _minTimeText;
        private set => SetProperty(ref _minTimeText, value);
    }

    public string MinHoursText
    {
        get => _minHoursText;
        private set => SetProperty(ref _minHoursText, value);
    }

    public string DescriptionHeaderText
    {
        get => _descriptionHeaderText;
        private set => SetProperty(ref _descriptionHeaderText, value);
    }

    public string DescriptionText
    {
        get => _descriptionText;
        private set => SetProperty(ref _descriptionText, value);
    }

    public string AmenitiesHeaderText
    {
        get => _amenitiesHeaderText;
        private set => SetProperty(ref _amenitiesHeaderText, value);
    }

    public string PricingHeaderText
    {
        get => _pricingHeaderText;
        private set => SetProperty(ref _pricingHeaderText, value);
    }

    public string HourlyPriceHeaderText
    {
        get => _hourlyPriceHeaderText;
        private set => SetProperty(ref _hourlyPriceHeaderText, value);
    }

    public string HourlyPriceText
    {
        get => _hourlyPriceText;
        private set => SetProperty(ref _hourlyPriceText, value);
    }

    public string DayPriceHeaderText
    {
        get => _dayPriceHeaderText;
        private set => SetProperty(ref _dayPriceHeaderText, value);
    }

    public string DayPriceText
    {
        get => _dayPriceText;
        private set => SetProperty(ref _dayPriceText, value);
    }

    public string RegularPriceText
    {
        get => _regularPriceText;
        private set => SetProperty(ref _regularPriceText, value);
    }

    public string MinHoursNoteText
    {
        get => _minHoursNoteText;
        private set => SetProperty(ref _minHoursNoteText, value);
    }

    public string LocationHeaderText
    {
        get => _locationHeaderText;
        private set => SetProperty(ref _locationHeaderText, value);
    }

    public string OpenMapsText
    {
        get => _openMapsText;
        private set => SetProperty(ref _openMapsText, value);
    }

    public string ContactHeaderText
    {
        get => _contactHeaderText;
        private set => SetProperty(ref _contactHeaderText, value);
    }

    public string CallText
    {
        get => _callText;
        private set => SetProperty(ref _callText, value);
    }

    public string EmailText
    {
        get => _emailText;
        private set => SetProperty(ref _emailText, value);
    }

    public string BottomPriceHeaderText
    {
        get => _bottomPriceHeaderText;
        private set => SetProperty(ref _bottomPriceHeaderText, value);
    }

    public string BottomPriceText
    {
        get => _bottomPriceText;
        private set => SetProperty(ref _bottomPriceText, value);
    }

    public string BookButtonText
    {
        get => _bookButtonText;
        private set => SetProperty(ref _bookButtonText, value);
    }

    public string FavouriteIcon
    {
        get => _favouriteIcon;
        private set => SetProperty(ref _favouriteIcon, value);
    }

    public HtmlWebViewSource MapSource
    {
        get => _mapSource;
        private set => SetProperty(ref _mapSource, value);
    }

    public async Task InitializeAsync(string houseId)
    {
        if (string.IsNullOrWhiteSpace(houseId))
            return;

        try
        {
            IsBusy = true;

            _house = await _houseService.GetByIdAsync(houseId);
            if (_house is null)
                return;

            ApplyLocalization();
            ApplyHouseDetails();
            await UpdateFavouriteButtonAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void SetActivePhotoIndex(int index)
    {
        if (PhotoDots.Count == 0)
            return;

        _activePhotoIndex = Math.Clamp(index, 0, PhotoDots.Count - 1);
        RebuildPhotoDots();
    }

    private void ApplyLocalization()
    {
        GuestsUntilText = _sessionService.L("Details_Guests");
        SizeHeaderText = _sessionService.L("Details_Size");
        MinTimeText = _sessionService.L("Details_MinTime");
        DescriptionHeaderText = _sessionService.L("Details_Description");
        AmenitiesHeaderText = _sessionService.L("Details_Amenities");
        PricingHeaderText = _sessionService.L("Details_Pricing");
        HourlyPriceHeaderText = _sessionService.L("Details_HourlyPrice");
        DayPriceHeaderText = _sessionService.L("Details_DayPrice");
        MinHoursNoteText = _sessionService.L("Details_MinHours");
        LocationHeaderText = _sessionService.L("Details_Location");
        OpenMapsText = _sessionService.L("Details_OpenMaps");
        ContactHeaderText = _sessionService.L("Details_Contact");
        CallText = _sessionService.L("Details_Call");
        EmailText = _sessionService.L("Details_Email");
        BottomPriceHeaderText = _sessionService.L("Details_Price24h");
        BookButtonText = _sessionService.L("Details_Book");
        LocationAddressText = _sessionService.L("Details_Location_Address");
    }

    private void ApplyHouseDetails()
    {
        if (_house is null)
            return;

        var language = _sessionService.Language;

        HouseTitle = _house.GetTitle(language);
        DescriptionText = _house.GetDescription(language);
        GuestsText = $"{_house.MaxGuests} {_sessionService.L("Details_Guests").ToLowerInvariant()}.";
        SizeText = $"{_house.SizeM2} m\u00b2";
        MinHoursText = $"{_house.MinHours}h";
        HourlyPriceText = $"\u20ac{_house.PricePerHour}/h";
        DayPriceText = $"\u20ac{_house.Price24h}";
        RegularPriceText = $"\u20ac{_house.Price24hRegular}";
        BottomPriceText = $"\u20ac{_house.Price24h}";
        MapSource = CreateMapSource();

        Amenities.Clear();
        foreach (var amenity in _house.GetAmenities(language))
            Amenities.Add(amenity);

        Photos.Clear();
        var photos = _house.PhotoUrls.Any()
            ? _house.PhotoUrls
            : new List<string> { _house.Image };

        foreach (var photo in photos)
            Photos.Add(photo);

        OnPropertyChanged(nameof(PhotoCount));
        _activePhotoIndex = 0;
        RebuildPhotoDots();
    }

    private void RebuildPhotoDots()
    {
        PhotoDots.Clear();

        for (var i = 0; i < Photos.Count; i++)
        {
            var isActive = i == _activePhotoIndex;
            PhotoDots.Add(new GalleryDot(
                isActive ? 20 : 7,
                isActive ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#80FFFFFF")));
        }
    }

    private async Task UpdateFavouriteButtonAsync()
    {
        if (!_sessionService.IsLoggedIn || _house is null)
        {
            FavouriteIcon = "\ud83e\udd0d";
            return;
        }

        await _databaseService.InitAsync();
        var isFavourite = await _databaseService.IsFavouriteAsync(
            _sessionService.CurrentUser!.Id,
            _house.Id);

        FavouriteIcon = isFavourite ? "\u2764\ufe0f" : "\ud83e\udd0d";
    }

    private async Task ToggleFavouriteAsync()
    {
        if (!_sessionService.IsLoggedIn)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
            return;
        }

        if (_house is null)
            return;

        await _databaseService.InitAsync();
        await _databaseService.ToggleFavouriteAsync(_sessionService.CurrentUser!.Id, _house.Id);
        await UpdateFavouriteButtonAsync();
    }

    private static Task OpenMapsAsync()
    {
        var uri = DeviceInfo.Platform == DevicePlatform.iOS
            ? "maps://?ll=59.3635824,24.5131448&q=Saunakula"
            : "geo:59.3635824,24.5131448?q=59.3635824,24.5131448(Saunakula)";

        return Launcher.Default.OpenAsync(new Uri(uri));
    }

    private static async Task CallAsync()
    {
        if (PhoneDialer.Default.IsSupported)
        {
            PhoneDialer.Default.Open(ContactPhoneDial);
            return;
        }

        await Shell.Current.DisplayAlert("Telefon", ContactPhoneDisplay, "OK");
    }

    private async Task EmailAsync()
    {
        try
        {
            if (!Email.Default.IsComposeSupported)
            {
                await Shell.Current.DisplayAlert("E-post", ContactEmail, "OK");
                return;
            }

            var message = new EmailMessage
            {
                Subject = $"Broneeringu paring - {HouseTitle}",
                Body = string.Empty,
                To = new List<string> { ContactEmail }
            };

            await Email.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlert("E-post", ContactEmail, "OK");
        }
    }

    private static Task OpenInstagramAsync()
        => Browser.Default.OpenAsync(InstagramUrl, BrowserLaunchMode.SystemPreferred);

    private Task BookAsync()
    {
        if (_house is null)
            return Task.CompletedTask;

        return _sessionService.IsLoggedIn
            ? Shell.Current.GoToAsync($"{nameof(BookingPage)}?houseId={_house.Id}")
            : Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private static HtmlWebViewSource CreateMapSource()
        => new()
        {
            Html = @"<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>* { margin:0; padding:0; } body { height:200px; overflow:hidden; } iframe { width:100%; height:200px; border:0; }</style>
</head>
<body>
    <iframe src='https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2031.8!2d24.5131448!3d59.3635824!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x4692bde914230565%3A0xa6fff1cee72bb1fe!2sSaunakyla!5e0!3m2!1sen!2see!4v1' allowfullscreen='' loading='lazy' referrerpolicy='no-referrer-when-downgrade'></iframe>
</body>
</html>"
        };
}

public class GalleryDot
{
    public double Width { get; }
    public Color Color { get; }

    public GalleryDot(double width, Color color)
    {
        Width = width;
        Color = color;
    }
}
