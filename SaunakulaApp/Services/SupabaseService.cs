using Postgrest.Constants;
using SaunakulaApp.Models;
using Supabase;

namespace SaunakulaApp.Services;

public class SupabaseService
{
    private const string SupabaseUrl = "https://pqfaxavhwafzsnfnvxtl.supabase.co";
    private const string SupabaseKey = "sb_publishable_34KSEXYms2DI15KcaSey_Q_I6Q-LJlm";

    private readonly Client _supabaseClient;

    public SupabaseService()
    {
        _supabaseClient = new Client(SupabaseUrl, SupabaseKey);
    }

    public async Task<List<House>> GetHousesAsync()
    {
        var housesResponse = await _supabaseClient.From<SupabaseHouse>().Get();
        var translationsResponse = await _supabaseClient.From<SupabaseHouseTranslation>().Get();
        var amenitiesResponse = await _supabaseClient.From<SupabaseHouseAmenity>().Get();
        var photosResponse = await _supabaseClient.From<SupabaseHousePhoto>().Get();

        var translations = (translationsResponse.Models ?? new List<SupabaseHouseTranslation>())
            .GroupBy(item => item.HouseId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var amenities = (amenitiesResponse.Models ?? new List<SupabaseHouseAmenity>())
            .GroupBy(item => item.HouseId)
            .ToDictionary(group => group.Key, group => group.OrderBy(item => item.SortOrder).ToList());

        var photos = (photosResponse.Models ?? new List<SupabaseHousePhoto>())
            .GroupBy(item => item.HouseId)
            .ToDictionary(group => group.Key, group => group.OrderBy(item => item.SortOrder).ToList());

        return (housesResponse.Models ?? new List<SupabaseHouse>())
            .Where(item => item.IsActive)
            .OrderBy(item => item.Id)
            .Select(item => ToHouse(item, translations, amenities, photos))
            .ToList();
    }

    public async Task<bool> BookHouseAsync(
        string houseId,
        string customerName,
        DateTime start,
        DateTime end,
        int guestCount = 1,
        decimal totalPrice = 0,
        string notes = "")
    {
        if (end <= start)
            throw new ArgumentException("End date must be later than start date.");

        var remoteHouseId = await ResolveRemoteHouseIdAsync(houseId);
        var startDate = start.Date;
        var endDate = end.Date;

        var existingReservations = await GetReservationsForHouseAsync(remoteHouseId);
        var isOverlapping = existingReservations.Any(reservation =>
            reservation.Status == "confirmed" &&
            startDate < reservation.EndDate.Date &&
            endDate > reservation.StartDate.Date);

        if (isOverlapping)
            return false;

        return await InsertReservationAsync(remoteHouseId, customerName, startDate, endDate, guestCount, totalPrice, notes);
    }

    public async Task<IReadOnlyList<SupabaseReservation>> GetReservationsForHouseAsync(string houseId)
    {
        var remoteHouseId = await ResolveRemoteHouseIdAsync(houseId);
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
            .Filter("status", Operator.Equals, "confirmed")
            .Get();

        return response.Models ?? new List<SupabaseReservation>();
    }

    private async Task<bool> InsertReservationAsync(
        long remoteHouseId,
        string customerName,
        DateTime start,
        DateTime end,
        int guestCount,
        decimal totalPrice,
        string notes)
    {
        var reservation = new SupabaseReservation
        {
            HouseId = remoteHouseId,
            CustomerName = customerName,
            StartDate = start,
            EndDate = end,
            GuestCount = guestCount,
            TotalPrice = totalPrice,
            Notes = notes,
            Status = "confirmed"
        };

        try
        {
            await _supabaseClient.From<SupabaseReservation>().Insert(reservation);
            return true;
        }
        catch (Exception ex) when (ex.Message.Contains("reservations_no_overlap", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
    }

    private async Task<long> ResolveRemoteHouseIdAsync(string houseId)
    {
        var response = await _supabaseClient
            .From<SupabaseHouse>()
            .Filter("app_house_id", Operator.Equals, houseId)
            .Get();

        var house = response.Models?.FirstOrDefault();
        return house?.Id ?? throw new InvalidOperationException($"House '{houseId}' does not exist in the central database.");
    }

    private static House ToHouse(
        SupabaseHouse source,
        IReadOnlyDictionary<long, List<SupabaseHouseTranslation>> translations,
        IReadOnlyDictionary<long, List<SupabaseHouseAmenity>> amenities,
        IReadOnlyDictionary<long, List<SupabaseHousePhoto>> photos)
    {
        var house = new House
        {
            Id = source.AppHouseId,
            Image = source.Image,
            MaxGuests = source.MaxGuests,
            SizeM2 = source.SizeM2,
            PricePerHour = source.PricePerHour,
            Price24h = source.Price24h,
            Price24hRegular = source.Price24hRegular,
            MinHours = source.MinHours
        };

        if (translations.TryGetValue(source.Id, out var houseTranslations))
        {
            foreach (var translation in houseTranslations)
            {
                ApplyTranslation(house, translation);
            }
        }

        if (amenities.TryGetValue(source.Id, out var houseAmenities))
        {
            house.AmenitiesEt = houseAmenities.Where(item => item.Language == "et").Select(item => item.Text).ToList();
            house.AmenitiesRu = houseAmenities.Where(item => item.Language == "ru").Select(item => item.Text).ToList();
            house.AmenitiesEn = houseAmenities.Where(item => item.Language == "en").Select(item => item.Text).ToList();
            house.AmenitiesFi = houseAmenities.Where(item => item.Language == "fi").Select(item => item.Text).ToList();
        }

        if (photos.TryGetValue(source.Id, out var housePhotos))
        {
            house.PhotoUrls = housePhotos.Select(item => item.Url).ToList();
        }

        return house;
    }

    private static void ApplyTranslation(House house, SupabaseHouseTranslation translation)
    {
        switch (translation.Language)
        {
            case "ru":
                house.TitleRu = translation.Title;
                house.DescriptionRu = translation.Description;
                break;
            case "en":
                house.TitleEn = translation.Title;
                house.DescriptionEn = translation.Description;
                break;
            case "fi":
                house.TitleFi = translation.Title;
                house.DescriptionFi = translation.Description;
                break;
            default:
                house.TitleEt = translation.Title;
                house.DescriptionEt = translation.Description;
                break;
        }
    }
}
