// Views/SearchPage.xaml.cs
using KifeApp.ViewModels;

namespace KifeApp.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}