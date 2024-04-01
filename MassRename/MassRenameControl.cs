using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaExtensions;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Text;

namespace MassRename;

class MassRenameControl : CanvasComponentBase {
  private Settings? _settings;
  private Settings Settings => _settings ??= GetSettings<Settings>();
  private readonly Args _args;

  private TextBox _tbBrowse = null!;
  private TextBox _tbOld = null!, _tbNew = null!;

  public MassRenameControl(Args args) {
    _args = args;
  }

  protected override void InitializeControls() {
    AddButton("Rename", OnRenameClick).TopRightInPanel();
    AddButton("Browse dir", OnBrowseDirClick).LeftOf();
    var btnBrowseFiles = AddButton("Browse files", OnBrowseFilesClick).LeftOf();
    _tbBrowse = AddTextBox(Settings.LastDir ?? "").TopLeftInPanel().StretchRightTo(btnBrowseFiles);

    _tbOld = AddMultilineTextBox().Below().StretchFractionRightInPanel(1, 2).StretchDownInPanel();
    _tbNew = AddMultilineTextBox().RightOf().StretchRightInPanel().StretchDownInPanel();
  }

  protected override void OnSizeChanged(SizeChangedEventArgs e) {
    base.OnSizeChanged(e);
    Settings.Size = e.NewSize;
  }

  private async void OnBrowseFilesClick(RoutedEventArgs e) { // Note: async void event handler
    try {
      var files = await GetPathsViaFileDialogAsync();
      if (files.Count > 0) {
        SetFiles(Path.GetDirectoryName(files.First().Path.AbsolutePath), files.Select(f => f.Name));
      }
    } catch (Exception exc) {
      Console.WriteLine(exc);
      throw;
    }
  }

  private async Task<IReadOnlyList<IStorageFile>> GetPathsViaFileDialogAsync() {
    var storageProvider = FindWindow().StorageProvider;

    var location = await GetFolderAsync(storageProvider, _args.InitialDirectory ?? Settings.LastDir);
    var options = new FilePickerOpenOptions {
        Title = "The files to rename",
        SuggestedStartLocation = location,
        AllowMultiple = true
    };

    return await storageProvider.OpenFilePickerAsync(options).ConfigureAwait(true);
  }

  private static async Task<IStorageFolder?> GetFolderAsync(IStorageProvider storageProvider, string? path) {
    return path is null ? null : await storageProvider.TryGetFolderFromPathAsync(new Uri(path));
  }

  private async void OnBrowseDirClick(RoutedEventArgs e) { // Note: async void event handler
    try {
      var dir = await GetPathViaDirectoryDialogAsync();
      SetFilesFromDir(dir.Path.AbsolutePath);
    } catch (Exception exc) {
      Console.WriteLine(exc);
      throw;
    }
  }

  private async Task<IStorageFolder> GetPathViaDirectoryDialogAsync() {
    var storageProvider = FindWindow().StorageProvider;

    var location = await GetFolderAsync(storageProvider, _args.InitialDirectory ?? Settings.LastDir);
    var options = new FolderPickerOpenOptions {
        Title = "Rename all folders and files in the selected folder",
        SuggestedStartLocation = location,
        AllowMultiple = false
    };

    var directories = await storageProvider.OpenFolderPickerAsync(options).ConfigureAwait(true);
    return directories.Single();
  }

  private void SetFilesFromDir(string directoryPath) {
    if (!Directory.Exists(directoryPath)) {
      return;
    }

    var filenames = Directory.GetFiles(directoryPath)
        .Select(p => Path.GetFileName(p) ?? "")
        .Where(s => !string.IsNullOrWhiteSpace(s));
    SetFiles(directoryPath, filenames);
  }

  private void SetFiles(string? pathPrefix, IEnumerable<string> fileNames) {
    if (pathPrefix is null) {
      return;
    }

    _tbBrowse.Text = pathPrefix;
    Settings.LastDir = pathPrefix;

    var sb = new StringBuilder();
    foreach (string path in fileNames) {
      sb.AppendLine(Path.GetFileName(path));
    }

    _tbOld.Text = sb.ToString();
    _tbNew.Text = _tbOld.Text;
    _tbNew.Focus();
  }

  private async void OnRenameClick(RoutedEventArgs e) { // Note: async void event handler
    string? errorMessage;
    try {
      errorMessage = FileManipulator.RenameFiles(_tbBrowse.Text, _tbOld.Text, _tbNew.Text);
    } catch (Exception exc) {
      errorMessage = "An unknown error occurred.\n" + exc.Message;
    }
    try {
      if (!string.IsNullOrWhiteSpace(errorMessage)) {
        await MessageBoxManager.GetMessageBoxStandard("Error", errorMessage, ButtonEnum.Ok, Icon.Error).ShowAsync();
        return;
      }

      _tbOld.Text = _tbNew.Text;
      _tbNew.Text = "The files are renamed successfully.";
    } catch (Exception exc) {
      Console.Write(exc);
    }
  }
}
