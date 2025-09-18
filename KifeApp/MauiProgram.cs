// MauiProgram.cs
using Microsoft.Extensions.Logging;
using KifeApp.Services;
using KifeApp.ViewModels;
using KifeApp.Views;
using System.Net.Http; // تأكد من استيراد هذه

namespace KifeApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("FA6Brands.otf", "FAB");
                    fonts.AddFont("FA6Regular.otf", "FAR");
                    fonts.AddFont("FA6Solid.otf", "FAS");

                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // تسجيل HttpClient كخدمة Singleton
            builder.Services.AddSingleton<HttpClient>();

            // تسجيل الخدمات (Service)
            builder.Services.AddSingleton<JsonDataService>();

            // تسجيل ViewModels
            builder.Services.AddTransient<FirstPlaylistViewModel>();
            builder.Services.AddTransient<OtherPlaylistsViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            // builder.Services.AddSingleton<AboutViewModel>(); // سيتم استبداله بـ SettingsViewModel جزئياً

            // تسجيل الصفحات (Views)
            builder.Services.AddTransient<FirstPlaylistPage>();
            builder.Services.AddTransient<OtherPlaylistsPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<AboutPage>();
            // إضافة SettingsPage و ViewModel الخاص بها
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

            return builder.Build();
        }
    }
}