using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class HouseDetailsPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly DatabaseService _db;
    private readonly SessionService _session;
    private House? _house;
    private int _photoCount = 0;
    private bool _scrollHandlerAttached = false;

    public string HouseId { get; set; } = "";

    public HouseDetailsPage(HouseService houseService,
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

        _house = await _houseService.GetByIdAsync(HouseId);
        if (_house is null) return;

        var lang = _session.Language;

        TitleLabel.Text = _house.GetTitle(lang);
        DescLabel.Text = _house.GetDescription(lang);
        GuestsLabel.Text = $"{_house.MaxGuests} külast.";
        SizeLabel.Text = $"{_house.SizeM2} m²";
        MinHoursLabel.Text = $"{_house.MinHours}h";
        HourlyPriceLabel.Text = $"€{_house.PricePerHour}/h";
        DayPriceLabel.Text = $"€{_house.Price24h}";
        RegularPriceLabel.Text = $"€{_house.Price24hRegular}";
        BottomPriceLabel.Text = $"€{_house.Price24h}";

        AmenitiesView.ItemsSource = _house.GetAmenities(lang);

        var photos = _house.PhotoUrls.Any()
            ? _house.PhotoUrls
            : new List<string> { _house.Image };

        _photoCount = photos.Count;
        PhotoGallery.ItemsSource = photos;
        BuildDots(_photoCount);
        LoadMap();
        await UpdateFavouriteButton();

        if (!_scrollHandlerAttached)
        {
            PhotoGallery.Scrolled += OnGalleryScrolled;
            _scrollHandlerAttached = true;
        }
    }

    // ── Gallery ───────────────────────────────────────────────

    private void BuildDots(int count)
    {
        DotsLayout.Children.Clear();
        for (int i = 0; i < count; i++)
        {
            DotsLayout.Children.Add(new BoxView
            {
                WidthRequest = i == 0 ? 20 : 7,
                HeightRequest = 7,
                CornerRadius = new CornerRadius(4),
                Color = i == 0
                    ? Color.FromArgb("#FFFFFF")
                    : Color.FromArgb("#80FFFFFF"),
                VerticalOptions = LayoutOptions.Center
            });
        }
    }

    private void OnGalleryScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        if (_photoCount <= 1) return;
        var itemWidth = Math.Max(Width, 1);
        var idx = (int)Math.Round(e.HorizontalOffset / itemWidth);
        idx = Math.Clamp(idx, 0, _photoCount - 1);
        UpdateDots(idx);
    }

    private void UpdateDots(int activeIndex)
    {
        for (int i = 0; i < DotsLayout.Children.Count; i++)
        {
            if (DotsLayout.Children[i] is BoxView dot)
            {
                dot.WidthRequest = i == activeIndex ? 20 : 7;
                dot.Color = i == activeIndex
                    ? Color.FromArgb("#FFFFFF")
                    : Color.FromArgb("#80FFFFFF");
            }
        }
    }

    // ── Map ───────────────────────────────────────────────────

    private void LoadMap()
    {
        var html = @"<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>* { margin:0; padding:0; } body { height:200px; overflow:hidden; } iframe { width:100%; height:200px; border:0; }</style>
</head>
<body>
    <iframe src='https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2031.8!2d24.5131448!3d59.3635824!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x4692bde914230565%3A0xa6fff1cee72bb1fe!2sSaunakyla!5e0!3m2!1sen!2see!4v1' allowfullscreen='' loading='lazy' referrerpolicy='no-referrer-when-downgrade'></iframe>
</body>
</html>";
        MapView.Source = new HtmlWebViewSource { Html = html };
    }

    private void OpenMaps_Clicked(object sender, EventArgs e)
    {
        Launcher.Default.OpenAsync(
            new Uri("https://www.google.com/maps/place/Saunak%C3%BCla/@59.3635824,24.5131448,17z"));
    }

    // ── Favourites ────────────────────────────────────────────

    private async Task UpdateFavouriteButton()
    {
        if (!_session.IsLoggedIn || _house is null)
        {
            FavLabel.Text = "🤍";
            return;
        }
        await _db.InitAsync();
        var isFav = await _db.IsFavouriteAsync(
            _session.CurrentUser!.Id, _house.Id);
        FavLabel.Text = isFav ? "❤️" : "🤍";
    }

    private async void Favourite_Tapped(object sender, TappedEventArgs e)
    {
        if (!_session.IsLoggedIn)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
            return;
        }
        if (_house is null) return;

        await _db.InitAsync();
        await _db.ToggleFavouriteAsync(_session.CurrentUser!.Id, _house.Id);
        await UpdateFavouriteButton();
    }

    // ── Contact ───────────────────────────────────────────────

    private async void Call_Tapped(object sender, TappedEventArgs e)
    {
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("+37255000075");
        else
            await DisplayAlert("Telefon", "+372 5500 075", "OK");
    }

    private async void Email_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (!Email.Default.IsComposeSupported)
            {
                await DisplayAlert("E-post", "sauna@saunamaailm.ee", "OK");
                return;
            }
            var message = new EmailMessage
            {
                Subject = $"Broneeringu päring – {_house?.GetTitle(_session.Language)}",
                Body = "",
                To = new List<string> { "sauna@saunamaailm.ee" }
            };
            await Email.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("E-post", "sauna@saunamaailm.ee", "OK");
        }
    }

    // ── Navigation ────────────────────────────────────────────

    private void Back_Tapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("..");

    private async void Book_Clicked(object sender, EventArgs e)
    {
        if (_session.IsLoggedIn)
            await Shell.Current.GoToAsync(
                $"{nameof(BookingPage)}?houseId={_house?.Id}");
        else
            await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
