using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class DatabaseService
{
    private readonly SupabaseService _supabaseService;

    public DatabaseService(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public Task InitAsync() => Task.CompletedTask;

    public async Task<int> InsertUserAsync(User user)
    {
        var created = await _supabaseService.RegisterAppUserAsync(user);
        CopyToUser(created, user);
        return 1;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var remoteUser = await _supabaseService.GetAppUserByEmailAsync(email);
        return remoteUser is null ? null : SupabaseService.ToLocalUser(remoteUser);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var remoteUser = await _supabaseService.GetAppUserByIdAsync(id);
        return remoteUser is null ? null : SupabaseService.ToLocalUser(remoteUser);
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        await _supabaseService.UpdateAppUserAsync(user);
        return 1;
    }

    public Task<(int Count, bool IsVip)> CheckAndUpdateVipAsync(int userId)
        => _supabaseService.CheckAndUpdateVipAsync(userId);

    public async Task<int> InsertBookingAsync(Booking booking)
    {
        var created = await _supabaseService.CreateReservationAsync(booking);
        if (created is null)
            return 0;

        booking.Id = checked((int)created.Id);
        booking.Status = ToLocalStatus(created.Status);
        booking.CreatedAt = created.CreatedAt == default ? DateTime.Now : created.CreatedAt;
        return 1;
    }

    public Task<List<Booking>> GetBookingsByUserAsync(int userId)
        => _supabaseService.GetBookingsByUserAsync(userId);

    public Task<int> CancelBookingAsync(int id)
        => _supabaseService.CancelReservationAsync(id);

    public Task<bool> IsHouseBookedAsync(
        string houseId,
        DateTime start,
        DateTime end,
        int? excludeBookingId = null)
        => _supabaseService.IsHouseBookedAsync(houseId, start, end, excludeBookingId);

    public Task<List<(DateTime Start, DateTime End)>> GetBookedPeriodsAsync(string houseId)
        => _supabaseService.GetBookedPeriodsAsync(houseId);

    public Task<List<Favourite>> GetFavouritesByUserAsync(int userId)
        => _supabaseService.GetFavouritesByUserAsync(userId);

    public Task<bool> IsFavouriteAsync(int userId, string houseId)
        => _supabaseService.IsFavouriteAsync(userId, houseId);

    public Task ToggleFavouriteAsync(int userId, string houseId)
        => _supabaseService.ToggleFavouriteAsync(userId, houseId);

    private static void CopyToUser(SupabaseAppUser source, User target)
    {
        target.Id = checked((int)source.Id);
        target.FullName = source.FullName;
        target.Email = source.Email;
        target.PasswordHash = source.PasswordHash;
        target.Phone = source.Phone;
        target.IsVip = source.IsVip;
        target.VipGrantedAt = source.VipGrantedAt;
        target.CreatedAt = source.CreatedAt == default ? DateTime.Now : source.CreatedAt;
    }

    private static string ToLocalStatus(string status)
        => status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)
            ? "Cancelled"
            : "Confirmed";
}
