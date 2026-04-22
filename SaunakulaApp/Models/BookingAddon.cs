namespace SaunakulaApp.Models;

public class BookingAddon
{
    public string Id { get; set; } = "";
    public string NameEt { get; set; } = "";
    public string NameRu { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string NameFi { get; set; } = "";
    public decimal Price { get; set; }
    public string Icon { get; set; } = "";
    public bool IsSelected { get; set; } = false;

    public string GetName(string lang) => lang switch
    {
        "ru" => NameRu,
        "en" => NameEn,
        "fi" => NameFi,
        _ => NameEt
    };
}