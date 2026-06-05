using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("houses")]
public class SupabaseHouse : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("app_house_id")]
    public string AppHouseId { get; set; } = string.Empty;

    [Column("image")]
    public string Image { get; set; } = string.Empty;

    [Column("max_guests")]
    public int MaxGuests { get; set; }

    [Column("size_m2")]
    public int SizeM2 { get; set; }

    [Column("price_per_hour")]
    public decimal PricePerHour { get; set; }

    [Column("price_24h")]
    public decimal Price24h { get; set; }

    [Column("price_24h_regular")]
    public decimal Price24hRegular { get; set; }

    [Column("min_hours")]
    public int MinHours { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}
