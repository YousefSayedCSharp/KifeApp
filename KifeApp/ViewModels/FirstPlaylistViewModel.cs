// ViewModels/FirstPlaylistViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KifeApp.Models;
using KifeApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KifeApp.ViewModels
{
    public partial class FirstPlaylistViewModel : ObservableObject
    {
        private readonly JsonDataService _jsonDataService;
        private List<VideoModel> _allFirstPlaylistVideos;
        private int _loadedVideoCount = 0;
        private const int VideosPerPage = 5;

        [ObservableProperty]
        public ObservableCollection<VideoModel> videos = new ObservableCollection<VideoModel>();

        [ObservableProperty]
        public bool isLoadingMore = false; // لإظهار مؤشر التحميل

        public ICommand LoadMoreVideosCommand { get; }
        public ICommand OpenVideoCommand { get; }
        public ICommand ShareVideoCommand { get; }

        public FirstPlaylistViewModel(JsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
            LoadMoreVideosCommand = new AsyncRelayCommand(LoadMoreVideosAsync);
            OpenVideoCommand = new AsyncRelayCommand<VideoModel>(OpenVideo);
            ShareVideoCommand = new AsyncRelayCommand<VideoModel>(ShareVideo);

            MainThread.BeginInvokeOnMainThread(async () => 
            {
            await LoadInitialVideosAsync(); // تحميل الفيديوهات الأولية عند إنشاء الـ ViewModel
            });
        }

        private async Task LoadInitialVideosAsync()
        {
            await Task.Run(async() =>
            {
            var allPlaylists = await _jsonDataService.LoadDataAsync();
            if (allPlaylists != null && allPlaylists.Any())
            {
                _allFirstPlaylistVideos = allPlaylists[0].ToList(); // أول قائمة تشغيل
                await LoadMoreVideosAsync(); // تحميل أول 10 فيديوهات
            
        }
            });
        }

        private async Task LoadMoreVideosAsync()
        {
            await Task.Run(async() =>
            {
            if (IsLoadingMore || _allFirstPlaylistVideos == null || _loadedVideoCount >= _allFirstPlaylistVideos.Count)
                return;

            IsLoadingMore = true;

            await Task.Delay(500); // محاكاة وقت التحميل

            var videosToLoad = _allFirstPlaylistVideos
                .Skip(_loadedVideoCount)
                .Take(VideosPerPage)
                .ToList();

            foreach (var video in videosToLoad)
            {
                Videos.Add(video);
            }

            _loadedVideoCount += videosToLoad.Count;
            IsLoadingMore = false;
                //Shell.Current.DisplayAlert("",""+Videos.FirstOrDefault().Title, "OK");
            });
        }
        

        private async Task OpenVideo(VideoModel video)
        {
            if (video?.URL != null)
            {
                try
                {
                    await Browser.OpenAsync(video.URL, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"تعذر فتح الفيديو: {ex.Message}", "موافق");
                }
            }
        }

        private async Task ShareVideo(VideoModel video)
        {
            try
            {
                //Shell.Current.DisplayAlert("", "" + video.Title, "OK");
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("",ex.Message,"OK");
                return;
            }
            if (video?.URL != null && video.Title != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = $"{video.Title}\n{video.URL}",
                    Title = "مشاركة الفيديو"
                });
            }
        }
    }
}