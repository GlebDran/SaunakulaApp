using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class SplashPage : ContentPage
{
    private readonly DatabaseService _db;

    public SplashPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await InitializeApp();
    }

    private async Task InitializeApp()
    {
        // Инициализируем БД параллельно с анимацией
        await Task.WhenAll(
            _db.InitAsync(),
            Task.Delay(2000) // минимум 2 секунды показываем splash
        );

        // Переходим на главную
        await Shell.Current.GoToAsync("//HomePage");
    }
}