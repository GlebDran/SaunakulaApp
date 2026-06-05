using Plugin.LocalNotification;
using SaunakulaApp.Services;
using SaunakulaApp.ViewModels;
using SaunakulaApp.Views;

namespace SaunakulaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<HouseService>();
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<LocalizationService>();
        builder.Services.AddSingleton<SupabaseService>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HouseDetailsViewModel>();
        builder.Services.AddTransient<BookingViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<BookingsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Views
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<HouseDetailsPage>();
        builder.Services.AddTransient<BookingPage>();
        builder.Services.AddTransient<BookingsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<PricingPage>();
        builder.Services.AddTransient<HouseFinderPage>();

        return builder.Build();
    }
}
