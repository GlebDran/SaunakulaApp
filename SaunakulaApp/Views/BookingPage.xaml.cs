using SaunakulaApp.ViewModels;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class BookingPage : ContentPage
{
    private readonly BookingViewModel _viewModel;
    private string _houseId = string.Empty;

    public string HouseId
    {
        get => _houseId;
        set
        {
            _houseId = value;
            _ = _viewModel.InitializeAsync(value);
        }
    }

    public BookingPage(BookingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
