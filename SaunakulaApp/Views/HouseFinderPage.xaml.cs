using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.Views;

public partial class HouseFinderPage : ContentPage
{
    private readonly HouseService _houseService;
    private readonly SessionService _session;

    private int _currentStep = 1;
    private string _answer1 = ""; // small / medium / large
    private string _answer2 = ""; // saun / pool / spa
    private string _answer3 = ""; // budget / mid / premium
    private House? _resultHouse;

    public HouseFinderPage(HouseService houseService, SessionService session)
    {
        InitializeComponent();
        _houseService = houseService;
        _session = session;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        ResetQuiz();
    }

    private void ApplyLocalization()
    {
        var lang = _session.Language;

        PageTitleLabel.Text = lang switch
        {
            "ru" => "🏡 Найди подходящий дом",
            "en" => "🏡 Find your house",
            "fi" => "🏡 Löydä sopiva talo",
            _ => "🏡 Leia sobiv maja"
        };
        PageSubtitleLabel.Text = lang switch
        {
            "ru" => "Ответь на 3 вопроса",
            "en" => "Answer 3 questions",
            "fi" => "Vastaa 3 kysymykseen",
            _ => "Vasta 3 küsimusele"
        };

        // Q1
        Q1Label.Text = lang switch
        {
            "ru" => "Сколько гостей придёт?",
            "en" => "How many guests?",
            "fi" => "Kuinka monta vierasta?",
            _ => "Kui palju külalisi tuleb?"
        };
        Q1SubLabel.Text = lang switch
        {
            "ru" => "Выбери подходящий диапазон",
            "en" => "Choose a range",
            "fi" => "Valitse sopiva määrä",
            _ => "Vali sobiv vahemik"
        };
        Opt1_1_Title.Text = lang switch { "ru" => "Маленькая группа", "en" => "Small group", "fi" => "Pieni ryhmä", _ => "Väike grupp" };
        Opt1_1_Sub.Text = lang switch { "ru" => "до 20 гостей", "en" => "up to 20 guests", "fi" => "enintään 20 vierasta", _ => "kuni 20 inimest" };
        Opt1_2_Title.Text = lang switch { "ru" => "Средняя группа", "en" => "Medium group", "fi" => "Keskikokoinen ryhmä", _ => "Keskmine grupp" };
        Opt1_2_Sub.Text = lang switch { "ru" => "20–30 гостей", "en" => "20–30 guests", "fi" => "20–30 vierasta", _ => "20–30 inimest" };
        Opt1_3_Title.Text = lang switch { "ru" => "Большая группа", "en" => "Large group", "fi" => "Suuri ryhmä", _ => "Suur grupp" };
        Opt1_3_Sub.Text = lang switch { "ru" => "30+ гостей", "en" => "30+ guests", "fi" => "30+ vierasta", _ => "30+ inimest" };

        // Q2
        Q2Label.Text = lang switch
        {
            "ru" => "Что важнее всего?",
            "en" => "What matters most?",
            "fi" => "Mikä on tärkeintä?",
            _ => "Mis on kõige tähtsam?"
        };
        Q2SubLabel.Text = lang switch { "ru" => "Выбери приоритет", "en" => "Choose priority", "fi" => "Valitse prioriteetti", _ => "Vali oma prioriteet" };
        Opt2_1_Title.Text = lang switch { "ru" => "Настоящая сауна", "en" => "Authentic sauna", "fi" => "Aito sauna", _ => "Ehtne saun" };
        Opt2_1_Sub.Text = lang switch { "ru" => "Дровяная сауна, аутентичный опыт", "en" => "Wood-fired, authentic experience", "fi" => "Puulämmitteinen, aito kokemus", _ => "Puidust saun, autentne kogemus" };
        Opt2_2_Title.Text = lang switch { "ru" => "Бассейн и купель", "en" => "Pool & hot tub", "fi" => "Allas ja kylpytynnyri", _ => "Bassein ja tünn" };
        Opt2_2_Sub.Text = lang switch { "ru" => "Холодный бассейн, горячая купель", "en" => "Cold pool, hot tub", "fi" => "Kylmä allas, kuuma kylpytynnyri", _ => "Külm bassein, kuum kümblustünn" };
        Opt2_3_Title.Text = lang switch { "ru" => "Полный СПА", "en" => "Full SPA", "fi" => "Täydellinen SPA", _ => "Täielik SPA" };
        Opt2_3_Sub.Text = lang switch { "ru" => "3 разные сауны, максимальное расслабление", "en" => "3 saunas, maximum relaxation", "fi" => "3 saunaa, maksimaalinen rentoutuminen", _ => "3 erinevat sauna, maksimaalne lõõgastus" };

        // Q3
        Q3Label.Text = lang switch
        {
            "ru" => "Какой бюджет?",
            "en" => "What is your budget?",
            "fi" => "Mikä on budjettisi?",
            _ => "Milline on eelarve?"
        };
        Q3SubLabel.Text = lang switch { "ru" => "Цена за 24ч", "en" => "Price per 24h", "fi" => "Hinta 24h", _ => "24h hind" };
        Opt3_1_Title.Text = lang switch { "ru" => "Экономный", "en" => "Budget", "fi" => "Säästävä", _ => "Säästlik" };
        Opt3_1_Sub.Text = lang switch { "ru" => "до 500€", "en" => "up to 500€", "fi" => "enintään 500€", _ => "kuni 500€" };
        Opt3_2_Title.Text = lang switch { "ru" => "Средний", "en" => "Mid-range", "fi" => "Keskitaso", _ => "Keskmine" };
        Opt3_2_Sub.Text = "500–600€";
        Opt3_3_Title.Text = lang switch { "ru" => "Лучший опыт", "en" => "Best experience", "fi" => "Paras kokemus", _ => "Parim kogemus" };
        Opt3_3_Sub.Text = "600€+";

        // Buttons
        NextButton.Text = lang switch { "ru" => "Далее →", "en" => "Next →", "fi" => "Seuraava →", _ => "Järgmine →" };
        RetryButton.Text = lang switch { "ru" => "↩ Начать заново", "en" => "↩ Start over", "fi" => "↩ Aloita alusta", _ => "↩ Alusta uuesti" };
        ResultTitleLabel.Text = lang switch { "ru" => "🎉 Наша рекомендация!", "en" => "🎉 Our recommendation!", "fi" => "🎉 Suosituksemme!", _ => "🎉 Meie soovitus!" };
        ResultDetailsButton.Text = lang switch { "ru" => "Подробнее", "en" => "Details", "fi" => "Lisätietoja", _ => "Vaata lähemalt" };
        ResultBookButton.Text = lang switch { "ru" => "Забронировать", "en" => "Book now", "fi" => "Varaa nyt", _ => "Broneeri" };
    }

    private void ResetQuiz()
    {
        _currentStep = 1;
        _answer1 = _answer2 = _answer3 = "";
        _resultHouse = null;

        Step1View.IsVisible = true;
        Step2View.IsVisible = false;
        Step3View.IsVisible = false;
        ResultView.IsVisible = false;
        BottomBar.IsVisible = true;
        NextButton.IsEnabled = false;

        UpdateProgressBar();
        ClearSelections();
    }

    private void ClearSelections()
    {
        var frames = new[] { Opt1_1, Opt1_2, Opt1_3, Opt2_1, Opt2_2, Opt2_3, Opt3_1, Opt3_2, Opt3_3 };
        foreach (var f in frames)
            f.BackgroundColor = Color.FromArgb("#FFFFFF");

        var checks = new[] { Check1_1, Check1_2, Check1_3, Check2_1, Check2_2, Check2_3, Check3_1, Check3_2, Check3_3 };
        foreach (var c in checks)
            c.Text = "";
    }

    private void UpdateProgressBar()
    {
        var active = Color.FromArgb("#5A7C5E");
        var inactive = Color.FromArgb("#E8EDE7");
        Step1Bar.BackgroundColor = _currentStep >= 1 ? active : inactive;
        Step2Bar.BackgroundColor = _currentStep >= 2 ? active : inactive;
        Step3Bar.BackgroundColor = _currentStep >= 3 ? active : inactive;
    }

    // ── Q1 handlers ───────────────────────────────────────────

    private void Q1_Small_Tapped(object sender, TappedEventArgs e) => SelectQ1("small", Opt1_1, Check1_1, new[] { Opt1_2, Opt1_3 }, new[] { Check1_2, Check1_3 });
    private void Q1_Medium_Tapped(object sender, TappedEventArgs e) => SelectQ1("medium", Opt1_2, Check1_2, new[] { Opt1_1, Opt1_3 }, new[] { Check1_1, Check1_3 });
    private void Q1_Large_Tapped(object sender, TappedEventArgs e) => SelectQ1("large", Opt1_3, Check1_3, new[] { Opt1_1, Opt1_2 }, new[] { Check1_1, Check1_2 });

    private void SelectQ1(string answer, Frame selected, Label check, Frame[] others, Label[] otherChecks)
    {
        _answer1 = answer;
        SelectOption(selected, check, others, otherChecks);
        NextButton.IsEnabled = true;
    }

    // ── Q2 handlers ───────────────────────────────────────────

    private void Q2_Saun_Tapped(object sender, TappedEventArgs e) => SelectQ2("saun", Opt2_1, Check2_1, new[] { Opt2_2, Opt2_3 }, new[] { Check2_2, Check2_3 });
    private void Q2_Pool_Tapped(object sender, TappedEventArgs e) => SelectQ2("pool", Opt2_2, Check2_2, new[] { Opt2_1, Opt2_3 }, new[] { Check2_1, Check2_3 });
    private void Q2_Spa_Tapped(object sender, TappedEventArgs e) => SelectQ2("spa", Opt2_3, Check2_3, new[] { Opt2_1, Opt2_2 }, new[] { Check2_1, Check2_2 });

    private void SelectQ2(string answer, Frame selected, Label check, Frame[] others, Label[] otherChecks)
    {
        _answer2 = answer;
        SelectOption(selected, check, others, otherChecks);
        NextButton.IsEnabled = true;
    }

    // ── Q3 handlers ───────────────────────────────────────────

    private void Q3_Budget_Tapped(object sender, TappedEventArgs e) => SelectQ3("budget", Opt3_1, Check3_1, new[] { Opt3_2, Opt3_3 }, new[] { Check3_2, Check3_3 });
    private void Q3_Mid_Tapped(object sender, TappedEventArgs e) => SelectQ3("mid", Opt3_2, Check3_2, new[] { Opt3_1, Opt3_3 }, new[] { Check3_1, Check3_3 });
    private void Q3_Premium_Tapped(object sender, TappedEventArgs e) => SelectQ3("premium", Opt3_3, Check3_3, new[] { Opt3_1, Opt3_2 }, new[] { Check3_1, Check3_2 });

    private void SelectQ3(string answer, Frame selected, Label check, Frame[] others, Label[] otherChecks)
    {
        _answer3 = answer;
        SelectOption(selected, check, others, otherChecks);
        NextButton.IsEnabled = true;
    }

    private void SelectOption(Frame selected, Label check, Frame[] others, Label[] otherChecks)
    {
        selected.BackgroundColor = Color.FromArgb("#E8EDE7");
        check.Text = "✅";
        for (int i = 0; i < others.Length; i++)
        {
            others[i].BackgroundColor = Color.FromArgb("#FFFFFF");
            otherChecks[i].Text = "";
        }
    }

    // ── Navigation ────────────────────────────────────────────

    private async void Next_Clicked(object sender, EventArgs e)
    {
        if (_currentStep == 1)
        {
            _currentStep = 2;
            Step1View.IsVisible = false;
            Step2View.IsVisible = true;
            NextButton.IsEnabled = !string.IsNullOrEmpty(_answer2);
        }
        else if (_currentStep == 2)
        {
            _currentStep = 3;
            Step2View.IsVisible = false;
            Step3View.IsVisible = true;
            NextButton.IsEnabled = !string.IsNullOrEmpty(_answer3);

            var lang = _session.Language;
            NextButton.Text = lang switch { "ru" => "Получить результат ✨", "en" => "Get result ✨", "fi" => "Näytä tulos ✨", _ => "Näita tulemust ✨" };
        }
        else if (_currentStep == 3)
        {
            await ShowResult();
        }

        UpdateProgressBar();
    }

    private async Task ShowResult()
    {
        var houses = await _houseService.GetAllAsync();
        var lang = _session.Language;

        // Логика рекомендации
        _resultHouse = GetRecommendation(houses);

        if (_resultHouse is null) return;

        Step3View.IsVisible = false;
        ResultView.IsVisible = true;
        BottomBar.IsVisible = false;

        ResultImage.Source = _resultHouse.Image;
        ResultHouseLabel.Text = _resultHouse.GetTitle(lang);
        ResultReasonLabel.Text = GetReason(lang);
    }

    private House GetRecommendation(List<House> houses)
    {
        // Алгоритм подбора по ответам
        // SPA maja — маленькая группа + spa приоритет
        if (_answer2 == "spa")
            return houses.First(h => h.Id == "spa");

        // Soome maja — маленькая группа + бюджет
        if (_answer1 == "small" && _answer3 == "budget")
            return houses.First(h => h.Id == "soome");

        // Vene maja — средняя/большая группа + pool или premium
        if ((_answer1 == "medium" || _answer1 == "large") && (_answer2 == "pool" || _answer3 == "premium"))
            return houses.First(h => h.Id == "vene");

        // Jahimehe maja — большая группа
        if (_answer1 == "large")
            return houses.First(h => h.Id == "jahimees");

        // Soome maja — по умолчанию для маленькой группы
        if (_answer1 == "small")
            return houses.First(h => h.Id == "soome");

        // Vene maja — по умолчанию для средней группы
        return houses.First(h => h.Id == "vene");
    }

    private string GetReason(string lang)
    {
        if (_resultHouse is null) return "";

        return (_resultHouse.Id, lang) switch
        {
            ("soome", "ru") => "Идеально для небольшой компании! Настоящая финская сауна, купель и уютная терраса с камином.",
            ("soome", "en") => "Perfect for a small group! Real Finnish sauna, hot tub and cozy terrace with fireplace.",
            ("soome", "fi") => "Täydellinen pienelle ryhmälle! Aito suomalainen sauna, kylpytynnyri ja tunnelmallinen terassi takalla.",
            ("soome", _) => "Ideaalne väikesele grupile! Ehtne Soome saun, kümblustünn ja hubane kaminaterrass.",

            ("vene", "ru") => "Отличный выбор для большой компании! 6-тонная русская печь из дерева КЕЛО — незабываемый опыт.",
            ("vene", "en") => "Great choice for a large group! 6-tonne Russian oven from KELO wood — unforgettable experience.",
            ("vene", "fi") => "Hieno valinta suurelle ryhmälle! 6-tonninen KELO-puinen venäläinen uuni — unohtumaton kokemus.",
            ("vene", _) => "Suurepärane valik suurele grupile! 6-tonnine KELO puidust vene ahi — unustamatu kogemus.",

            ("jahimees", "ru") => "Для большой компании с приключенческой атмосферой! Трофеи и старинные ружья создают уникальный уют.",
            ("jahimees", "en") => "For large groups seeking adventure! Trophies and old rifles create a unique cozy atmosphere.",
            ("jahimees", "fi") => "Suurelle seurueelle seikkailullisella tunnelmalla! Trofeet ja vanhat aseet luovat ainutlaatuisen viihtyisyyden.",
            ("jahimees", _) => "Suurele grupile seiklusrikka atmosfääriga! Trofeed ja vanad püssid loovad ainulaadse hubase õhkkonna.",

            ("spa", "ru") => "Максимальное расслабление! Три разные сауны — финская, паровая и инфракрасная. Идеально для СПА-дня.",
            ("spa", "en") => "Maximum relaxation! Three different saunas — Finnish, steam and infrared. Perfect for a SPA day.",
            ("spa", "fi") => "Maksimaalinen rentoutuminen! Kolme eri saunaa — suomalainen, höyry- ja infrapunasauna. Täydellinen SPA-päivälle.",
            ("spa", _) => "Maksimaalne lõõgastus! Kolm erinevat sauna — Soome, auru- ja infrapunasaun. Ideaalne SPA-päevaks.",

            _ => ""
        };
    }

    // ── Result actions ────────────────────────────────────────

    private async void ResultDetails_Clicked(object sender, EventArgs e)
    {
        if (_resultHouse is null) return;
        await Shell.Current.GoToAsync(
            $"{nameof(HouseDetailsPage)}?houseId={_resultHouse.Id}");
    }

    private async void ResultBook_Clicked(object sender, EventArgs e)
    {
        if (_resultHouse is null) return;
        if (_session.IsLoggedIn)
            await Shell.Current.GoToAsync(
                $"{nameof(BookingPage)}?houseId={_resultHouse.Id}");
        else
            await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private void Retry_Clicked(object sender, EventArgs e)
    {
        ApplyLocalization();
        ResetQuiz();
    }

    private void Back_Tapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("..");
}
