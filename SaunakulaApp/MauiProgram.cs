using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<HouseService>();
        builder.Services.AddSingleton<SessionService>();

        // Views (добавим по мере создания)
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<HouseDetailsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<BookingPage>();

        return builder.Build();
    }
}