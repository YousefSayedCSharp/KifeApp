// ViewModels/AboutViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace KifeApp.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    public string developerName = "Yousef Sayed";

    [ObservableProperty]
    public string developerInfo = "تطوير تطبيقات الهاتف المحمول باستخدام .NET MAUI.";

    // يمكنك إضافة خصائص أخرى خاصة بصفحة About هنا إذا لزم الأمر
    [ObservableProperty]
    public string aboutAppText = "KifeApp هو تطبيق يسمح لك بتصفح قناه كيف kife بسهولة مع عرض سهل لقوائم التشغيل والفيديوهات. استمتع بتجربة مشاهدة منظمة ومميزة مع إمكانية البحث والتصفية حسب تاريخ النشر. تمتع بالتحكم الكامل في عرض المحتوى!";

    public AboutViewModel()
    {
        // لا حاجة لأي أوامر هنا بعد الآن
    }
}