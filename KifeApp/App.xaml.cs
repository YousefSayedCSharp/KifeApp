#if WINDOWS
using Windows.UI.Notifications;
#endif
using KifeApp.Services; // تأكد من استيراد هذه

namespace KifeApp;

public partial class App : Application
{
    private readonly JsonDataService _jsonDataService;

    public App(JsonDataService jsonDataService)
    {
        InitializeComponent();
        _jsonDataService = jsonDataService; // حقن JsonDataService
        Preferences.Clear();
        // تحميل تفضيل الثيم عند بدء التطبيق
        if (Preferences.ContainsKey("AppTheme")&&App.Current!=null)
        {
            var savedTheme = (AppTheme)Preferences.Get("AppTheme", (int)AppTheme.Unspecified);
            App.Current.UserAppTheme = savedTheme;
        }

        // ابدأ عملية التحقق من تحديث البيانات في الخلفية
        // لا تنتظر اكتمالها هنا، لكي لا تعرقل بدء تشغيل التطبيق
        _ = CheckForDataUpdateOnStartupAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private async Task CheckForDataUpdateOnStartupAsync()
    {
        // تأخير بسيط لإعطاء التطبيق فرصة للبدء بالكامل وتجنب المشاكل عند التشغيل
        await Task.Delay(TimeSpan.FromSeconds(5));

        Console.WriteLine("Initiating background data update check...");
        bool updated = await _jsonDataService.CheckForAndUpdateDataAsync(forceUpdate: false);

        if (updated)
        {
            // إذا تم التحديث بنجاح، أبلغ المستخدم لإعادة التشغيل
            // يجب أن يحدث هذا على الـ UI thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                //await Shell.Current.DisplayAlert("تحديث البيانات", "تم تحديث البيانات بنجاح", "موافق");
                ReloadAppShell();
                ShowToast("تم تحديث البيانات بنجاح!", 3);
            });
        }
    }

    //public static void ReloadAppShell()
    //{
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        // لو في MainPage حالية، فضيها
    //        if (App.Current.MainPage is IDisposable disposablePage)
    //        {
    //            disposablePage.Dispose();
    //        }

    //        // اعمل شيل جديد
    //        App.Current.MainPage = new AppShell();
    //    });
    //}

    public static void ReloadAppShell()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page is IDisposable disposablePage)
            {
                disposablePage.Dispose();
            }

            if (window != null)
            {
                window.Page = new AppShell();
            }
        });
    }


    //    public void ShowToast(string msg, int length)
    //    {
    //#if ANDROID
    //        var l = (length==3)? Android.Widget.ToastLength.Short : Android.Widget.ToastLength.Long;
    //        var context = Android.App.Application.Context;
    //        Android.Widget.Toast.MakeText(context, msg, Android.Widget.ToastLength.Short)?.Show();
    //#elif WINDOWS
    //        string title = "إشعار";
    //    string content = msg;

    //    // إنشاء XML template للتوست
    //    var toastXml = global::Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(
    //        global::Windows.UI.Notifications.ToastTemplateType.ToastText02);

    //    var stringElements = toastXml.GetElementsByTagName("text");
    //    stringElements[0].AppendChild(toastXml.CreateTextNode(title));
    //    stringElements[1].AppendChild(toastXml.CreateTextNode(content));

    //    // بناء ToastNotification
    //    var toast = new global::Windows.UI.Notifications.ToastNotification(toastXml);

    //    // استخدم الـ AppId (من Package.appxmanifest)
    //    var notifier = global::Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier("com.yourcompany.yourapp");
    //    notifier.Show(toast);
    //#endif
    //    }

    public static void ShowToast(string msg, int length)
    {
#if ANDROID
        // لو 3 ثواني => Short, غير كدة => Long
        var l = (length == 3) ? Android.Widget.ToastLength.Short : Android.Widget.ToastLength.Long;
        var context = Android.App.Application.Context;
        Android.Widget.Toast.MakeText(context, msg, l)?.Show();

#elif WINDOWS
    string title = "إشعار";
    string content = msg;

    // إنشاء XML template للتوست
    var toastXml = global::Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(
        global::Windows.UI.Notifications.ToastTemplateType.ToastText02);

    var stringElements = toastXml.GetElementsByTagName("text");
    stringElements[0].AppendChild(toastXml.CreateTextNode(title));
    stringElements[1].AppendChild(toastXml.CreateTextNode(content));

    // بناء ToastNotification
    var toast = new global::Windows.UI.Notifications.ToastNotification(toastXml);

    // 🔹 تحديد مدة الظهور بالثواني (حسب ما المستخدم بعت)
    toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(length);

    // استخدم الـ AppId (من Package.appxmanifest)
    var notifier = global::Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier("KifeApp");
    notifier.Show(toast);
#endif
    }

}