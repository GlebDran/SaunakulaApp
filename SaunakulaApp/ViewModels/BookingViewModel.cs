using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.ViewModels;

public partial class BookingViewModel : BaseViewModel
{
    private readonly HouseService _houseService;
    private readonly SupabaseService _supabaseService;
    private readonly SessionService _sessionService;

    public ObservableCollection<House> Houses { get; } = new();
    public ObservableCollection<SupabaseReservation> MyReservations { get; } = new();

    [ObservableProperty]
    private House? selectedHouse;

    [ObservableProperty]
    private string customerName = string.Empty;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public BookingViewModel(
        HouseService houseService,
        SupabaseService supabaseService,
        SessionService sessionService)
    {
        _houseService = houseService;
        _supabaseService = supabaseService;
        _sessionService = sessionService;
        Title = "Booking";

        if (_sessionService.CurrentUser is not null)
        {
            CustomerName = string.IsNullOrWhiteSpace(_sessionService.CurrentUser.FullName)
                ? _sessionService.CurrentUser.Email
                : _sessionService.CurrentUser.FullName;
        }

        _ = LoadHousesAsync();
    }

    [RelayCommand]
    private async Task LoadHousesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var houses = await _houseService.GetAllAsync();

            Houses.Clear();
            foreach (var house in houses)
                Houses.Add(house);

            SelectedHouse ??= Houses.FirstOrDefault();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Houses could not be loaded: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ConfirmBookingAsync()
    {
        if (SelectedHouse is null || string.IsNullOrWhiteSpace(CustomerName) || EndDate <= StartDate)
        {
            StatusMessage = "Check selected house, customer name and dates.";
            return;
        }

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Checking central database...";

            var success = await _supabaseService.BookHouseAsync(
                SelectedHouse.Id,
                CustomerName.Trim(),
                StartDate,
                EndDate);

            StatusMessage = success
                ? "Booking confirmed in central database."
                : "This house is already booked for the selected dates.";

            if (success)
                await LoadMyReservationsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Booking failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadMyReservationsAsync()
    {
        if (string.IsNullOrWhiteSpace(CustomerName))
        {
            MyReservations.Clear();
            return;
        }

        try
        {
            var reservations = await _supabaseService.GetClientReservationsAsync(CustomerName.Trim());
            MyReservations.Clear();

            foreach (var reservation in reservations)
                MyReservations.Add(reservation);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Reservations could not be loaded: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CancelReservationAsync(SupabaseReservation reservation)
    {
        if (reservation is null)
            return;

        try
        {
            await _supabaseService.DeleteReservationAsync(reservation.Id);
            StatusMessage = "Booking cancelled in central database.";
            await LoadMyReservationsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Cancellation failed: {ex.Message}";
        }
    }
}
