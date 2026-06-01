using SaunakulaApp.ViewModels;

namespace SaunakulaApp.Views;

public partial class BookingsPage : ContentPage
{
    private readonly BookingsViewModel _viewModel;

    public BookingsPage(BookingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
