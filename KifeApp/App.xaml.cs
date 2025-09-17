namespace KifeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // تحميل تفضيل الثيم عند بدء التطبيق
            if (Preferences.ContainsKey("AppTheme"))
            {
                var savedTheme = (AppTheme)Preferences.Get("AppTheme", (int)AppTheme.Unspecified);
                App.Current.UserAppTheme = savedTheme;
            }
        }
        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}