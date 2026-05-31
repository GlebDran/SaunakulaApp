using Postgrest.Constants;
using SaunakulaApp.Models;
using Supabase;

namespace SaunakulaApp.Services;

public class SupabaseService
{
    private const string SupabaseUrl = "https://agsocpclacqwrbxbvmpa.supabase.co";
    private const string SupabaseKey = "sb_publishable_HhxJ2THI7QsgSrGix9I4TA_frsEqPpv";

    private readonly Client _supabaseClient;

    private static readonly IReadOnlyDictionary<string, long> HouseIdMap =
        new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
        {
            ["soome"] = 1,
            ["jahimees"] = 2,
            ["vene"] = 3,
            ["spa"] = 4
        };

    public SupabaseService()
    {
        _supabaseClient = new Client(SupabaseUrl, SupabaseKey);
    }

    public async Task<bool> BookHouseAsync(string houseId, string customerName, DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be later than start date.");

        var remoteHouseId = ResolveRemoteHouseId(houseId);
        var startDate = start.Date;
        var endDate = end.Date;

        var existingReservations = await GetReservationsForHouseAsync(remoteHouseId);
        var isOverlapping = existingReservations.Any(reservation =>
            startDate < reservation.EndDate.Date && endDate > reservation.StartDate.Date);

        if (isOverlapping)
            return false;

        await InsertReservationAsync(remoteHouseId, customerName, startDate, endDate);
        return true;
    }

    public async Task<IReadOnlyList<SupabaseReservation>> GetReservationsForHouseAsync(string houseId)
    {
        var remoteHouseId = ResolveRemoteHouseId(houseId);
        return await GetReservationsForHouseAsync(remoteHouseId);
    }

    public async Task<IReadOnlyList<SupabaseReservation>> GetClientReservationsAsync(string customerName)
    {
        var response = await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("customer_name", Operator.Equals, customerName)
            .Get();

        return response.Models ?? new List<SupabaseReservation>();
    }

    public async Task DeleteReservationAsync(long reservationId)
    {
        await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("id", Operator.Equals, reservationId.ToString())
            .Delete();
    }

    private async Task<IReadOnlyList<SupabaseReservation>> GetReservationsForHouseAsync(long remoteHouseId)
    {
        var response = await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("house_id", Operator.Equals, remoteHouseId.ToString())
            .Get();

        return response.Models ?? new List<SupabaseReservation>();
    }

    private async Task InsertReservationAsync(long remoteHouseId, string customerName, DateTime start, DateTime end)
    {
        var reservation = new SupabaseReservation
        {
            HouseId = remoteHouseId,
            CustomerName = customerName,
            StartDate = start,
            EndDate = end
        };

        await _supabaseClient.From<SupabaseReservation>().Insert(reservation);
    }

    private static long ResolveRemoteHouseId(string houseId)
    {
        if (HouseIdMap.TryGetValue(houseId, out var remoteHouseId))
            return remoteHouseId;

        throw new InvalidOperationException($"House '{houseId}' is not mapped to Supabase houses table.");
    }
}
