// Services/JsonDataService.cs
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KifeApp.Models; // تأكد من استيراد النماذج

namespace KifeApp.Services
{
   public class JsonDataService
    {
        private const string FileName = "final.json";
        private const string FilePath = "Resources\\Raw\\final.json"; // المسار داخل حزمة التطبيق

        public async Task<List<PlayListModel>> LoadDataAsync()
        {
            try
            {
                using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(FileName);
                using StreamReader reader = new StreamReader(fileStream);
                string jsonContent = await reader.ReadToEndAsync();

                var rawPlaylists = JsonSerializer.Deserialize<List<RawPlayListModel>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // لتجاهل حساسية الأحرف في أسماء الخصائص
                });

                // تحويل RawPlayListModel إلى PlayListModel الجديد
                var playlists = new List<PlayListModel>();
                foreach (var rawPlaylist in rawPlaylists)
                {
                    playlists.Add(new PlayListModel(rawPlaylist.playlist, rawPlaylist.videos));
                }

                return playlists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON data: {ex.Message}");
                // يمكنك التعامل مع الخطأ بشكل أفضل هنا، مثلاً عرض تنبيه للمستخدم
                return new List<PlayListModel>();
            }
        }
    }
}