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

        // Views
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<HouseDetailsPage>();
        builder.Services.AddTransient<BookingPage>();
        builder.Services.AddTransient<BookingsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

        return builder.Build();
    }
}