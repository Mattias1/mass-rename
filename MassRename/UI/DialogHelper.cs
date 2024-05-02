using Avalonia.Platform.Storage;

namespace MassRename.UI;

public static class DialogHelper {
  public static async Task<IStorageFolder> GetPathViaDirectoryDialogAsync(IStorageProvider storageProvider, string? directory) {
    var location = await GetFolderAsync(storageProvider, directory);
    var options = new FolderPickerOpenOptions {
        Title = "Rename all folders and files in the selected folder",
        SuggestedStartLocation = location,
        AllowMultiple = false
    };

    var directories = await storageProvider.OpenFolderPickerAsync(options).ConfigureAwait(true);
    return directories.Single();
  }

  public static async Task<IReadOnlyList<IStorageFile>> GetPathsViaFileDialogAsync(IStorageProvider storageProvider, string? directory) {
    var location = await GetFolderAsync(storageProvider, directory);
    var options = new FilePickerOpenOptions {
        Title = "The files to rename",
        SuggestedStartLocation = location,
        AllowMultiple = true
    };

    return await storageProvider.OpenFilePickerAsync(options).ConfigureAwait(true);
  }

  private static async Task<IStorageFolder?> GetFolderAsync(IStorageProvider storageProvider, string? path) {
    try {
      return path is null ? null : await storageProvider.TryGetFolderFromPathAsync(new Uri(path));
    } catch {
      return null;
    }
  }
}
