using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("house_amenities")]
public class SupabaseHouseAmenity : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("house_id")]
    public long HouseId { get; set; }

    [Column("language")]
    public string Language { get; set; } = string.Empty;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("text")]
    public string Text { get; set; } = string.Empty;
}
