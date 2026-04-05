using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class HouseDetailsPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;
    private House? _house;
    private int _photoCount = 0;
    private bool _scrollHandlerAttached = false;

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

        // Подписываемся на скролл только один раз
        if (!_scrollHandlerAttached)
        {
            PhotoGallery.Scrolled += OnGalleryScrolled;
            _scrollHandlerAttached = true;
        }
    }

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

        // Вычисляем индекс по горизонтальному смещению
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
