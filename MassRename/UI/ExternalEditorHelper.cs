using System.Diagnostics;

namespace MassRename.UI;

public static class ExternalEditorHelper {
  public static async Task<string> OpenAsync(Settings settings, string? content) {
    if (string.IsNullOrWhiteSpace(settings.Editor)) {
      settings.Editor = Settings.DEFAULT_EDITOR;
    }

    var tempFile = Path.Join(Path.GetTempPath(), "/mass-rename-temp");
    await File.WriteAllTextAsync(tempFile, content);

    await StartProcessAsync(settings.Editor.Split(' ', StringSplitOptions.RemoveEmptyEntries).Append(tempFile));

    string output = await File.ReadAllTextAsync(tempFile);
    File.Delete(tempFile);
    return output;
  }

  private static async Task StartProcessAsync(IEnumerable<string> input) {
    // ReSharper disable twice PossibleMultipleEnumeration
    var process = new Process();
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.FileName = input.First();
    process.StartInfo.Arguments = string.Join(' ', input.Skip(1));
    process.Start();
    await process.WaitForExitAsync();
  }
}
