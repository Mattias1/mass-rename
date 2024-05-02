using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;
using AvaloniaExtensions;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MassRename.UI;

class MassRenameControl : CanvasComponentBase {
  private Settings? _settings;
  private Settings Settings => _settings ??= GetSettings<Settings>();
  private readonly Args _args;

  private CheckBox _tbMusic = null!;
  private TextBox _tbBrowse = null!, _tbOld = null!, _tbNew = null!;

  public MassRenameControl(Args args) {
    _args = args;
  }

  protected override void InitializeControls() {
    AddButton("Rename", OnRenameClick).TopRightInPanel();
    _tbMusic = AddCheckBox("Music data", OnMusicDataToggle).LeftOf();
    AddButton("Folders", OnBrowseDirClick).LeftOf();
    AddButton("Files", OnBrowseFilesClick).LeftOf();
    _tbBrowse = AddTextBox(Settings.LastDir ?? "").TopLeftInPanel().StretchRightTo();

    _tbOld = AddMultilineTextBox().IsReadOnly(true).Below().StretchFractionRightInPanel(1, 2).StretchDownInPanel();
    _tbNew = AddMultilineTextBox().RightOf().StretchRightInPanel().StretchDownInPanel();
  }

  protected override void OnInitialized() {
    base.OnInitialized();

    if (_args.SetEditor is not null) {
      Settings.Editor = _args.SetEditor;
    }
    if (_args.Music is not null) {
      Settings.SetMusicData = _args.Music.Value;
    }
    _tbMusic.IsChecked(Settings.SetMusicData);

    if (!string.IsNullOrWhiteSpace(_args.InitialDirectory)) {
      SetFilesFromDir(_args.InitialDirectory);
      Settings.LastDir = _args.InitialDirectory;
    }
  }

  protected override void OnSizeChanged(SizeChangedEventArgs e) {
    base.OnSizeChanged(e);
    Settings.Width = e.NewSize.Width;
    Settings.Height = e.NewSize.Height;
  }

  private async void OnBrowseFilesClick(RoutedEventArgs e) { // Note: async void event handler
    try {
      var files = await DialogHelper.GetPathsViaFileDialogAsync(FindWindow().StorageProvider, _args.InitialDirectory ?? Settings.LastDir);
      if (files.Count > 0) {
        SetFiles(Path.GetDirectoryName(files.First().Path.AbsolutePath), files.Select(f => f.Name));
      }
    } catch (Exception exc) {
      Console.WriteLine(exc);
      throw;
    }
  }

  private async void OnBrowseDirClick(RoutedEventArgs e) { // Note: async void event handler
    try {
      var dir = await DialogHelper.GetPathViaDirectoryDialogAsync(FindWindow().StorageProvider, _args.InitialDirectory ?? Settings.LastDir);
      SetFilesFromDir(dir.Path.AbsolutePath);
    } catch (Exception exc) {
      Console.WriteLine(exc);
      throw;
    }
  }

  private void SetFilesFromDir(string directoryPath) {
    if (!Directory.Exists(directoryPath)) {
      return;
    }

    var filenames = Directory.GetFiles(directoryPath)
        .Select(p => Path.GetFileName(p))
        .Where(s => !string.IsNullOrWhiteSpace(s));
    SetFiles(directoryPath, filenames);
  }

  private void SetFiles(string? pathPrefix, IEnumerable<string> fileNames) {
    if (pathPrefix is null) {
      return;
    }

    _tbBrowse.Text = pathPrefix;
    Settings.LastDir = pathPrefix;

    var (from, to) = FileManipulator.DetermineTextboxData(pathPrefix, fileNames.ToArray(), Settings.SetMusicData);

    _tbOld.Text = from;
    _tbNew.Text = to;
    _tbNew.Focus();
  }

  private void OnMusicDataToggle(RoutedEventArgs obj) {
    Settings.SetMusicData = (obj.Source as ToggleButton)?.IsChecked ?? throw new InvalidOperationException("Cast fail");

    var filenames = _tbOld.Text?.Split(Environment.NewLine.ToCharArray()) ?? [];
    filenames = filenames.Skip(Settings.SetMusicData ? 0 : 1).ToArray();
    SetFiles(_tbBrowse.Text, filenames);
  }

  private async void OnRenameClick(RoutedEventArgs e) { // Note: async void event handler
    string? errorMessage;
    try {
      errorMessage = FileManipulator.RenameFiles(_tbBrowse.Text, _tbOld.Text, _tbNew.Text, Settings.SetMusicData);
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
