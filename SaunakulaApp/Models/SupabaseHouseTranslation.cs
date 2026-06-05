using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("house_translations")]
public class SupabaseHouseTranslation : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("house_id")]
    public long HouseId { get; set; }

    [Column("language")]
    public string Language { get; set; } = string.Empty;

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;
}
