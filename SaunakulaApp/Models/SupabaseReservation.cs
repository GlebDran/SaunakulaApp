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

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }
}
