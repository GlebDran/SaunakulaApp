using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("reservation_addons")]
public class SupabaseReservationAddon : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("reservation_id")]
    public long ReservationId { get; set; }

    [Column("addon_id")]
    public string AddonId { get; set; } = string.Empty;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
