# SAUNAKULAAPP

SAUNAKULAAPP on .NET MAUI abil loodud mobiilirakenduse prototüüp, mis aitab kasutajal tutvuda Saunaküla puhkemajade ja saunadega, vaadata hindu ning teha broneeringut.

Rakendus valmis õppeprojekti raames. Eesmärk oli luua praktiline lahendus, mis näitab, kuidas Saunaküla teenuse info ja broneerimise loogika võiks töötada mobiilirakenduses.

## Projekti eesmärk

Projekti eesmärk oli luua mobiilirakenduse prototüüp, kus kasutaja saab:

- vaadata Saunaküla maju;
- lugeda majade kirjeldusi;
- võrrelda hindu;
- valida sobiva maja;
- teha broneeringu;
- luua kasutajakonto;
- logida sisse;
- vaadata oma broneeringuid;
- muuta rakenduse keelt.

Rakendus ei ole lõplik tootmissüsteem, vaid töötav prototüüp, mida saab tulevikus edasi arendada.

## Põhifunktsioonid

- Avaleht majade ja teenuse tutvustusega
- Sobiva maja leidmise vaade
- Majade detailvaated
- Hinnakirja vaade
- Broneerimise vorm
- Kasutaja registreerimine ja sisselogimine
- Kasutaja profiil
- Broneeringute vaatamine
- Keelevalik
- Kohalikud teavitused
- Lokaalne SQLite andmebaas

## Kasutatud tehnoloogiad

Projektis kasutati järgmisi tehnoloogiaid ja töövahendeid:

- C#
- XAML
- .NET MAUI
- SQLite
- sqlite-net-pcl
- Plugin.LocalNotification
- Visual Studio 2022
- GitHub

## Projekti struktuur

Projekt on jagatud loogilisteks osadeks:

```text
SaunakulaApp/
├── Models/
├── Services/
├── Views/
├── ViewModels/
├── Resources/
├── AppShell.xaml
├── MauiProgram.cs
└── SaunakulaApp.csproj
