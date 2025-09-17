// Views/FirstPlaylistPage.xaml.cs
using KifeApp.ViewModels;

namespace KifeApp.Views;

public partial class FirstPlaylistPage : ContentPage
{
    FirstPlaylistViewModel _viewModel;

    public FirstPlaylistPage(FirstPlaylistViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
#if ANDROID
Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(500);
                if (viewModel.videos.Count <= 10)
                {
                    cv.ScrollTo(0);
                    continue;
                }

                if (viewModel.videos.Count > 10)
                    break;
            }
        });
#endif
    }
}
