using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("favourites")]
public class SupabaseFavourite : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("house_id")]
    public long HouseId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
