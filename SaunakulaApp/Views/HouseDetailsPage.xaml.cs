using SaunakulaApp.ViewModels;

namespace SaunakulaApp.Views;

[QueryProperty(nameof(HouseId), "houseId")]
public partial class HouseDetailsPage : ContentPage
{
    private readonly HouseDetailsViewModel _viewModel;
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

    public HouseDetailsPage(HouseDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void PhotoGallery_Scrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        if (_viewModel.PhotoCount <= 1)
            return;

        var itemWidth = Math.Max(Width, 1);
        var index = (int)Math.Round(e.HorizontalOffset / itemWidth);
        _viewModel.SetActivePhotoIndex(index);
    }
}
