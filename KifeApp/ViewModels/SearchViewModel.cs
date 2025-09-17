//// ViewModels/SearchViewModel.cs
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using KifeApp.Models;
//using KifeApp.Services;
//using System.Collections.ObjectModel;
//using System.Windows.Input;

//namespace KifeApp.ViewModels;

//public partial class SearchViewModel : ObservableObject
//{
//    private readonly JsonDataService _jsonDataService;
//    private List<VideoModel> _allVideos;

//    [ObservableProperty]
//    public ObservableCollection<VideoModel> filteredVideos = new ObservableCollection<VideoModel>();

//    [ObservableProperty]
//    public string searchText;

//    public ICommand FilterVideosCommand { get; }
//    public ICommand OpenVideoCommand { get; }
//    public ICommand ShareVideoCommand { get; }

//    public ICommand ShowAllVideosCommand { get; }
//    public ICommand FilterByTodayCommand { get; }
//    public ICommand FilterByLast7DaysCommand { get; }
//    public ICommand FilterByLastMonthCommand { get; }
//    public ICommand FilterByLastYearCommand { get; }

//    //public SearchViewModel(JsonDataService jsonDataService)
//    //{
//    //    _jsonDataService = jsonDataService;
//    //    FilterVideosCommand = new RelayCommand(FilterVideos);
//    //    OpenVideoCommand = new AsyncRelayCommand<VideoModel>(OpenVideo);
//    //    ShareVideoCommand = new AsyncRelayCommand<VideoModel>(ShareVideo);

//    //    _ = LoadAllVideosAsync();
//    //}

//    public SearchViewModel(JsonDataService jsonDataService)
//    {
//        _jsonDataService = jsonDataService;
//        FilterVideosCommand = new RelayCommand(FilterVideos);
//        OpenVideoCommand = new AsyncRelayCommand<VideoModel>(OpenVideo);
//        ShareVideoCommand = new AsyncRelayCommand<VideoModel>(ShareVideo);

//        ShowAllVideosCommand = new RelayCommand(ShowAllVideos);
//        FilterByTodayCommand = new RelayCommand(FilterByToday);
//        FilterByLast7DaysCommand = new RelayCommand(FilterByLast7Days);
//        FilterByLastMonthCommand = new RelayCommand(FilterByLastMonth);
//        FilterByLastYearCommand = new RelayCommand(FilterByLastYear);

//        _ = LoadAllVideosAsync();
//    }

//    private async Task LoadAllVideosAsync()
//    {
//        var allPlaylists = await _jsonDataService.LoadDataAsync();
//        if (allPlaylists != null)
//        {
//            _allVideos = allPlaylists.SelectMany(p => p.ToList()).ToList();
//            FilterVideos(); // عرض كل الفيديوهات مبدئيًا
//        }
//    }

//    private void FilterVideos()
//    {
//        if (_allVideos == null)
//            return;

//        FilteredVideos.Clear();

//        IEnumerable<VideoModel> query = _allVideos;

//        if (!string.IsNullOrWhiteSpace(SearchText))
//        {
//            // البحث بالعنوان
//            query = query.Where(v => v.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
//        }

//        // الآن، تطبيق الفلاتر الزمنية بناءً على الأزرار (ستتم إضافتها في الـ XAML)
//        // هذه منطق عام، ستحتاج لربطها بـ Properties في الـ ViewModel تحدد الفترة الزمنية المختارة.
//        // على سبيل المثال، يمكنك إضافة خاصية SelectedFilterPeriod (اليوم، آخر 7 أيام، شهر، عام)
//        // ونقوم بتصفية الفيديوهات بناءً عليها.

//        foreach (var video in query)
//        {
//            //if(FilteredVideos.FirstOrDefault(v=>v.Title==video.Title)==null)
//            FilteredVideos.Add(video);
//        }
//    }

//    // هذه دوال مساعدة للفلاتر الزمنية، يمكنك استدعائها من خلال أزرار
//    public void FilterByToday()
//    {
//        FilteredVideos.Clear();
//        var today = DateTime.Today;
//        foreach (var video in _allVideos.Where(v => v.UploadDate?.Date == today))
//        {
//            FilteredVideos.Add(video);
//        }
//    }

//    public void FilterByLast7Days()
//    {
//        FilteredVideos.Clear();
//        var sevenDaysAgo = DateTime.Today.AddDays(-7);
//        foreach (var video in _allVideos.Where(v => v.UploadDate?.Date >= sevenDaysAgo && v.UploadDate?.Date <= DateTime.Today))
//        {
//            FilteredVideos.Add(video);
//        }
//    }

//    public void FilterByLastMonth()
//    {
//        FilteredVideos.Clear();
//        var oneMonthAgo = DateTime.Today.AddMonths(-1);
//        foreach (var video in _allVideos.Where(v => v.UploadDate?.Date >= oneMonthAgo && v.UploadDate?.Date <= DateTime.Today))
//        {
//            FilteredVideos.Add(video);
//        }
//    }

//    public void FilterByLastYear()
//    {
//        FilteredVideos.Clear();
//        var oneYearAgo = DateTime.Today.AddYears(-1);
//        foreach (var video in _allVideos.Where(v => v.UploadDate?.Date >= oneYearAgo && v.UploadDate?.Date <= DateTime.Today))
//        {
//            FilteredVideos.Add(video);
//        }
//    }

//    public void ShowAllVideos()
//    {
//        FilteredVideos.Clear();
//        if (_allVideos != null)
//        {
//            foreach (var video in _allVideos)
//            {
//                FilteredVideos.Add(video);
//            }
//        }
//    }

//    private async Task OpenVideo(VideoModel video)
//    {
//        if (video?.URL != null)
//        {
//            try
//            {
//                await Browser.OpenAsync(video.URL, BrowserLaunchMode.SystemPreferred);
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("خطأ", $"تعذر فتح الفيديو: {ex.Message}", "موافق");
//            }
//        }
//    }

//    private async Task ShareVideo(VideoModel video)
//    {
//        if (video?.URL != null && video.Title != null)
//        {
//            await Share.RequestAsync(new ShareTextRequest
//            {
//                Text = $"{video.Title}\n{video.URL}",
//                Title = "مشاركة الفيديو"
//            });
//        }
//    }
//}
//2//
// ViewModels/SearchViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KifeApp.Models;
using KifeApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KifeApp.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly JsonDataService _jsonDataService;
    private List<VideoModel> _allVideos; // تحتوي على جميع الفيديوهات بدون تكرار ومرتبة

    [ObservableProperty]
    public ObservableCollection<VideoModel> filteredVideos = new ObservableCollection<VideoModel>();

    [ObservableProperty]
    public string searchText;

    // تم حذف هذه الأوامر من هنا لأنها تم تعريفها كـ [RelayCommand] أدناه
    // public ICommand FilterVideosCommand { get; }
    // public ICommand OpenVideoCommand { get; }
    // public ICommand ShareVideoCommand { get; }
    // public ICommand ShowAllVideosCommand { get; }
    // public ICommand FilterByTodayCommand { get; }
    // public ICommand FilterByLast7DaysCommand { get; }
    // public ICommand FilterByLastMonthCommand { get; }
    // public ICommand FilterByLastYearCommand { get; }

    public SearchViewModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
        // تم استبدال تعريف الأوامر اليدوي بـ [RelayCommand]
        _ = LoadAllVideosAsync();
    }

    private async Task LoadAllVideosAsync()
    {
        var allPlaylists = await _jsonDataService.LoadDataAsync();
        if (allPlaylists != null)
        {
            // دمج جميع الفيديوهات من قوائم التشغيل
            var combinedVideos = allPlaylists.SelectMany(p => p.ToList()).ToList();

            // إزالة التكرارات بناءً على URL (يمكنك اختيار خاصية أخرى كـ ID إن وجدت)
            // ثم ترتيبها تصاعديًا حسب تاريخ الرفع
            _allVideos = combinedVideos
                            .GroupBy(v => v.URL) // تجميع بناءً على الـ URL لإزالة التكرارات
                            .Select(g => g.First()) // أخذ أول عنصر من كل مجموعة (للتخلص من التكرار)
                            .OrderByDescending(v => v.UploadDate ?? DateTime.MinValue) // ترتيب الأحدث أولاً، مع التعامل مع القيم الفارغة
                            .ToList();

            FilterVideos(); // عرض كل الفيديوهات مبدئيًا بعد التحميل والترتيب وإزالة التكرارات
        }
    }

    [RelayCommand]
    private void FilterVideos()
    {
        if (_allVideos == null)
            return;

        FilteredVideos.Clear();

        IEnumerable<VideoModel> query = _allVideos;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            // البحث بالعنوان
            query = query.Where(v => v.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // تطبيق الفلترة بعد البحث، والترتيب تصاعديًا حسب تاريخ الرفع (الأحدث أولاً)
        foreach (var video in query)
        {
            FilteredVideos.Add(video);
        }
    }

    [RelayCommand]
    public void FilterByToday()
    {
        FilteredVideos.Clear();
        var today = DateTime.Today;
        foreach (var video in _allVideos
                                .Where(v => v.UploadDate?.Date == today)
                                .OrderByDescending(v => v.UploadDate)) // الأحدث أولاً
        {
            FilteredVideos.Add(video);
        }
    }

    [RelayCommand]
    public void FilterByLast7Days()
    {
        FilteredVideos.Clear();
        var sevenDaysAgo = DateTime.Today.AddDays(-7);
        foreach (var video in _allVideos
                                .Where(v => v.UploadDate?.Date >= sevenDaysAgo && v.UploadDate?.Date <= DateTime.Today)
                                .OrderByDescending(v => v.UploadDate)) // الأحدث أولاً
        {
            FilteredVideos.Add(video);
        }
    }

    [RelayCommand]
    public void FilterByLastMonth()
    {
        FilteredVideos.Clear();
        var oneMonthAgo = DateTime.Today.AddMonths(-1);
        foreach (var video in _allVideos
                                .Where(v => v.UploadDate?.Date >= oneMonthAgo && v.UploadDate?.Date <= DateTime.Today)
                                .OrderByDescending(v => v.UploadDate)) // الأحدث أولاً
        {
            FilteredVideos.Add(video);
        }
    }

    [RelayCommand]
    public void FilterByLastYear()
    {
        FilteredVideos.Clear();
        var oneYearAgo = DateTime.Today.AddYears(-1);
        foreach (var video in _allVideos
                                .Where(v => v.UploadDate?.Date >= oneYearAgo && v.UploadDate?.Date <= DateTime.Today)
                                .OrderByDescending(v => v.UploadDate)) // الأحدث أولاً
        {
            FilteredVideos.Add(video);
        }
    }

    [RelayCommand]
    public void ShowAllVideos()
    {
        FilteredVideos.Clear();
        if (_allVideos != null)
        {
            // _allVideos بالفعل مرتبة حسب الأحدث أولاً بعد التحميل
            foreach (var video in _allVideos)
            {
                FilteredVideos.Add(video);
            }
        }
    }

    [RelayCommand]
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

    [RelayCommand]
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
