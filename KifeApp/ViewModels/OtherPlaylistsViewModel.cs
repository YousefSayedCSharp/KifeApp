// ViewModels/OtherPlaylistsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KifeApp.Models;
using KifeApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KifeApp.ViewModels
{
    public partial class OtherPlaylistsViewModel : ObservableObject
    {
        private readonly JsonDataService _jsonDataService;
        private List<PlayListModel> _allOtherPlaylists;
        private int _loadedGroupCount = 0;
        private const int GroupsPerPage = 10;

        [ObservableProperty]
        public ObservableCollection<PlayListModel> playlists = new ObservableCollection<PlayListModel>();

        [ObservableProperty]
        public bool isLoadingMore = false;

        public ICommand LoadMoreGroupsCommand { get; }
        public ICommand OpenVideoCommand { get; }
        public ICommand ShareVideoCommand { get; }

        public OtherPlaylistsViewModel(JsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
            LoadMoreGroupsCommand = new AsyncRelayCommand(LoadMoreGroupsAsync);
            OpenVideoCommand = new AsyncRelayCommand<VideoModel>(OpenVideo);
            ShareVideoCommand = new AsyncRelayCommand<VideoModel>(ShareVideo);

            _ = LoadInitialPlaylistsAsync();
        }

        private async Task LoadInitialPlaylistsAsync()
        {
            var allPlaylists = await _jsonDataService.LoadDataAsync();
            if (allPlaylists != null && allPlaylists.Count > 1) // إذا كان هناك أكثر من قائمة واحدة
            {
                _allOtherPlaylists = allPlaylists.Skip(1).ToList(); // تجاهل أول قائمة
                await LoadMoreGroupsAsync();
            }
            else
            {
                _allOtherPlaylists = new List<PlayListModel>(); // تأكد من تهيئة القائمة حتى لو كانت فارغة
            }
        }

        private async Task LoadMoreGroupsAsync()
        {
            try
            {
                if (IsLoadingMore || _allOtherPlaylists == null || _loadedGroupCount >= _allOtherPlaylists.Count)
                    return;

                IsLoadingMore = true;

#if ANDROID
                await Task.Delay(500); // محاكاة وقت التحميل
#endif

                var groupsToLoad = _allOtherPlaylists
                    .Skip(_loadedGroupCount)
                    .Take(GroupsPerPage)
                    .ToList();
                //Shell.Current.DisplayAlert("",""+ groupsToLoad.LastOrDefault().playlist, "OK");
                //return;
                foreach (var group in groupsToLoad)
                {
                    //await Shell.Current.DisplayAlert("",""+ group.FirstOrDefault().Title, "OK");
                    Playlists.Add(group);
                }

                _loadedGroupCount += groupsToLoad.Count;
                IsLoadingMore = false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
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