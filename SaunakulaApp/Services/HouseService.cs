using SaunakulaApp.Models;

namespace SaunakulaApp.Services;

public class HouseService
{
    private readonly List<House> _houses = new()
    {
        // ─── SOOME MAJA ───────────────────────────────────────────
        new House
        {
            Id = "soome",
            TitleEt = "Soome maja",
            TitleRu = "Финский дом",
            TitleEn = "Finnish House",
            TitleFi = "Suomalainen talo",

            DescriptionEt = "Looduslikust puidust interjöör toob Sind tagasi kahe jalaga maa peale ja aitab puhastada keha ning hinge. Sinu käsutada on ehtne puuküttega Soome saun, kus saad mõnuga vihelda ja hiljem end külma või kuuma vette kasta. Pärast saunatamist puhka kaminaga varustatud hubases toas või mõnule suurel terrassil.",
            DescriptionRu = "Интерьер из натурального дерева вернёт вас к природе и поможет очистить тело и душу. В вашем распоряжении настоящая финская сауна на дровах, после которой можно окунуться в холодную или горячую купель. Отдохните у камина или на большой террасе.",
            DescriptionEn = "A natural wood interior brings you back to earth and helps cleanse body and soul. Enjoy a real wood-fired Finnish sauna, then cool down in the cold pool or warm tub. Relax by the fireplace or on the spacious terrace.",
            DescriptionFi = "Luonnonpuinen sisustus tuo sinut takaisin maanpinnalle ja auttaa puhdistamaan kehon ja sielun. Nauti aidosta puulämmitteisestä suomalaisesta saunasta ja hyppää sen jälkeen kylmään altaaseen tai lämpimään kylpytynnyriin.",

            Image = "soome.jpg",
            MaxGuests = 20,
            SizeM2 = 85,
            PricePerHour = 60,
            Price24h = 476,
            Price24hRegular = 596,
            MinHours = 4,

            AmenitiesEt = new()
            {
                "Puuküttega Soome saun (kuni 6 inimest)",
                "Kuuma veega kümblustünn",
                "Jaheda veega bassein",
                "Üks magamistuba (2 kaheinimese voodit)",
                "Peasaal ja grillterrass koos kaminaga",
                "Köök + kohvimasin",
                "Muusikakeskus + karaoke",
                "WiFi + TV"
            },
            AmenitiesRu = new()
            {
                "Финская сауна на дровах (до 6 чел.)",
                "Горячая купель",
                "Холодный бассейн",
                "Одна спальня (2 двуспальные кровати)",
                "Гостиная и терраса с камином",
                "Кухня + кофемашина",
                "Музыкальный центр + karaoke",
                "WiFi + TV"
            },
            AmenitiesEn = new()
            {
                "Wood-fired Finnish sauna (up to 6 guests)",
                "Hot soaking tub",
                "Cold pool",
                "One bedroom (2 double beds)",
                "Living room & grill terrace with fireplace",
                "Kitchen + coffee machine",
                "Music system + karaoke",
                "WiFi + TV"
            },
            AmenitiesFi = new()
            {
                "Puulämmitteinen sauna (enintään 6 henkilöä)",
                "Kuuma kylpytynnyri",
                "Kylmä allas",
                "Yksi makuuhuone (2 parisänkyä)",
                "Olohuone ja grillikatos takalla",
                "Keittiö + kahvinkeitin",
                "Musiikkijärjestelmä + karaoke",
                "WiFi + TV"
            },
            PhotoUrls = new()
            {
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0788-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0791-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0789-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0557-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0558-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2489-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2480-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0551-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0549-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0561-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0554-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_111034-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_110628-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_110810-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_110842-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2025/06/grill-soomemaja.jpg",
            }
        },

        // ─── VENE MAJA ────────────────────────────────────────────
        new House
        {
            Id = "vene",
            TitleEt = "Vene maja",
            TitleRu = "Русский дом",
            TitleEn = "Russian House",
            TitleFi = "Venäläinen talo",

            DescriptionEt = "Saunas on 6-tonnine vene ahi, mis loob hämmastavat auru, mis tungib sügavale kehasse, parandab Su tervist ja pakub unustamatuid elamusi. Saun on ehitatud ainulaadsest 700–1000 aasta vanusest KELO puidust, mille pehme okaspuu aroom toob kõigile naeratuse näole.",
            DescriptionRu = "В сауне стоит 6-тонная русская печь, которая создаёт удивительный пар, проникающий глубоко в тело и восстанавливающий здоровье. Сауна построена из уникального дерева КЕЛО возрастом 700–1000 лет — мягкий хвойный аромат поднимет настроение каждому.",
            DescriptionEn = "The sauna features a 6-tonne Russian oven creating amazing steam that penetrates deep into the body. Built from unique 700–1000 year old KELO wood, whose soft pine aroma will put a smile on everyone's face.",
            DescriptionFi = "Saunassa on 6-tonninen venäläinen uuni, joka luo hämmästyttävää höyryä kehon syviin kerroksiin. Rakennettu ainutlaatuisesta 700–1000 vuotta vanhasta KELO-puusta, jonka pehmeä havupuun tuoksu ilahduttaa jokaista.",

            Image = "vene.jpg",
            MaxGuests = 40,
            SizeM2 = 165,
            PricePerHour = 95,
            Price24h = 672,
            Price24hRegular = 841,
            MinHours = 4,

            AmenitiesEt = new()
            {
                "6-tonnine vene ahi (kuni 10 inimest)",
                "Kuuma veega kümblustünn",
                "Jaheda veega bassein",
                "Neli magamistuba (19 magamiskohta)",
                "Peasaal, kaminasaal, grillterrass",
                "Köök + kaks külmkappi",
                "Muusikakeskus + karaoke",
                "WiFi + TV"
            },
            AmenitiesRu = new()
            {
                "6-тонная русская печь (до 10 чел.)",
                "Горячая купель",
                "Холодный бассейн",
                "Четыре спальни (19 спальных мест)",
                "Гостиная, каминный зал, гриль-терраса",
                "Кухня + два холодильника",
                "Музыкальный центр + karaoke",
                "WiFi + TV"
            },
            AmenitiesEn = new()
            {
                "6-tonne Russian oven sauna (up to 10)",
                "Hot soaking tub",
                "Cold pool",
                "Four bedrooms (19 sleeping places)",
                "Hall, fireplace room, grill terrace",
                "Kitchen + two fridges",
                "Music system + karaoke",
                "WiFi + TV"
            },
            AmenitiesFi = new()
            {
                "6-tonninen venäläinen uuni (enintään 10)",
                "Kuuma kylpytynnyri",
                "Kylmä allas",
                "Neljä makuuhuonetta (19 nukkumapaikkaa)",
                "Sali, takkahuone, grillikatos",
                "Keittiö + kaksi jääkaappia",
                "Musiikkijärjestelmä + karaoke",
                "WiFi + TV"
            },
            PhotoUrls = new()
            {
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2368-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2403-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_103732-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_103801-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102702-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/Saunum-base.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/image001-17.png",
                "https://saunakula.ee/wp-content/uploads/2024/07/image002-3.png",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0714-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_103524-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102547-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_101857_1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_101806-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102000-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102252-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102942-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102822-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0709-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_102848-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_101529-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/Saunakula-Tunn-1606912416533.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0693-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0703-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2025/06/grill-venemaja.jpg",
            }
        },

        // ─── JAHIMEHE MAJA ────────────────────────────────────────
        new House
        {
            Id = "jahimees",
            TitleEt = "Jahimehe maja",
            TitleRu = "Дом охотника",
            TitleEn = "Hunter's Lodge",
            TitleFi = "Metsästäjän talo",

            DescriptionEt = "Jahimehe maja interjööri kaunistavad trofeed ja vanad püssid, mis loovad enneolematult hubase ja seiklusrikka atmosfääri. Sauna 6-tonnine vene ahi loob hämmastavat auru. Ehitusel on kasutatud 700–1000 aasta vanust KELO puitu, mille pehme okaspuu aroom toob kõigile naeratuse näole.",
            DescriptionRu = "Интерьер дома украшен трофеями и старинными ружьями, создающими неповторимую уютную и приключенческую атмосферу. 6-тонная русская печь в сауне создаёт удивительный пар. Построен из дерева КЕЛО возрастом 700–1000 лет.",
            DescriptionEn = "The interior is decorated with trophies and old rifles creating a uniquely cozy and adventurous atmosphere. A 6-tonne Russian oven creates amazing sauna steam. Built from 700–1000 year old KELO wood.",
            DescriptionFi = "Sisustusta koristavat trofeet ja vanhat aseet, jotka luovat ainutlaatuisen kodikkaan seikkailullisen tunnelman. 6-tonninen venäläinen uuni luo hämmästyttävää saunanhöyryä. Rakennettu 700–1000 vuotta vanhasta KELO-puusta.",

            Image = "jahimees.jpg",
            MaxGuests = 40,
            SizeM2 = 120,
            PricePerHour = 95,
            Price24h = 672,
            Price24hRegular = 841,
            MinHours = 4,

            AmenitiesEt = new()
            {
                "Vene saun KELO puidust (kuni 14 inimest)",
                "Jaheda veega bassein",
                "Neli magamistuba (16 magamiskohta)",
                "Peasaal ja grillterrass",
                "Köök + kaks külmkappi",
                "Muusikakeskus + karaoke",
                "WiFi + TV",
                "Kümblustünn (lisatasu 140€)"
            },
            AmenitiesRu = new()
            {
                "Русская сауна из КЕЛО (до 14 чел.)",
                "Холодный бассейн",
                "Четыре спальни (16 спальных мест)",
                "Гостиная и гриль-терраса",
                "Кухня + два холодильника",
                "Музыкальный центр + karaoke",
                "WiFi + TV",
                "Купель (доп. 140€)"
            },
            AmenitiesEn = new()
            {
                "Russian KELO wood sauna (up to 14)",
                "Cold pool",
                "Four bedrooms (16 sleeping places)",
                "Hall and grill terrace",
                "Kitchen + two fridges",
                "Music system + karaoke",
                "WiFi + TV",
                "Hot tub (extra 140€)"
            },
            AmenitiesFi = new()
            {
                "Venäläinen KELO-puusauna (enintään 14)",
                "Kylmä allas",
                "Neljä makuuhuonetta (16 nukkumapaikkaa)",
                "Sali ja grillikatos",
                "Keittiö + kaksi jääkaappia",
                "Musiikkijärjestelmä + karaoke",
                "WiFi + TV",
                "Kylpytynnyri (lisämaksu 140€)"
            },
            PhotoUrls = new()
            {
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_2255-copy-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_110001-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0768-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_110026-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0580-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2287-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_2281-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0782-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_104825-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_9246-1-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_9232-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_105458-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_105227-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_20201008_105348-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0721-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0726-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0734-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0754-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0736-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0725-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2025/06/grill-jahimaja.jpg",
                "https://saunakula.ee/wp-content/uploads/2025/08/jahimaja-tunn.jpg",
            }
        },

        // ─── SPA MAJA ─────────────────────────────────────────────
        new House
        {
            Id = "spa",
            TitleEt = "SPA maja",
            TitleRu = "СПА дом",
            TitleEn = "SPA House",
            TitleFi = "SPA-talo",

            DescriptionEt = "Sind ootavad puuküttega Soome saun, aurusaun koos eriti pehme auruga ning mahedalt soe infrapunasaun. Kolm eri sauna ja mõnus puhkus suurele seltskonnale. Bassein, kümblustünn, karaoke — kõik ühes kohas!",
            DescriptionRu = "Вас ждут финская сауна на дровах, паровая сауна с особенно мягким паром и инфракрасная сауна. Три разные сауны и приятный отдых для большой компании. Бассейн, купель, karaoke — всё в одном месте!",
            DescriptionEn = "Three saunas await you: a wood-fired Finnish sauna, a steam room with extra soft steam, and an infrared sauna. Pool, hot tub, karaoke — everything in one place for a large group!",
            DescriptionFi = "Kolme saunaa odottaa: puulämmitteinen sauna, höyrysauna erityisen pehmeällä höyryllä ja infrapunasauna. Allas, kylpytynnyri, karaoke — kaikki yhdessä paikassa suurelle seurueelle!",

            Image = "spa.jpg",
            MaxGuests = 20,
            SizeM2 = 120,
            PricePerHour = 75,
            Price24h = 560,
            Price24hRegular = 701,
            MinHours = 4,

            AmenitiesEt = new()
            {
                "Puuküttega Soome saun",
                "Aurusaun (eriti pehme aur)",
                "Infrapunasaun",
                "Jaheda veega bassein",
                "Kümblustünn",
                "Köök + kohvimasin",
                "Muusikakeskus + karaoke",
                "WiFi + TV"
            },
            AmenitiesRu = new()
            {
                "Финская сауна на дровах",
                "Паровая сауна (особенно мягкий пар)",
                "Инфракрасная сауна",
                "Холодный бассейн",
                "Горячая купель",
                "Кухня + кофемашина",
                "Музыкальный центр + karaoke",
                "WiFi + TV"
            },
            AmenitiesEn = new()
            {
                "Wood-fired Finnish sauna",
                "Steam room (extra soft steam)",
                "Infrared sauna",
                "Cold pool",
                "Hot soaking tub",
                "Kitchen + coffee machine",
                "Music system + karaoke",
                "WiFi + TV"
            },
            AmenitiesFi = new()
            {
                "Puulämmitteinen suomalainen sauna",
                "Höyrysauna (erityisen pehmeä höyry)",
                "Infrapunasauna",
                "Kylmä allas",
                "Kuuma kylpytynnyri",
                "Keittiö + kahvinkeitin",
                "Musiikkijärjestelmä + karaoke",
                "WiFi + TV"
            },
            PhotoUrls = new()
            {
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_0571-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1003-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1006-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1022-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1030-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/image001-12.png",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_8924.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/06/IMG_8915.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1050-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1021-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1058-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1034-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1038-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1019-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1054-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2024/07/IMG_1049-scaled.jpg",
                "https://saunakula.ee/wp-content/uploads/2025/06/grill-spamaja.jpg",
                "https://saunakula.ee/wp-content/uploads/2026/02/spa-grill.jpg",
                "https://saunakula.ee/wp-content/uploads/2026/02/spa-grillsaal.jpg",
                "https://saunakula.ee/wp-content/uploads/2026/02/spamaja-grill.jpg",
            }
        },
    };

    public Task<List<House>> GetAllAsync() => Task.FromResult(_houses);

    public Task<House?> GetByIdAsync(string id)
        => Task.FromResult(_houses.FirstOrDefault(h => h.Id == id));
}