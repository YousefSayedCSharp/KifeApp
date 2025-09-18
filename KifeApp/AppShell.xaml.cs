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
}
