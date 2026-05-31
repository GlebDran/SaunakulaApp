using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class HouseService
{
    private readonly SupabaseService _supabaseService;
    private List<House>? _cachedHouses;

    public HouseService(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<House>> GetAllAsync()
    {
        if (_cachedHouses is not null)
            return _cachedHouses;

        _cachedHouses = await _supabaseService.GetHousesAsync();
        return _cachedHouses;
    }

    public async Task<House?> GetByIdAsync(string id)
    {
        var houses = await GetAllAsync();
        return houses.FirstOrDefault(house => house.Id == id);
    }

    public void ClearCache()
    {
        _cachedHouses = null;
    }
}
