// App.cs
using KifeApp.Services; // تأكد من استيراد هذه

namespace KifeApp
{
    public partial class App : Application
    {
        private readonly JsonDataService _jsonDataService;

        public App(JsonDataService jsonDataService)
        {
            InitializeComponent();
            _jsonDataService = jsonDataService; // حقن JsonDataService

            // تحميل تفضيل الثيم عند بدء التطبيق
            if (Preferences.ContainsKey("AppTheme"))
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
                    await Shell.Current.DisplayAlert("تحديث البيانات", "تم تحديث البيانات بنجاح! يرجى إعادة تشغيل التطبيق لتطبيق التغييرات.", "موافق");
                });
            }
        }
    }
}