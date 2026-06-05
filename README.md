# SAUNAKULAAPP

SAUNAKULAAPP on .NET MAUI abil loodud mobiilirakenduse prototüüp Saunaküla puhkemajade ja saunade tutvustamiseks, hindade vaatamiseks ning broneeringute tegemiseks.

Rakendus valmis õppeprojekti raames. Projekti eesmärk oli luua praktiline ja töötav mobiilirakenduse prototüüp, mis näitab, kuidas Saunaküla teenuse info, kasutajakontod, broneerimine ja lisateenuste valik võiksid töötada reaalses rakenduses.

## Projekti eesmärk

Projekti eesmärk oli luua mobiilirakenduse prototüüp, kus kasutaja saab:

- vaadata Saunaküla puhkemaju ja saunu;
- lugeda majade kirjeldusi;
- vaadata fotosid ja mugavusi;
- võrrelda hindu;
- valida sobiva maja;
- valida broneeringule lisateenuseid;
- teha broneeringu;
- luua kasutajakonto;
- logida sisse;
- vaadata oma broneeringuid;
- hallata profiili;
- muuta rakenduse keelt.

Rakendus ei ole lõplik tootmissüsteem, vaid töötav prototüüp, mida saab tulevikus edasi arendada.

## Põhifunktsioonid

- Avaleht majade ja teenuse tutvustusega
- Sobiva maja leidmise vaade
- Majade detailvaated
- Hinnakirja vaade
- Broneerimise vorm
- Lisateenuste valik broneerimisel
- Kasutaja registreerimine ja sisselogimine
- Kasutaja profiil
- Broneeringute vaatamine
- Lemmikute lisamine ja eemaldamine
- VIP-loogika kasutaja broneeringute põhjal
- Keelevalik
- Kohalikud teavitused
- Keskne Supabase andmebaas

## Andmebaas ja serveripoolne loogika

Varasem lokaalse andmebaasi loogika on asendatud Supabase-põhise keskse andmebaasiga. See tähendab, et kasutajad, majad, broneeringud, lisateenused ja lemmikud ei ela ainult ühes seadmes, vaid salvestatakse ühisesse andmebaasi.

Rakendus kasutab Supabase andmebaasis järgmisi põhitabeleid:

- `app_users` — kasutajakontod, profiiliandmed ja VIP-staatus;
- `houses` — majade põhiandmed;
- `house_translations` — majade tekstid eri keeltes;
- `house_amenities` — majade mugavused;
- `house_photos` — majade pildid;
- `reservations` — kasutajate broneeringud;
- `reservation_addons` — broneeringu juurde valitud lisateenused;
- `favourites` — kasutaja lemmikmajad.

Broneeringu loomisel kontrollib rakendus valitud maja saadavust, loob kirje tabelisse `reservations` ning salvestab valitud lisateenused tabelisse `reservation_addons`.

## Kasutatud tehnoloogiad

Projektis kasutati järgmisi tehnoloogiaid ja töövahendeid:

- C#
- XAML
- .NET 9
- .NET MAUI
- MVVM arhitektuur
- CommunityToolkit.Mvvm
- Supabase
- PostgREST
- Plugin.LocalNotification
- Visual Studio 2022
- GitHub

## Toetatud platvormid

Projekt on seadistatud järgmistele .NET MAUI platvormidele:

- Android
- iOS
- MacCatalyst
- Windows

Põhitestimine on suunatud eelkõige Androidi ja Windowsi arenduskeskkonnale. MacCatalyst on kasulik arenduse ja kiire testimise jaoks, kuid kohalikud teavitused võivad vajada platvormipõhist käsitlemist.

## Projekti struktuur

Projekt on jagatud loogilisteks osadeks:

```text
SaunakulaApp/
├── Models/          # Andmemudelid ja Supabase tabelite mudelid
├── Services/        # Andmebaasi, kasutaja, broneeringute ja lokaliseerimise teenused
├── Views/           # Rakenduse vaated
├── ViewModels/      # MVVM vaatemudelid
├── Resources/       # Pildid, ikoonid, fondid ja muud ressursid
├── AppShell.xaml    # Rakenduse navigatsioon
├── MauiProgram.cs   # Teenuste ja MAUI rakenduse seadistus
└── SaunakulaApp.csproj
```

## Käivitamine arenduskeskkonnas

1. Klooni repositoorium:

```bash
git clone https://github.com/GlebDran/SaunakulaApp.git
```

2. Liigu projekti kausta:

```bash
cd SaunakulaApp
```

3. Vali arendusharu:

```bash
git checkout mvvm-db-logic
```

4. Taasta paketid ja ehita projekt:

```bash
dotnet restore
dotnet build
```

Visual Studio 2022 kasutamisel ava lahendusfail ning vali sobiv käivitusplatvorm, näiteks Android emulaator või Windows Machine.

## Märkus

See projekt on õppeprojekti prototüüp. Rakendus demonstreerib mobiilirakenduse arhitektuuri, MVVM-mustrit, Supabase andmebaasiga suhtlemist, kasutajakonto loomist ja broneerimise töövoogu. Enne päris tootmiskasutust tuleks lisada täiendav turvalisus, põhjalikum veakäsitlus ja lõplik äriloogika kontroll serveri poolel.
