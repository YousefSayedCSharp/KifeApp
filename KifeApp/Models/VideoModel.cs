// Models/VideoModel.cs
using System;

namespace KifeApp.Models;

public class VideoModel
{
    public string? Title { get; set; }
    public string? URL { get; set; }
    public DateTime? UploadDate { get; set; }
    public string? Duration { get; set; }
    public string? Thumbnail { get; set; }

    // خاصية محسوبة (ReadOnly)
    public string Row
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Title))
                parts.Add(Title);

            if (UploadDate.HasValue)
                parts.Add($"تاريخ النشر: {UploadDate.Value:dd/MM/yyyy}");

            if (!string.IsNullOrWhiteSpace(Duration))
                parts.Add($"المدة: {Duration}");

            return string.Join(", ", parts);
        }
    }

}