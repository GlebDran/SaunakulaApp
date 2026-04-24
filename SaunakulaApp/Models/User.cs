using SQLite;

namespace SaunakulaApp.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // VIP статус
    public bool IsVip { get; set; } = false;
    public DateTime? VipGrantedAt { get; set; } = null;

    /// <summary>
    /// VIP активен если получен менее 12 месяцев назад
    /// </summary>
    [Ignore]
    public bool IsVipActive =>
        IsVip &&
        VipGrantedAt.HasValue &&
        VipGrantedAt.Value.AddYears(1) > DateTime.Now;
}
