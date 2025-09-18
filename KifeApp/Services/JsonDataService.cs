using System.Text.Json;
using KifeApp.Models;

namespace KifeApp.Services;

public class JsonDataService
{
    // اسم الملف الأصلي داخل حزمة التطبيق
    private const string InitialJsonFileName = "final.json";
    // اسم الملف الذي سيتم حفظه وتحديثه في AppDataDirectory
    private const string AppDataJsonFileName = "kife_data.json";

    // مسار ملف البيانات في AppDataDirectory
    private string AppDataFilePath => Path.Combine(FileSystem.AppDataDirectory, AppDataJsonFileName);

    private readonly HttpClient _httpClient;

    // مفتاح لتخزين رقم إصدار البيانات في Preferences
    private const string DataVersionKey = "CurrentDataVersion";

    public JsonDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // خاصية للوصول إلى رقم إصدار البيانات الحالي
    public int CurrentDataVersion => Preferences.Get(DataVersionKey, 0);

    /// <summary>
    /// يحمل البيانات من ملف JSON. يفضل قراءتها من AppDataDirectory،
    /// وإذا لم يكن الملف موجوداً هناك، يقرأ من Resources/Raw ثم يحفظ نسخة في AppDataDirectory.
    /// </summary>
    public async Task<List<PlayListModel>> LoadDataAsync()
    {
        string jsonContent;

        // 1. حاول قراءة الملف من AppDataDirectory
        if (File.Exists(AppDataFilePath))
        {
            Console.WriteLine($"Loading data from AppDataDirectory: {AppDataFilePath}");
            jsonContent = await File.ReadAllTextAsync(AppDataFilePath);
        }
        else
        {
            // 2. إذا لم يكن موجوداً، اقرأ من Resources/Raw وحفظ نسخة
            Console.WriteLine($"Loading data from Resources/Raw: {InitialJsonFileName}");
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(InitialJsonFileName);
            using StreamReader reader = new StreamReader(fileStream);
            jsonContent = await reader.ReadToEndAsync();

            // حفظ نسخة في AppDataDirectory للاستخدامات المستقبلية والتحديثات
            await File.WriteAllTextAsync(AppDataFilePath, jsonContent);
            Console.WriteLine($"Saved initial data to AppDataDirectory: {AppDataFilePath}");
        }

        return DeserializeJsonContent(jsonContent);
    }

    private List<PlayListModel> DeserializeJsonContent(string jsonContent)
    {
        try
        {
            var rawPlaylists = JsonSerializer.Deserialize<List<RawPlayListModel>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var playlists = new List<PlayListModel>();
            foreach (var rawPlaylist in rawPlaylists)
            {
                playlists.Add(new PlayListModel(rawPlaylist.playlist, rawPlaylist.videos));
            }
            return playlists;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing JSON data: {ex.Message}");
            // في حالة وجود خطأ في تحليل JSON، يمكنك العودة بقائمة فارغة أو التعامل مع الخطأ
            return new List<PlayListModel>();
        }
    }

    /// <summary>
    /// يتحقق من اتصال الإنترنت.
    /// </summary>
    public bool IsInternetAvailable()
    {
        return Connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    /// <summary>
    /// يحمل ملف JSON من رابط مباشر.
    /// </summary>
    public async Task<string?> DownloadJsonFileAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // يرمي استثناء إذا لم يكن الكود 200-299
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading JSON from {url}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// يتحقق من وجود تحديث لملف البيانات ويقوم بتحميله وتحديثه إذا لزم الأمر.
    /// </summary>
    /// <param name="forceUpdate">لتجاهل مقارنة الإصدار وتحديث البيانات على أي حال.</param>
    /// <returns>صحيح إذا تم التحديث، خطأ بخلاف ذلك.</returns>
    public async Task<bool> CheckForAndUpdateDataAsync(bool forceUpdate = false)
    {
        if (!IsInternetAvailable())
        {
            Console.WriteLine("No internet connection. Cannot check for data updates.");
            return false;
        }

        Console.WriteLine("Checking for data updates...");
        var versionJson = await DownloadJsonFileAsync(Constants.DataVersionUpdateUrl);
        if (versionJson == null)
        {
            Console.WriteLine("Could not download data version file.");
            return false;
        }

        try
        {
            var versionInfo = JsonSerializer.Deserialize<DataVersionInfoModel>(versionJson);
            if (versionInfo == null)
            {
                Console.WriteLine("Failed to deserialize data version info.");
                return false;
            }

            int latestVersion = versionInfo.Version;
            int currentVersion = CurrentDataVersion;

            if (forceUpdate || latestVersion > currentVersion)
            {
                Console.WriteLine($"Data update required: Current {currentVersion}, Latest {latestVersion}. {(forceUpdate ? "(Forced)" : "")}");
                var updatedDataJson = await DownloadJsonFileAsync(Constants.FinalUpdateDataUrl);
                if (updatedDataJson == null)
                {
                    Console.WriteLine("Could not download updated data file.");
                    return false;
                }

                // التحقق من أن JSON المحمل صالح قبل الكتابة
                try
                {
                    DeserializeJsonContent(updatedDataJson); // محاولة تحليل للتحقق من الصلاحية
                }
                catch (Exception jsonEx)
                {
                    Console.WriteLine($"Downloaded JSON is invalid: {jsonEx.Message}");
                    return false;
                }

                await File.WriteAllTextAsync(AppDataFilePath, updatedDataJson);
                Preferences.Set(DataVersionKey, latestVersion);
                Console.WriteLine("Data updated successfully.");
                return true;
            }
            else
            {
                Console.WriteLine($"Data is already up to date. Current {currentVersion}, Latest {latestVersion}.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during data update process: {ex.Message}");
            return false;
        }
    }
}
