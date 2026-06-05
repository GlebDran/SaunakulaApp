using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("house_photos")]
public class SupabaseHousePhoto : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("house_id")]
    public long HouseId { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("url")]
    public string Url { get; set; } = string.Empty;
}
