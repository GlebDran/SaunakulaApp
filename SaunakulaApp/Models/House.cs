using SQLite;

namespace SaunakulaApp.Models;

public class House
{
    [PrimaryKey]
    public string Id { get; set; } = "";
    public string TitleEt { get; set; } = "";
    public string TitleRu { get; set; } = "";
    public string TitleEn { get; set; } = "";
    public string TitleFi { get; set; } = "";
    public string DescriptionEt { get; set; } = "";
    public string DescriptionRu { get; set; } = "";
    public string DescriptionEn { get; set; } = "";
    public string DescriptionFi { get; set; } = "";
    public string Image { get; set; } = "";
    public int MaxGuests { get; set; }
    public int SizeM2 { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal Price24h { get; set; }
    public int MinHours { get; set; } = 4;

    [Ignore]
    public List<string> Amenities { get; set; } = new();

    // Возвращает название по текущему языку
    public string GetTitle(string lang) => lang switch
    {
        "ru" => TitleRu,
        "en" => TitleEn,
        "fi" => TitleFi,
        _ => TitleEt
    };

    public string GetDescription(string lang) => lang switch
    {
        "ru" => DescriptionRu,
        "en" => DescriptionEn,
        "fi" => DescriptionFi,
        _ => DescriptionEt
    };
}