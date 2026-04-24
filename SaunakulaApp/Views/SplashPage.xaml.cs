using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class SplashPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly SessionService _session;

    public SplashPage(DatabaseService db, SessionService session)
    {
        InitializeComponent();
        _db = db;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await InitializeApp();
    }

    private async Task InitializeApp()
    {
        // Инициализируем БД и ждём минимум 1.5 сек
        await Task.WhenAll(
            _db.InitAsync(),
            Task.Delay(1500)
        );

        // Прячем лоадер, показываем выбор языка
        Loader.IsVisible = false;
        Loader.IsRunning = false;

        // Анимация появления
        LangView.Opacity = 0;
        LangView.IsVisible = true;
        await LangView.FadeTo(1, 400);
    }

    private async Task SelectLanguage(string lang)
    {
        // Подсвечиваем выбранную кнопку
        var selected = lang switch
        {
            "ru" => BtnRu,
            "en" => BtnEn,
            "fi" => BtnFi,
            _ => BtnEt
        };

        selected.BackgroundColor = Color.FromArgb("#66FFFFFF");
        await Task.Delay(200);

        // Сохраняем язык
        _session.SetLanguage(lang);

        // Плавный переход
        await this.FadeTo(0, 300);
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void LangEt_Tapped(object sender, TappedEventArgs e)
        => await SelectLanguage("et");

    private async void LangRu_Tapped(object sender, TappedEventArgs e)
        => await SelectLanguage("ru");

    private async void LangEn_Tapped(object sender, TappedEventArgs e)
        => await SelectLanguage("en");

    private async void LangFi_Tapped(object sender, TappedEventArgs e)
        => await SelectLanguage("fi");
}
