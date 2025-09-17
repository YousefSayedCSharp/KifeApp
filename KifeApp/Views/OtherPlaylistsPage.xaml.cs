// Views/OtherPlaylistsPage.xaml.cs
using KifeApp.ViewModels;

namespace KifeApp.Views;

public partial class OtherPlaylistsPage : ContentPage
{
    public OtherPlaylistsPage(OtherPlaylistsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
