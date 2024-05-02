using System.Diagnostics;

namespace MassRename.UI;

public static class ExternalEditorHelper {
  public static async Task<string> OpenAsync(Settings settings, string? content) {
    if (string.IsNullOrWhiteSpace(settings.Editor)) {
      settings.Editor = Settings.DEFAULT_EDITOR;
    }

    var tempFile = Path.Join(Path.GetTempPath(), "/mass-rename-temp");
    await File.WriteAllTextAsync(tempFile, content);

    await StartProcessAsync($"{settings.Editor} {tempFile}");

    string output = await File.ReadAllTextAsync(tempFile);
    File.Delete(tempFile);
    return output;
  }

  public static (string cmd, string args) ParseCommand(string nameAndArgs) {
    // Case for quotes
    if (nameAndArgs.Length > 2 && nameAndArgs[0] == '"') {
      int i = nameAndArgs.IndexOf('"', 1);
      if (i > 0) {
        return (nameAndArgs.Substring(0, i + 1), nameAndArgs.Substring(i + 1).Trim());
      }
    }

    // Case for everything else (complications is escaped space char)
    bool previousIsEscapeChar = false;
    for (int i = 0; i < nameAndArgs.Length; i++) {
      if (nameAndArgs[i] == ' ') {
        if (!previousIsEscapeChar) {
          return (nameAndArgs.Substring(0, i), nameAndArgs.Substring(i).Trim());
        }
      }
      previousIsEscapeChar = nameAndArgs[i] == '\\';
    }
    return (nameAndArgs, "");
  }

  private static async Task StartProcessAsync(string nameAndArgs) {
    var (cmd, args) = ParseCommand(nameAndArgs);

    var process = new Process();
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.FileName = cmd;
    process.StartInfo.Arguments = args;
    process.Start();
    await process.WaitForExitAsync();
  }
}
