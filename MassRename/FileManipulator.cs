using System.Text;

namespace MassRename;

public static class FileManipulator {
  public static (string from, string to) DetermineTextboxData(string pathPrefix, string[] fileNames, bool setMusicData) {
    var sbOld = new StringBuilder();
    var sbNew = new StringBuilder();
    if (setMusicData) {
      sbOld.AppendLine("## Filename");
      sbNew.AppendLine("## Filename | Title | Album | Artists");
    }

    foreach (string fileName in fileNames) {
      sbOld.AppendLine(fileName);
      if (setMusicData) {
        try {
          var musicData = GetMusicData(pathPrefix, fileName);
          sbNew.AppendLine(musicData.ToString());
        } catch {
          sbNew.AppendLine(fileName);
        }
      } else {
        sbNew.AppendLine(fileName);
      }
    }
    return (sbOld.ToString(), sbNew.ToString());
  }

  public static string? RenameFiles(string? pathPrefix, string? oldNames, string? newNames, bool setMusicData) {
    if (string.IsNullOrWhiteSpace(pathPrefix)) {
      return "No path selected";
    }
    if (string.IsNullOrWhiteSpace(oldNames) || string.IsNullOrWhiteSpace(newNames)) {
      return "No filenames provided (from or to)";
    }

    var original = ParseFileList(oldNames);
    string[] replacement = ParseFileList(newNames);
    string prefix = pathPrefix + Path.DirectorySeparatorChar;

    if (original.Length == 0) {
      return "No files to rename";
    }
    if (original.Length != replacement.Length) {
      return "The number of files is not equal";
    }

    for (int i = 0; i < original.Length; i++) {
      try {
        string originalPath = prefix + original[i];
        var musicData = setMusicData ? MusicData.FromString(replacement[i]) : null;
        string newPath = prefix + (musicData?.FileName ?? replacement[i]);

        if (originalPath == newPath && !setMusicData) {
          continue;
        }

        if (IsDirectory(originalPath)) {
          Directory.Move(originalPath, newPath);
        } else {
          File.Move(originalPath, newPath);
          SetMusicData(newPath, musicData);
        }
      } catch (Exception ex) {
        return $"Error trying to rename the file (#{i}): {original[i]}{Environment.NewLine}Message: {ex.Message}";
      }
    }
    return null;
  }

  private static string[] ParseFileList(string oldNames) {
    char[] delimiter = Environment.NewLine.ToCharArray();
    string[] original = oldNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
    if (original.Length > 0 && original[0].StartsWith("##")) {
      original = original.Skip(1).ToArray();
    }
    return original;
  }

  // Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist.
  private static bool IsDirectory(string path) {
    if (Directory.Exists(path) || File.Exists(path)) {
      var fileAttr = File.GetAttributes(path);
      return (fileAttr & FileAttributes.Directory) == FileAttributes.Directory;
    }
    throw new FileNotFoundException("The path doesn't exist.");
  }

  private static void SetMusicData(string path, MusicData? musicData) {
    if (musicData is null) {
      return;
    }

    var file = TagLib.File.Create(path);
    file.Tag.Title = musicData.Title;
    file.Tag.Album = musicData.Album;
    file.Tag.Performers = musicData.Artists?.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    file.Tag.AlbumArtists = file.Tag.Performers;
    file.Save();
  }

  public static MusicData GetMusicData(string pathPrefix, string fileName) {
    var file = TagLib.File.Create(Path.Join(pathPrefix, fileName));
    return new MusicData(fileName, file.Tag.Title, file.Tag.Album, file.Tag.JoinedPerformers);
  }

  public record MusicData(string FileName, string? Title, string? Album, string? Artists) {
    public override string ToString() => ToString('|');
    public string ToString(char sep) => $"{FileName} {sep} {Title} {sep} {Album} {sep} {Artists}";

    public static MusicData FromString(string raw, char sep = '|') {
      var data = raw.Split(sep, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
      return new MusicData(GetOrEmpty(data, 0), GetOrEmpty(data, 1), GetOrEmpty(data, 2), GetOrEmpty(data, 3));
    }

    private static string GetOrEmpty(string[] data, int i) => data.Length > i ? data[i] : "";
  }
}
