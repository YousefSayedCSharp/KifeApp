// Services/Constants.cs
namespace KifeApp.Services;

public static class Constants
{
    // GitHub repository details (replace with your actual username and repo name)
    private const string GitHubUser = "YousefSayedCSharp";
    private const string GitHubRepo = "KifeApp";
    private const string GitHubBranch = "master"; // أو main حسب فرعك الرئيسي

    // Base URL for raw content on GitHub
    private static string GitHubRawContentBaseUrl => $"https://raw.githubusercontent.com/{GitHubUser}/{GitHubRepo}/{GitHubBranch}/";

    // Paths to update files within your GitHub repository
    private const string UpdatesFolderPath = "KifeApp/Updates/";
    private const string ReleasesFolderPath = "KifeApp/Releases/"; // لوضع ملفات الـ APK

    // Direct URLs for update files
    public static string DataVersionUpdateUrl => $"{GitHubRawContentBaseUrl}{UpdatesFolderPath}DataVersionUpdate.json";
    public static string AppVersionUpdateUrl => $"{GitHubRawContentBaseUrl}{UpdatesFolderPath}AppVersionUpdate.json";
    public static string FinalUpdateDataUrl => $"{GitHubRawContentBaseUrl}{UpdatesFolderPath}finalUpdate.json";

    // Helper to convert a GitHub blob URL to a raw content URL
    // Example input: https://github.com/YousefSayedCSharp/KifeApp/blob/master/KifeApp/Updates/DataVersionUpdate.json
    // Output: https://raw.githubusercontent.com/YousefSayedCSharp/KifeApp/master/KifeApp/Updates/DataVersionUpdate.json
    public static string ConvertGitHubBlobToRawUrl(string blobUrl)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
            return blobUrl;

        // Replace "github.com" with "raw.githubusercontent.com" and remove "/blob"
        return blobUrl.Replace("https://github.com/", "https://raw.githubusercontent.com/").Replace("/blob/", "/");
    }
}