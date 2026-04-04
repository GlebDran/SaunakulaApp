using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class HouseService
{
    private readonly List<House> _houses = new()
    {
        new House
        {
            Id = "soome",
            TitleEt = "Soome maja",
            TitleRu = "Финский дом",
            TitleEn = "Finnish House",
            TitleFi = "Suomalainen talo",
            DescriptionEt = "Looduslikust puidust interjöör toob Sind tagasi kahe jalaga maa peale ja aitab puhastada keha ning hinge.",
            DescriptionRu = "Интерьер из натурального дерева вернёт вас к природе и поможет очистить тело и душу.",
            DescriptionEn = "A natural wood interior brings you back to earth and helps cleanse body and soul.",
            DescriptionFi = "Luonnonpuinen sisustus tuo sinut takaisin maanpinnalle ja auttaa puhdistamaan kehon ja sielun.",
            Image = "soome.jpg",
            MaxGuests = 20, SizeM2 = 85,
            PricePerHour = 60, Price24h = 476, MinHours = 4,
            Amenities = new() { "Soome saun / Финская сауна", "Kümblustünn / Купель", "Bassein / Бассейн", "Köök / Кухня", "Karaoke / WiFi" }
        },
        new House
        {
            Id = "vene",
            TitleEt = "Vene maja",
            TitleRu = "Русский дом",
            TitleEn = "Russian House",
            TitleFi = "Venäläinen talo",
            DescriptionEt = "Saunas on 6-tonnine vene ahi, mis loob hämmastavat auru.",
            DescriptionRu = "В сауне стоит 6-тонная русская печь, создающая удивительный пар.",
            DescriptionEn = "The sauna has a 6-tonne Russian oven that creates amazing steam.",
            DescriptionFi = "Saunassa on 6-tonninen venäläinen uuni, joka luo hämmästyttävää höyryä.",
            Image = "vene.jpg",
            MaxGuests = 20, SizeM2 = 90,
            PricePerHour = 55, Price24h = 450, MinHours = 4,
            Amenities = new() { "Vene saun / Русская баня", "Kümblustünn / Купель", "Bassein / Бассейн", "Köök / Кухня", "WiFi" }
        },
        new House
        {
            Id = "jahimees",
            TitleEt = "Jahimehe maja",
            TitleRu = "Дом охотника",
            TitleEn = "Hunter's House",
            TitleFi = "Metsästäjän talo",
            DescriptionEt = "Interjööri kaunistavad trofeed ja vanad püssid.",
            DescriptionRu = "Интерьер украшен трофеями и старинными ружьями.",
            DescriptionEn = "The interior is decorated with trophies and old rifles.",
            DescriptionFi = "Sisustusta koristavat trofeet ja vanhat aseet.",
            Image = "jahimees.jpg",
            MaxGuests = 20, SizeM2 = 80,
            PricePerHour = 50, Price24h = 420, MinHours = 4,
            Amenities = new() { "Saun / Сауна", "Kümblustünn / Купель", "Grill / Гриль", "Köök / Кухня", "WiFi" }
        },
        new House
        {
            Id = "spa",
            TitleEt = "SPA maja",
            TitleRu = "СПА дом",
            TitleEn = "SPA House",
            TitleFi = "SPA-talo",
            DescriptionEt = "Kolm eri sauna ja mõnus puhkus suurele seltskonnale.",
            DescriptionRu = "Три разные сауны и приятный отдых для большой компании.",
            DescriptionEn = "Three different saunas and a pleasant holiday for large groups.",
            DescriptionFi = "Kolme erilaista saunaa ja mukava loma suurelle seurueelle.",
            Image = "spa.jpg",
            MaxGuests = 20, SizeM2 = 120,
            PricePerHour = 70, Price24h = 596, MinHours = 4,
            Amenities = new() { "Soome saun", "Aurusaun / Паровая", "Infrapunasaun / ИК-сауна", "Bassein", "Kümblustünn", "Karaoke", "WiFi" }
        },
    };

    public Task<List<House>> GetAllAsync() => Task.FromResult(_houses);
    public Task<House?> GetByIdAsync(string id)
        => Task.FromResult(_houses.FirstOrDefault(h => h.Id == id));
}