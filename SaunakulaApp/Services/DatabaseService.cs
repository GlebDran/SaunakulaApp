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

    // ─── BOOKINGS ─────────────────────────────────────────────

    public Task<int> InsertBookingAsync(Booking b) => _db.InsertAsync(b);

    public Task<List<Booking>> GetBookingsByUserAsync(int userId)
        => _db.Table<Booking>().Where(b => b.UserId == userId).ToListAsync();

    public Task<int> CancelBookingAsync(int id)
        => _db.ExecuteAsync("UPDATE Booking SET Status='Cancelled' WHERE Id=?", id);

    /// <summary>
    /// Проверяет пересечение дат для конкретного дома.
    /// Возвращает true если дом занят в указанный период.
    /// </summary>
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
            // Пропускаем текущее бронирование (для редактирования)
            if (excludeBookingId.HasValue && b.Id == excludeBookingId.Value)
                continue;

            // Пересечение: новый start < существующий end И новый end > существующий start
            bool overlaps = start < b.EndDateTime && end > b.StartDateTime;
            if (overlaps) return true;
        }

        return false;
    }

    /// <summary>
    /// Возвращает список занятых периодов для дома (для отображения).
    /// </summary>
    public async Task<List<(DateTime Start, DateTime End)>> GetBookedPeriodsAsync(string houseId)
    {
        var bookings = await _db.Table<Booking>()
            .Where(b => b.HouseId == houseId && b.Status == "Confirmed")
            .ToListAsync();

        return bookings
            .Select(b => (b.StartDateTime, b.EndDateTime))
            .ToList();
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
