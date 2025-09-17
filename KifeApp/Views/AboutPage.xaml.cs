// Views/AboutPage.xaml.cs
using KifeApp.ViewModels;

namespace KifeApp.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}