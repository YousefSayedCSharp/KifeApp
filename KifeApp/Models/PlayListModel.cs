//namespace KifeApp.Models;

//public class PlayListModel
//{
//    public string? playlist { get; set; }
//    public List<VideoModel>? videos { get; set; }
//}
//1//

// Models/PlayListModel.cs
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace KifeApp.Models;

public class PlayListModel : ObservableCollection<VideoModel>
{
    public string? playlist { get; set; }

    public PlayListModel(string? playlistName, IEnumerable<VideoModel>? videos) : base(videos ?? new List<VideoModel>())
    {
        playlist = playlistName;
    }

    // باني افتراضي مطلوب للـ CollectionView.IsGrouped ليعمل بشكل صحيح
    public PlayListModel() : base() { }
}