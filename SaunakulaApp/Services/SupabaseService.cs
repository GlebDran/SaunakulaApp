using System.Text.Json;
using static Postgrest.Constants;
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

    public async Task<SupabaseAppUser?> GetAppUserByEmailAsync(string email)
    {
        var response = await _supabaseClient
            .From<SupabaseAppUser>()
            .Filter("email", Operator.Equals, email.Trim())
            .Get();

        return response.Models?.FirstOrDefault();
    }

    public async Task<SupabaseAppUser?> GetAppUserByIdAsync(int id)
    {
        var response = await _supabaseClient
            .From<SupabaseAppUser>()
            .Filter("id", Operator.Equals, id.ToString())
            .Get();

        return response.Models?.FirstOrDefault();
    }

    public async Task<SupabaseAppUser> RegisterAppUserAsync(User user)
    {
        var email = user.Email.Trim();
        var existing = await GetAppUserByEmailAsync(email);
        if (existing is not null)
            throw new InvalidOperationException("This email is already registered.");

        var remoteUser = new SupabaseAppUser
        {
            FullName = user.FullName.Trim(),
            Email = email,
            PasswordHash = user.PasswordHash,
            Phone = user.Phone.Trim(),
            IsVip = user.IsVip,
            VipGrantedAt = user.VipGrantedAt,
            CreatedAt = DateTime.UtcNow
        };

        await _supabaseClient
            .From<SupabaseAppUser>()
            .Insert(remoteUser);

        return await GetAppUserByEmailAsync(email)
            ?? throw new InvalidOperationException("User was not found after registration.");
    }

    public async Task<SupabaseAppUser> UpdateAppUserAsync(User user)
    {
        var remoteUser = await GetAppUserByIdAsync(user.Id)
            ?? throw new InvalidOperationException($"User '{user.Id}' does not exist in the central database.");

        remoteUser.FullName = user.FullName.Trim();
        remoteUser.Email = user.Email.Trim();
        remoteUser.PasswordHash = user.PasswordHash;
        remoteUser.Phone = user.Phone.Trim();
        remoteUser.IsVip = user.IsVip;
        remoteUser.VipGrantedAt = user.VipGrantedAt;

        await _supabaseClient.From<SupabaseAppUser>().Update(remoteUser);
        return remoteUser;
    }

    public async Task<User?> LoginAppUserAsync(string email, string passwordHash)
    {
        var remoteUser = await GetAppUserByEmailAsync(email);
        if (remoteUser is null || remoteUser.PasswordHash != passwordHash)
            return null;

        return ToLocalUser(remoteUser);
    }

    public async Task<(int Count, bool IsVip)> CheckAndUpdateVipAsync(int userId)
    {
        var user = await GetAppUserByIdAsync(userId);
        if (user is null)
            return (0, false);

        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        var reservations = await GetReservationsByUserAsync(userId);
        var count = reservations.Count(reservation =>
            reservation.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase) &&
            (reservation.CreatedAt == default || reservation.CreatedAt >= oneYearAgo));

        if (user.IsVip &&
            user.VipGrantedAt.HasValue &&
            user.VipGrantedAt.Value.AddYears(1) <= DateTime.UtcNow)
        {
            user.IsVip = false;
            user.VipGrantedAt = null;
            await _supabaseClient.From<SupabaseAppUser>().Update(user);
        }

        if (count >= 10 && !user.IsVip)
        {
            user.IsVip = true;
            user.VipGrantedAt = DateTime.UtcNow;
            await _supabaseClient.From<SupabaseAppUser>().Update(user);
        }

        return (count, user.IsVip && user.VipGrantedAt.HasValue && user.VipGrantedAt.Value.AddYears(1) > DateTime.UtcNow);
    }

    public async Task<bool> BookHouseAsync(
        string houseId,
        string customerName,
        DateTime start,
        DateTime end,
        int guestCount = 1,
        decimal totalPrice = 0,
        string notes = "",
        int? userId = null)
    {
        if (end <= start)
            throw new ArgumentException("End date must be later than start date.");

        var booking = new Booking
        {
            UserId = userId ?? 0,
            HouseId = houseId,
            StartDateTime = start,
            EndDateTime = end,
            GuestCount = guestCount,
            TotalPrice = totalPrice,
            Notes = notes,
            Status = "Confirmed"
        };

        var reservation = await CreateReservationAsync(booking, customerName);
        return reservation is not null;
    }

    public async Task<SupabaseReservation?> CreateReservationAsync(Booking booking, string? customerName = null)
    {
        if (booking.EndDateTime <= booking.StartDateTime)
            throw new ArgumentException("End date must be later than start date.");

        var remoteHouseId = await ResolveRemoteHouseIdAsync(booking.HouseId);
        var startDate = booking.StartDateTime.Date;
        var endDate = booking.EndDateTime.Date;

        var existingReservations = await GetReservationsForHouseAsync(remoteHouseId);
        var isOverlapping = existingReservations.Any(reservation =>
            reservation.Status == "confirmed" &&
            startDate < reservation.EndDate.Date &&
            endDate > reservation.StartDate.Date);

        if (isOverlapping)
            return null;

        var user = booking.UserId > 0 ? await GetAppUserByIdAsync(booking.UserId) : null;
        var resolvedCustomerName = !string.IsNullOrWhiteSpace(customerName)
            ? customerName.Trim()
            : user?.FullName ?? $"User {booking.UserId}";

        var reservation = await InsertReservationAsync(
            remoteHouseId,
            booking.UserId > 0 ? booking.UserId : null,
            resolvedCustomerName,
            startDate,
            endDate,
            booking.GuestCount,
            booking.TotalPrice,
            booking.Notes ?? string.Empty);

        if (reservation is not null)
            await InsertReservationAddonsAsync(reservation.Id, booking.AddonsJson);

        return reservation;
    }

    public async Task<List<Booking>> GetBookingsByUserAsync(int userId)
    {
        var reservations = await GetReservationsByUserAsync(userId);
        var houseMap = await GetHouseAppIdsByRemoteIdAsync();

        return reservations
            .Select(reservation => ToBooking(reservation, houseMap))
            .OrderByDescending(booking => booking.StartDateTime)
            .ToList();
    }

    public async Task<int> CancelReservationAsync(long reservationId)
    {
        var response = await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("id", Operator.Equals, reservationId.ToString())
            .Get();

        var reservation = response.Models?.FirstOrDefault();
        if (reservation is null)
            return 0;

        reservation.Status = "cancelled";
        await _supabaseClient.From<SupabaseReservation>().Update(reservation);
        return 1;
    }

    public async Task<bool> IsHouseBookedAsync(
        string houseId,
        DateTime start,
        DateTime end,
        int? excludeBookingId = null)
    {
        var reservations = await GetReservationsForHouseAsync(houseId);
        return reservations.Any(reservation =>
            reservation.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase) &&
            (!excludeBookingId.HasValue || reservation.Id != excludeBookingId.Value) &&
            start.Date < reservation.EndDate.Date &&
            end.Date > reservation.StartDate.Date);
    }

    public async Task<List<(DateTime Start, DateTime End)>> GetBookedPeriodsAsync(string houseId)
    {
        var reservations = await GetReservationsForHouseAsync(houseId);
        return reservations
            .Where(reservation => reservation.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase))
            .Select(reservation => (reservation.StartDate, reservation.EndDate))
            .ToList();
    }

    public async Task<List<Favourite>> GetFavouritesByUserAsync(int userId)
    {
        var response = await _supabaseClient
            .From<SupabaseFavourite>()
            .Filter("user_id", Operator.Equals, userId.ToString())
            .Get();

        var houseMap = await GetHouseAppIdsByRemoteIdAsync();

        return (response.Models ?? new List<SupabaseFavourite>())
            .Select(item => ToFavourite(item, houseMap))
            .ToList();
    }

    public async Task<bool> IsFavouriteAsync(int userId, string houseId)
    {
        var remoteHouseId = await ResolveRemoteHouseIdAsync(houseId);
        var response = await _supabaseClient
            .From<SupabaseFavourite>()
            .Filter("user_id", Operator.Equals, userId.ToString())
            .Filter("house_id", Operator.Equals, remoteHouseId.ToString())
            .Get();

        return response.Models?.Any() == true;
    }

    public async Task ToggleFavouriteAsync(int userId, string houseId)
    {
        var remoteHouseId = await ResolveRemoteHouseIdAsync(houseId);
        var response = await _supabaseClient
            .From<SupabaseFavourite>()
            .Filter("user_id", Operator.Equals, userId.ToString())
            .Filter("house_id", Operator.Equals, remoteHouseId.ToString())
            .Get();

        var existing = response.Models?.FirstOrDefault();
        if (existing is not null)
        {
            await _supabaseClient
                .From<SupabaseFavourite>()
                .Filter("id", Operator.Equals, existing.Id.ToString())
                .Delete();
            return;
        }

        await _supabaseClient.From<SupabaseFavourite>().Insert(new SupabaseFavourite
        {
            UserId = userId,
            HouseId = remoteHouseId,
            CreatedAt = DateTime.UtcNow
        });
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

    public async Task<IReadOnlyList<SupabaseReservation>> GetReservationsByUserAsync(int userId)
    {
        var response = await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("user_id", Operator.Equals, userId.ToString())
            .Get();

        return response.Models ?? new List<SupabaseReservation>();
    }

    public async Task DeleteReservationAsync(long reservationId)
        => await CancelReservationAsync(reservationId);

    private async Task<IReadOnlyList<SupabaseReservation>> GetReservationsForHouseAsync(long remoteHouseId)
    {
        var response = await _supabaseClient
            .From<SupabaseReservation>()
            .Filter("house_id", Operator.Equals, remoteHouseId.ToString())
            .Filter("status", Operator.Equals, "confirmed")
            .Get();

        return response.Models ?? new List<SupabaseReservation>();
    }

    private async Task<SupabaseReservation?> InsertReservationAsync(
        long remoteHouseId,
        int? userId,
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
            UserId = userId,
            CustomerName = customerName,
            StartDate = start,
            EndDate = end,
            GuestCount = guestCount,
            TotalPrice = totalPrice,
            Notes = notes,
            Status = "confirmed",
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var response = await _supabaseClient.From<SupabaseReservation>().Insert(reservation);
            return response.Models?.FirstOrDefault() ?? reservation;
        }
        catch (Exception ex) when (ex.Message.Contains("reservations_no_overlap", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
    }

    private async Task InsertReservationAddonsAsync(long reservationId, string addonsJson)
    {
        if (reservationId <= 0 || string.IsNullOrWhiteSpace(addonsJson))
            return;

        List<BookingAddonPayload>? addons;
        try
        {
            addons = JsonSerializer.Deserialize<List<BookingAddonPayload>>(addonsJson);
        }
        catch (JsonException)
        {
            return;
        }

        if (addons is null || addons.Count == 0)
            return;

        foreach (var addon in addons.Where(item => !string.IsNullOrWhiteSpace(item.Id) && item.Count > 0))
        {
            await _supabaseClient.From<SupabaseReservationAddon>().Insert(new SupabaseReservationAddon
            {
                ReservationId = reservationId,
                AddonId = addon.Id,
                Quantity = addon.Count,
                UnitPrice = addon.Price,
                CreatedAt = DateTime.UtcNow
            });
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

    private async Task<IReadOnlyDictionary<long, string>> GetHouseAppIdsByRemoteIdAsync()
    {
        var response = await _supabaseClient.From<SupabaseHouse>().Get();
        return (response.Models ?? new List<SupabaseHouse>())
            .ToDictionary(house => house.Id, house => house.AppHouseId);
    }

    public static User ToLocalUser(SupabaseAppUser source)
    {
        return new User
        {
            Id = checked((int)source.Id),
            FullName = source.FullName,
            Email = source.Email,
            PasswordHash = source.PasswordHash,
            Phone = source.Phone,
            IsVip = source.IsVip,
            VipGrantedAt = source.VipGrantedAt,
            CreatedAt = source.CreatedAt == default ? DateTime.Now : source.CreatedAt
        };
    }

    private static Booking ToBooking(SupabaseReservation source, IReadOnlyDictionary<long, string> houseMap)
    {
        return new Booking
        {
            Id = checked((int)source.Id),
            UserId = checked((int)(source.UserId ?? 0)),
            HouseId = houseMap.TryGetValue(source.HouseId, out var appHouseId) ? appHouseId : source.HouseId.ToString(),
            StartDateTime = source.StartDate,
            EndDateTime = source.EndDate,
            GuestCount = source.GuestCount,
            TotalPrice = source.TotalPrice,
            Notes = source.Notes,
            Status = source.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase) ? "Cancelled" : "Confirmed",
            CreatedAt = source.CreatedAt == default ? DateTime.Now : source.CreatedAt
        };
    }

    private static Favourite ToFavourite(SupabaseFavourite source, IReadOnlyDictionary<long, string> houseMap)
    {
        return new Favourite
        {
            Id = checked((int)source.Id),
            UserId = checked((int)source.UserId),
            HouseId = houseMap.TryGetValue(source.HouseId, out var appHouseId) ? appHouseId : source.HouseId.ToString(),
            AddedAt = source.CreatedAt == default ? DateTime.Now : source.CreatedAt
        };
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

    private sealed class BookingAddonPayload
    {
        public string Id { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
