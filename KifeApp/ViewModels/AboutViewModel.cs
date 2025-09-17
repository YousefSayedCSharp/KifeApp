// ViewModels/AboutViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace KifeApp.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        public string appVersion = "1.0.0"; // يمكنك تحديث هذا من info.plist أو AndroidManifest

        [ObservableProperty]
        public string developerName = "Yousef Sayed"; // اسم المطور

        [ObservableProperty]
        public string developerInfo = "تطوير تطبيقات الهاتف المحمول باستخدام .NET MAUI."; // نبذة عن المطور

        public ICommand ToggleDarkModeCommand { get; }
        public ICommand ToggleLightModeCommand { get; }
        public ICommand ToggleSystemThemeCommand { get; }
        public ICommand OpenWhatsAppCommand { get; }
        public ICommand OpenTelegramCommand { get; }

        public AboutViewModel()
        {
            ToggleDarkModeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Dark));
            ToggleLightModeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Light));
            ToggleSystemThemeCommand = new RelayCommand(() => SetUserAppTheme(AppTheme.Unspecified));
            OpenWhatsAppCommand = new AsyncRelayCommand(OpenWhatsApp);
            OpenTelegramCommand = new AsyncRelayCommand(OpenTelegram);
        }

        private void SetUserAppTheme(AppTheme theme)
        {
            App.Current.UserAppTheme = theme;
            // يمكنك حفظ هذا التفضيل في Preferences ليتم تحميله عند فتح التطبيق مجددًا
            Preferences.Set("AppTheme", (int)theme);
        }

        private async Task OpenWhatsApp()
        {
            string phoneNumber = "01098673012";
            string whatsappUrl = $"whatsapp://send?phone=+2{phoneNumber}"; // +2 Egypt country code

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
            string telegramUrl = $"tg://resolve?phone=+2{phoneNumber}"; // +2 Egypt country code

            try
            {
                await Launcher.OpenAsync(telegramUrl);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("خطأ", "تليجرام غير مثبت أو حدث خطأ.", "موافق");
            }
        }
    }
}