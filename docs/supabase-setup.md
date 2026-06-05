# Supabase setup for SaunakulaApp

This branch adapts SaunakulaApp to a central Supabase/PostgreSQL database. Houses, translations, amenities, photos, addons, users, favourites and reservations are stored in shared tables so every device works with the same booking data.

## Why this fixes the main database issue

Before:

```text
Phone A -> local saunakula.db3
Phone B -> local saunakula.db3
```

Both phones can book the same house for the same dates because they do not see each other.

After:

```text
MAUI View -> ViewModel -> DatabaseService/SupabaseService -> Supabase PostgreSQL
```

Every device loads the same house data, user data, favourites and reservations from the same central database.

## What changed in the app

- `HouseService` no longer stores a hardcoded list of houses.
- `HouseService` now loads house business data through `SupabaseService`.
- `SupabaseService` loads normalized tables: `houses`, `house_translations`, `house_amenities`, `house_photos`.
- `DatabaseService` now routes users, bookings, favourites and VIP state through Supabase instead of local SQLite.
- `LoginViewModel` and `RegisterViewModel` read and write users through the `app_users` table.
- `BookingViewModel` creates reservations with `user_id`, so `BookingsPage` can show the current user's central bookings.
- `HouseDetailsViewModel` and `ProfileViewModel` use the central `favourites` table.
- Booked dates are shown as grey calendar cells, and the confirm button is disabled if the selected period overlaps a confirmed reservation.
- `reservations` has a PostgreSQL exclusion constraint that blocks overlapping confirmed bookings for the same house.
- The seed data includes all four app house IDs: `soome`, `vene`, `jahimees`, and `spa`.

## MVVM coverage

The branch now has real ViewModels instead of empty placeholders:

- `HomeViewModel` loads the home screen data, featured house, category filters and navigation commands.
- `HouseDetailsViewModel` loads one house, photo gallery state, amenities, localized labels, contact actions, favourite toggle and booking navigation.
- `BookingViewModel` owns booking state, guest count, addons, price calculation, Supabase booking creation and the availability calendar.
- `LoginViewModel` owns login form state, validation, error display, password checking and register navigation.
- `RegisterViewModel` owns registration form state, validation, duplicate email check, user creation and post-registration login.
- `BookingsViewModel` owns the upcoming/past booking list, empty/login states and cancellation command.
- `ProfileViewModel` owns profile state, language switching, VIP progress, favourites, contact actions and logout.
- `HomePage.xaml.cs`, `HouseDetailsPage.xaml.cs`, `BookingPage.xaml.cs`, `BookingsPage.xaml.cs`, `ProfilePage.xaml.cs`, `LoginPage.xaml.cs` and `RegisterPage.xaml.cs` are reduced to page initialization, route wiring or small UI-only event handling.

## Setup steps

1. Open Supabase SQL Editor.
2. Run [`database/supabase-schema.sql`](../database/supabase-schema.sql).
3. Run [`database/supabase-app-tables.sql`](../database/supabase-app-tables.sql). Run it again if the project was already set up before this branch update; it adds the `reservations.user_id` relation.
4. Start the MAUI app from this branch.
5. Test that houses load from the central database.
6. Register or log in and check that the new user appears in `app_users`.
7. Open a house details page and check that photos, amenities, prices, favourite and booking navigation still work.
8. Open a house booking page and check that confirmed Supabase reservations appear as grey unavailable dates.
9. Create a booking and check that `reservations.user_id` is filled with the logged-in user's id.
10. Open bookings and profile screens to check user bookings, favourites, language switching and VIP progress.
11. Test booking the same house for overlapping dates from two devices or two runs; the second booking should be rejected.

## Tables

```text
app_users
houses
house_translations
house_amenities
house_photos
addons
addon_translations
reservations
reservation_addons
favourites
```

## Important note

The current `SupabaseService` contains one Supabase URL/key pair. If you create your own Supabase project, replace `SupabaseUrl` and `SupabaseKey` in `SaunakulaApp/Services/SupabaseService.cs` with values from your project settings.

## Next integration step

The database, user accounts, favourites, home screen, house details screen, auth screens, booking flow, bookings page and profile page are ready for local testing. After that, the remaining cleanup is mostly polish: check the less central pages (`PricingPage`, `HouseFinderPage`, `SplashPage`) and decide whether every page needs a dedicated ViewModel or whether the main user flows are enough for the project requirements.
