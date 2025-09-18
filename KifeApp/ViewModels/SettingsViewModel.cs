// ViewModels/SettingsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KifeApp.Models;
using KifeApp.Services; // تأكد من استيراد هذه
using System.Text.Json;
using System.Windows.Input;

namespace KifeApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly JsonDataService _jsonDataService;

    [ObservableProperty]
    public string appVersion = "غير متوفر"; // سيتم تحديثه عند إنشاء الـ ViewModel

    [ObservableProperty]
    public string developerName = "Yousef Sayed";

    [ObservableProperty]
    public string developerInfo = "تطوير تطبيقات الهاتف المحمول باستخدام .NET MAUI.";

    public ICommand ToggleDarkModeCommand { get; }
    public ICommand ToggleLightModeCommand { get; }
    public ICommand ToggleSystemThemeCommand { get; }
    public ICommand OpenWhatsAppCommand { get; }
    public ICommand OpenTelegramCommand { get; }
    public ICommand OpenAboutPageCommand { get; } // زر لفتح صفحة عن التطبيق

    // أوامر جديدة للتحديث
    public ICommand CheckForDataUpdateCommand { get; }
    public ICommand CheckForAppUpdateCommand { get; }

    public SettingsViewModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;

        // تحديث إصدار التطبيق من AppInfo
        AppVersion = AppInfo.Current.VersionString;

        ToggleDarkModeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Dark));
        ToggleLightModeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Light));
        ToggleSystemThemeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Unspecified));
        OpenWhatsAppCommand = new AsyncRelayCommand(OpenWhatsApp);
        OpenTelegramCommand = new AsyncRelayCommand(OpenTelegram);
        OpenAboutPageCommand = new AsyncRelayCommand(OpenAboutPage); // أمر فتح AboutPage

        CheckForDataUpdateCommand = new AsyncRelayCommand(PerformDataUpdateCheck);
        CheckForAppUpdateCommand = new AsyncRelayCommand(PerformAppUpdateCheck);
    }

    private void SetUserAppTheme(AppTheme theme)
    {
        App.Current.UserAppTheme = theme;
        Preferences.Set("AppTheme", (int)theme);
    }

    private async Task OpenWhatsApp()
    {
        string phoneNumber = "01098673012";
        string whatsappUrl = $"whatsapp://send?phone=+2{phoneNumber}";

        try
        {
            await Launcher.OpenAsync(whatsappUrl);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("خطأ", "واتساب غير مثبت أو حدث خطأ.", "موافق");
        }
    }

    private async Task OpenTelegram()
    {
        string phoneNumber = "01098673012";
        string telegramUrl = $"tg://resolve?phone=+2{phoneNumber}";

        try
        {
            await Launcher.OpenAsync(telegramUrl);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("خطأ", "تليجرام غير مثبت أو حدث خطأ.", "موافق");
        }
    }

    private async Task OpenAboutPage()
    {
        await Shell.Current.GoToAsync("AboutPage");
    }

    private async Task PerformDataUpdateCheck()
    {
        if (!_jsonDataService.IsInternetAvailable())
        {
            await Shell.Current.DisplayAlert("تحديث البيانات", "لا يوجد اتصال بالإنترنت. يرجى التحقق من اتصالك والمحاولة مرة أخرى.", "موافق");
            return;
        }

        // تحميل معلومات الإصدار من السحابة
        string? versionJson = await _jsonDataService.DownloadJsonFileAsync(Constants.DataVersionUpdateUrl);
        if (versionJson == null)
        {
            await Shell.Current.DisplayAlert("تحديث البيانات", "حدث خطأ أثناء تحميل معلومات التحديث. يرجى المحاولة لاحقاً.", "موافق");
            return;
        }

        DataVersionInfoModel? versionInfo = null;
        try
        {
            versionInfo = JsonSerializer.Deserialize<DataVersionInfoModel>(versionJson);
        }
        catch (Exception) { /* تجاهل أخطاء التحليل هنا */ }

        if (versionInfo == null)
        {
            await Shell.Current.DisplayAlert("تحديث البيانات", "صيغة ملف معلومات التحديث غير صحيحة. يرجى التواصل مع الدعم.", "موافق");
            return;
        }

        int latestVersion = versionInfo.Version;
        int currentVersion = _jsonDataService.CurrentDataVersion;

        if (latestVersion > currentVersion)
        {
            // يوجد تحديث جديد
            bool userConfirmed = await Shell.Current.DisplayAlert(
                "تحديث البيانات",
                $"يتوفر إصدار بيانات جديد (الإصدار {latestVersion}). هل تريد التحديث الآن؟",
                "نعم",
                "لا");

            if (userConfirmed)
            {
                bool updated = await _jsonDataService.CheckForAndUpdateDataAsync(forceUpdate: false);
                if (updated)
                {
                    await Shell.Current.DisplayAlert("تحديث البيانات", "تم تحديث البيانات بنجاح! يرجى إعادة تشغيل التطبيق لتطبيق التغييرات.", "موافق");
                }
                else
                {
                    await Shell.Current.DisplayAlert("تحديث البيانات", "فشل التحديث. يرجى المحاولة مرة أخرى.", "موافق");
                }
            }
        }
        else
        {
            // لا يوجد تحديث، لكن نعطي خيار التحديث الإجباري
            bool forceUpdateConfirmed = await Shell.Current.DisplayAlert(
                "تحديث البيانات",
                "أنت تستخدم أحدث إصدار من البيانات. هل تريد التحديث على أي حال (قد يكون مفيداً لإصلاح المشاكل)؟",
                "نعم",
                "لا");

            if (forceUpdateConfirmed)
            {
                bool updated = await _jsonDataService.CheckForAndUpdateDataAsync(forceUpdate: true);
                if (updated)
                {
                    await Shell.Current.DisplayAlert("تحديث البيانات", "تم تحديث البيانات بنجاح! يرجى إعادة تشغيل التطبيق لتطبيق التغييرات.", "موافق");
                }
                else
                {
                    await Shell.Current.DisplayAlert("تحديث البيانات", "فشل التحديث الإجباري. يرجى المحاولة مرة أخرى.", "موافق");
                }
            }
        }
    }

    private async Task PerformAppUpdateCheck()
    {
        if (!_jsonDataService.IsInternetAvailable())
        {
            await Shell.Current.DisplayAlert("تحديث التطبيق", "لا يوجد اتصال بالإنترنت. يرجى التحقق من اتصالك والمحاولة مرة أخرى.", "موافق");
            return;
        }

        // تحميل معلومات إصدار التطبيق من السحابة
        string? appVersionJson = await _jsonDataService.DownloadJsonFileAsync(Constants.AppVersionUpdateUrl);
        if (appVersionJson == null)
        {
            await Shell.Current.DisplayAlert("تحديث التطبيق", "حدث خطأ أثناء تحميل معلومات تحديث التطبيق. يرجى المحاولة لاحقاً.", "موافق");
            return;
        }

        AppUpdateInfoModel? appUpdateInfo = null;
        try
        {
            appUpdateInfo = JsonSerializer.Deserialize<AppUpdateInfoModel>(appVersionJson);
        }
        catch (Exception) { /* تجاهل أخطاء التحليل هنا */ }

        if (appUpdateInfo == null || string.IsNullOrWhiteSpace(appUpdateInfo.Version) || string.IsNullOrWhiteSpace(appUpdateInfo.DownloadUrl))
        {
            await Shell.Current.DisplayAlert("تحديث التطبيق", "صيغة ملف معلومات تحديث التطبيق غير صحيحة. يرجى التواصل مع الدعم.", "موافق");
            return;
        }

        Version latestAppVersion = new Version(appUpdateInfo.Version);
        Version currentAppVersion = AppInfo.Current.Version; // استخدام AppInfo.Current.Version للحصول على كائن Version

        if (latestAppVersion > currentAppVersion)
        {
            // يوجد تحديث جديد للتطبيق
            bool userConfirmed = await Shell.Current.DisplayAlert(
                "تحديث التطبيق",
                $"يتوفر إصدار جديد من التطبيق (الإصدار {appUpdateInfo.Version}). هل تريد التحديث الآن؟",
                "نعم",
                "لا");

            if (userConfirmed)
            {
                try
                {
                    // استخدام دالة المساعدة لتحويل الرابط إذا كان رابط GitHub blob
                    string directDownloadUrl = Constants.ConvertGitHubBlobToRawUrl(appUpdateInfo.DownloadUrl);
                    await Browser.OpenAsync(directDownloadUrl, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"تعذر فتح رابط التحديث: {ex.Message}", "موافق");
                }
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("تحديث التطبيق", "أنت تستخدم أحدث إصدار من التطبيق.", "موافق");
        }
    }
}

// موديل بسيط لقراءة معلومات تحديث التطبيق
