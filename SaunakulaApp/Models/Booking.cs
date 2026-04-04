using SQLite;

namespace SaunakulaApp.Models;

public class Booking
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string HouseId { get; set; } = "";
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Confirmed";
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Ignore] public string HouseTitle { get; set; } = "";
    [Ignore] public string HouseImage { get; set; } = "";
}