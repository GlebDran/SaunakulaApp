using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class PricingPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;

    public PricingPage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;
        PricingList.ItemsSource = houses.Select(h => new PricingDisplay(h, lang, _session)).ToList();
    }

    private void ApplyLocalization()
    {
        var lang = _session.Language;

        PricingTitleLabel.Text = _session.L("Pricing_Title");
        PricingSubtitleLabel.Text = _session.L("Pricing_Subtitle");

        // Chips
        OpenLabel.Text = lang switch
        {
            "ru" => "Открыто 24/7",
            "en" => "Open 24/7",
            "fi" => "Avoinna 24/7",
            _ => "Avatud 24/7"
        };
        ParkingLabel.Text = lang switch
        {
            "ru" => "Бесплатная парковка",
            "en" => "Free parking",
            "fi" => "Ilmainen pysäköinti",
            _ => "Tasuta parkimine"
        };
        GiftLabel.Text = lang switch
        {
            "ru" => "Веник + вода в подарок",
            "en" => "Whisk + water as gift",
            "fi" => "Vihta + vesi lahjaksi",
            _ => "Viht + vesi kingituseks"
        };
        WifiLabel.Text = lang switch
        {
            "ru" => "Бесплатный WiFi",
            "en" => "Free WiFi",
            "fi" => "Ilmainen WiFi",
            _ => "Tasuta WiFi"
        };

        // Extras table header
        ExtrasHeaderLabel.Text = _session.L("Pricing_Extras");
        ExtrasServiceLabel.Text = lang switch
        {
            "ru" => "Услуга",
            "en" => "Service",
            "fi" => "Palvelu",
            _ => "Teenus"
        };
        ExtrasPriceLabel.Text = lang switch
        {
            "ru" => "Цена",
            "en" => "Price",
            "fi" => "Hinta",
            _ => "Hind"
        };

        // Extras items
        TunnLabel.Text = lang switch
        {
            "ru" => "Купель (предзаказ!)",
            "en" => "Hot tub (pre-order!)",
            "fi" => "Kylpytynnyri (ennakkotilaus!)",
            _ => "Kümblustünn (etteteatamine!)"
        };
        KasevihtLabel.Text = lang switch
        {
            "ru" => "Берёзовый веник",
            "en" => "Birch whisk",
            "fi" => "Koivuvihta",
            _ => "Kaaseviht"
        };
        TammevihtLabel.Text = lang switch
        {
            "ru" => "Дубовый веник",
            "en" => "Oak whisk",
            "fi" => "Tammivihta",
            _ => "Tammeviht"
        };
        LinaLabel.Text = lang switch
        {
            "ru" => "Аренда полотенца",
            "en" => "Towel rental",
            "fi" => "Pyyheliina vuokra",
            _ => "Saunalina rent"
        };
        AroomLabel.Text = lang switch
        {
            "ru" => "Аромат для сауны",
            "en" => "Sauna aroma",
            "fi" => "Saunaaromi",
            _ => "Saunaaroom"
        };
        SusiLabel.Text = lang switch
        {
            "ru" => "Уголь для гриля",
            "en" => "Charcoal",
            "fi" => "Grillihiili",
            _ => "Grillsüsi"
        };
        SoftLabel.Text = lang switch
        {
            "ru" => "Безалкогольные напитки от",
            "en" => "Non-alcoholic drinks from",
            "fi" => "Alkoholittomat juomat alkaen",
            _ => "Alkoholivabad joogid al."
        };
        AlcoLabel.Text = lang switch
        {
            "ru" => "Алкогольные напитки от",
            "en" => "Alcoholic drinks from",
            "fi" => "Alkoholijuomat alkaen",
            _ => "Alkohoolsed joogid al."
        };
    }

    private async void Details_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string houseId)
            await Shell.Current.GoToAsync($"{nameof(HouseDetailsPage)}?houseId={houseId}");
    }

    private async void Book_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string houseId)
        {
            if (_session.IsLoggedIn)
                await Shell.Current.GoToAsync($"{nameof(BookingPage)}?houseId={houseId}");
            else
                await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}

public class PricingDisplay
{
    public string HouseId { get; }
    public string DisplayTitle { get; }
    public string Image { get; }
    public string GuestsText { get; }
    public string HourlyText { get; }
    public string HourlyHeaderText { get; }
    public string DayPriceText { get; }
    public string DayPriceHeaderText { get; }
    public string RegularPriceText { get; }
    public string MinHoursText { get; }
    public string DetailsButtonText { get; }
    public string BookButtonText { get; }

    public PricingDisplay(Models.House house, string lang, SessionService session)
    {
        HouseId = house.Id;
        DisplayTitle = house.GetTitle(lang);
        Image = house.Image;

        GuestsText = lang switch
        {
            "ru" => $"До {house.MaxGuests} гостей · {house.SizeM2} m²",
            "en" => $"Up to {house.MaxGuests} guests · {house.SizeM2} m²",
            "fi" => $"Enintään {house.MaxGuests} vierasta · {house.SizeM2} m²",
            _ => $"Kuni {house.MaxGuests} külalist · {house.SizeM2} m²"
        };

        HourlyHeaderText = lang switch
        {
            "ru" => "Цена за час (4–7ч)",
            "en" => "Hourly rate (4–7h)",
            "fi" => "Tuntihinta (4–7h)",
            _ => "Tunnihind (4–7h)"
        };

        HourlyText = lang switch
        {
            "ru" => $"€{house.PricePerHour} / час",
            "en" => $"€{house.PricePerHour} / hour",
            "fi" => $"€{house.PricePerHour} / tunti",
            _ => $"€{house.PricePerHour} / tund"
        };

        DayPriceHeaderText = lang switch
        {
            "ru" => "Цена за 24ч",
            "en" => "24h price",
            "fi" => "24h hinta",
            _ => "24h hind"
        };

        DayPriceText = $"€{house.Price24h}";
        RegularPriceText = $"€{house.Price24hRegular}";

        MinHoursText = lang switch
        {
            "ru" => $"Минимальное время бронирования {house.MinHours}ч",
            "en" => $"Minimum booking time {house.MinHours}h",
            "fi" => $"Vähimmäisvarausaika {house.MinHours}h",
            _ => $"Minimaalne broneerimisaeg {house.MinHours}h"
        };

        DetailsButtonText = session.L("Pricing_Details");
        BookButtonText = session.L("Pricing_Book");
    }
}
