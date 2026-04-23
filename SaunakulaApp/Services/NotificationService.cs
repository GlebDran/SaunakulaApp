using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class NotificationService
{
    private int _notificationId = 100;

    public async Task SendBookingConfirmedAsync(Booking booking,
                                                string houseTitle,
                                                string lang)
    {
        var title = lang switch
        {
            "ru" => "✅ Бронирование подтверждено!",
            "en" => "✅ Booking confirmed!",
            "fi" => "✅ Varaus vahvistettu!",
            _ => "✅ Broneering kinnitatud!"
        };

        var body = lang switch
        {
            "ru" => $"{houseTitle}\n{booking.StartDateTime:dd.MM.yyyy} – {booking.EndDateTime:dd.MM.yyyy}",
            "en" => $"{houseTitle}\n{booking.StartDateTime:dd.MM.yyyy} – {booking.EndDateTime:dd.MM.yyyy}",
            "fi" => $"{houseTitle}\n{booking.StartDateTime:dd.MM.yyyy} – {booking.EndDateTime:dd.MM.yyyy}",
            _ => $"{houseTitle}\n{booking.StartDateTime:dd.MM.yyyy} – {booking.EndDateTime:dd.MM.yyyy}"
        };

        var notification = new NotificationRequest
        {
            NotificationId = _notificationId++,
            Title = title,
            Description = body,
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(2)
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }

    public async Task ScheduleArrivalReminderAsync(Booking booking,
                                                   string houseTitle,
                                                   string lang)
    {
        var reminderTime = booking.StartDateTime.AddHours(-24);
        if (reminderTime < DateTime.Now)
            reminderTime = booking.StartDateTime.AddHours(-1);
        if (reminderTime < DateTime.Now) return;

        var title = lang switch
        {
            "ru" => "🏡 Напоминание о заезде",
            "en" => "🏡 Arrival reminder",
            "fi" => "🏡 Saapumismuistutus",
            _ => "🏡 Meeldetuletus saabumisest"
        };

        var body = lang switch
        {
            "ru" => $"Завтра: {houseTitle} · {booking.StartDateTime:dd.MM.yyyy}",
            "en" => $"Tomorrow: {houseTitle} · {booking.StartDateTime:dd.MM.yyyy}",
            "fi" => $"Huomenna: {houseTitle} · {booking.StartDateTime:dd.MM.yyyy}",
            _ => $"Homme: {houseTitle} · {booking.StartDateTime:dd.MM.yyyy}"
        };

        var notification = new NotificationRequest
        {
            NotificationId = _notificationId++,
            Title = title,
            Description = body,
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = reminderTime
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }

    public async Task<bool> RequestPermissionAsync()
        => await LocalNotificationCenter.Current.RequestNotificationPermission();
}