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
    }

    // Users
    public Task<int> InsertUserAsync(User u) => _db.InsertAsync(u);
    public Task<User?> GetUserByEmailAsync(string email)
        => _db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
    public Task<User?> GetUserByIdAsync(int id)
        => _db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();

    // Bookings
    public Task<int> InsertBookingAsync(Booking b) => _db.InsertAsync(b);
    public Task<List<Booking>> GetBookingsByUserAsync(int userId)
        => _db.Table<Booking>().Where(b => b.UserId == userId).ToListAsync();
    public Task<int> CancelBookingAsync(int id)
    {
        return _db.ExecuteAsync(
            "UPDATE Booking SET Status='Cancelled' WHERE Id=?", id);
    }
}