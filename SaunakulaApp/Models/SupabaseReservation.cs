using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("reservations")]
public class SupabaseReservation : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("house_id")]
    public long HouseId { get; set; }

    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty;

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("guest_count")]
    public int GuestCount { get; set; }

    [Column("total_price")]
    public decimal TotalPrice { get; set; }

    [Column("notes")]
    public string Notes { get; set; } = string.Empty;

    [Column("status")]
    public string Status { get; set; } = "confirmed";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
