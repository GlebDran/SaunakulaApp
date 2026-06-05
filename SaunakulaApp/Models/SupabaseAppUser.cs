using Postgrest.Attributes;
using Postgrest.Models;

namespace SaunakulaApp.Models;

[Table("app_users")]
public class SupabaseAppUser : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("is_vip")]
    public bool IsVip { get; set; }

    [Column("vip_granted_at")]
    public DateTime? VipGrantedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
