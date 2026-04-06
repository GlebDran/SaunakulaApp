using SQLite;

namespace SaunakulaApp.Models;

public class Favourite
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string HouseId { get; set; } = "";
    public DateTime AddedAt { get; set; } = DateTime.Now;
}