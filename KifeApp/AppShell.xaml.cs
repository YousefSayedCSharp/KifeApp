using KifeApp.Views;

namespace KifeApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // تسجيل المسار لصفحة AboutPage هنا
        // هذا يسمح بالانتقال إلى AboutPage باستخدام Shell.Current.GoToAsync("AboutPage");
        // دون الحاجة لجعلها جزءًا من الـ TabBar أو الـ Flyout بشكل مباشر.
        Routing.RegisterRoute("AboutPage", typeof(Views.AboutPage));
    }

    public static void ShowToast(string msg, int length)
    {
#if ANDROID
        var context = Android.App.Application.Context;
        Android.Widget.Toast.MakeText(context, msg, Android.Widget.ToastLength.Short)?.Show();
#elif WINDOWS
//new ToastContentBuilder()
                //.AddText("إشعار")
                //.AddText(msg)
                //.Show();
                
    //                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
    //var stringElements = toastXml.GetElementsByTagName("text");
    //stringElements[0].AppendChild(toastXml.CreateTextNode("إشعار"));
    //stringElements[1].AppendChild(toastXml.CreateTextNode(msg));

    //// إنشاء التوست
    //var toast = new ToastNotification(toastXml);

    //// عرض التوست
    //var notifier = ToastNotificationManager.CreateToastNotifier("YourAppId");
    //notifier.Show(toast);

        string title = "إشعار";
    string content = msg;

    // إنشاء XML template للتوست
    var toastXml = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(
        Windows.UI.Notifications.ToastTemplateType.ToastText02);

    var stringElements = toastXml.GetElementsByTagName("text");
    stringElements[0].AppendChild(toastXml.CreateTextNode(title));
    stringElements[1].AppendChild(toastXml.CreateTextNode(content));

    // بناء ToastNotification
    var toast = new Windows.UI.Notifications.ToastNotification(toastXml);

    // استخدم الـ AppId (من Package.appxmanifest)
    var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier("com.yourcompany.yourapp");
    notifier.Show(toast);
#endif
    }
}
