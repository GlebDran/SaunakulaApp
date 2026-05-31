# Supabase setup for SaunakulaApp

This branch uses the useful idea from `MarinaOleinik/Saun_App`, but adapts it to the real SaunakulaApp structure: houses, translations, amenities, photos, addons, users, favourites and reservations are stored in one central Supabase/PostgreSQL database.

## Why this fixes the main database issue

Before:

```text
Phone A -> local saunakula.db3
Phone B -> local saunakula.db3
```

Both phones can book the same house for the same dates because they do not see each other.

After:

```text
MAUI View -> ViewModel -> HouseService/SupabaseService -> Supabase PostgreSQL
```

Every device loads the same house data and checks the same `reservations` table before creating a booking.

## What changed in the app

- `HouseService` no longer stores a hardcoded list of houses.
- `HouseService` now loads house business data through `SupabaseService`.
- `SupabaseService` loads normalized tables: `houses`, `house_translations`, `house_amenities`, `house_photos`.
- `reservations` has a PostgreSQL exclusion constraint that blocks overlapping confirmed bookings for the same house.
- The teacher prototype had only three simple houses; this branch adds all four app house IDs: `soome`, `vene`, `jahimees`, `spa`.

## Setup steps

1. Open Supabase SQL Editor.
2. Run [`database/supabase-schema.sql`](../database/supabase-schema.sql).
3. Run [`database/supabase-app-tables.sql`](../database/supabase-app-tables.sql).
4. Start the MAUI app from this branch.
5. Test that houses load from the central database.
6. Test booking the same house for overlapping dates from two devices or two runs; the second booking should be rejected.

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

The current `SupabaseService` points to the Supabase URL/key from the teacher prototype. If you create your own Supabase project, replace `SupabaseUrl` and `SupabaseKey` in `SaunakulaApp/Services/SupabaseService.cs`.

## Next integration step

The database layer is now ready enough for the next task: connect the existing booking UI fully to `BookingViewModel` and move the remaining booking logic out of `Views/BookingPage.xaml.cs`.
