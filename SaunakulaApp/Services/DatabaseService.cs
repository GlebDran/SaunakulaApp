using SQLite;
using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _db = null!;

    public async Task InitAsync()
    {
        if (_db != null) return;
        var path = Path.Combine(FileSystem.AppDataDirectory, "saunakula.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<User>();
        await _db.CreateTableAsync<Booking>();
        await _db.CreateTableAsync<Favourite>();
    }

    // ─── USERS ────────────────────────────────────────────────

    public Task<int> InsertUserAsync(User u) => _db.InsertAsync(u);

    public Task<User?> GetUserByEmailAsync(string email)
        => _db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

    public Task<User?> GetUserByIdAsync(int id)
        => _db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();

    public Task<int> UpdateUserAsync(User u) => _db.UpdateAsync(u);

    // ─── VIP ──────────────────────────────────────────────────

    /// <summary>
    /// Считает подтверждённые бронирования за последние 12 месяцев.
    /// Если >= 10 — присваивает VIP. Если год прошёл — сбрасывает.
    /// Возвращает (count за год, isVipActive).
    /// </summary>
    public async Task<(int Count, bool IsVip)> CheckAndUpdateVipAsync(int userId)
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);

        var bookings = await _db.Table<Booking>()
            .Where(b => b.UserId == userId &&
                        b.Status == "Confirmed" &&
                        b.CreatedAt >= oneYearAgo)
            .ToListAsync();

        var count = bookings.Count;
        var user = await GetUserByIdAsync(userId);
        if (user is null) return (count, false);

        // Сбрасываем если год прошёл
        if (user.IsVip && user.VipGrantedAt.HasValue &&
            user.VipGrantedAt.Value.AddYears(1) <= DateTime.Now)
        {
            user.IsVip = false;
            user.VipGrantedAt = null;
            await UpdateUserAsync(user);
        }

        // Даём VIP если достиг 10
        if (count >= 10 && !user.IsVip)
        {
            user.IsVip = true;
            user.VipGrantedAt = DateTime.Now;
            await UpdateUserAsync(user);
        }

        user = await GetUserByIdAsync(userId);
        return (count, user?.IsVipActive ?? false);
    }

    // ─── BOOKINGS ─────────────────────────────────────────────

    public Task<int> InsertBookingAsync(Booking b) => _db.InsertAsync(b);

    public Task<List<Booking>> GetBookingsByUserAsync(int userId)
        => _db.Table<Booking>().Where(b => b.UserId == userId).ToListAsync();

    public Task<int> CancelBookingAsync(int id)
        => _db.ExecuteAsync("UPDATE Booking SET Status='Cancelled' WHERE Id=?", id);

    public async Task<bool> IsHouseBookedAsync(string houseId,
                                               DateTime start,
                                               DateTime end,
                                               int? excludeBookingId = null)
    {
        var bookings = await _db.Table<Booking>()
            .Where(b => b.HouseId == houseId && b.Status == "Confirmed")
            .ToListAsync();

        foreach (var b in bookings)
        {
            if (excludeBookingId.HasValue && b.Id == excludeBookingId.Value)
                continue;
            if (start < b.EndDateTime && end > b.StartDateTime) return true;
        }
        return false;
    }

    public async Task<List<(DateTime Start, DateTime End)>> GetBookedPeriodsAsync(string houseId)
    {
        var bookings = await _db.Table<Booking>()
            .Where(b => b.HouseId == houseId && b.Status == "Confirmed")
            .ToListAsync();
        return bookings.Select(b => (b.StartDateTime, b.EndDateTime)).ToList();
    }

    // ─── FAVOURITES ───────────────────────────────────────────

    public Task<List<Favourite>> GetFavouritesByUserAsync(int userId)
        => _db.Table<Favourite>().Where(f => f.UserId == userId).ToListAsync();

    public async Task<bool> IsFavouriteAsync(int userId, string houseId)
    {
        var count = await _db.Table<Favourite>()
            .Where(f => f.UserId == userId && f.HouseId == houseId)
            .CountAsync();
        return count > 0;
    }

    public async Task ToggleFavouriteAsync(int userId, string houseId)
    {
        var existing = await _db.Table<Favourite>()
            .Where(f => f.UserId == userId && f.HouseId == houseId)
            .FirstOrDefaultAsync();
        if (existing != null)
            await _db.DeleteAsync(existing);
        else
            await _db.InsertAsync(new Favourite { UserId = userId, HouseId = houseId });
    }
}
