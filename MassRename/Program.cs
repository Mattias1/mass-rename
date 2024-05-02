using Avalonia;
using AvaloniaExtensions;
using Avalonia.Markup.Declarative;
using MassRename;
using MassRename.UI;

var parsedArgs = Args.ParseFrom(args);
if (parsedArgs.PrintedHelp) {
  return;
}

var minSize = new Size(700, 400);
AvaloniaExtensionsApp.Init()
  .WithSettingsFile<Settings>("./mass-rename-settings.json")
  .StartDesktopApp(() => ExtendedWindow.Init("Mass rename tool", () => new MassRenameControl(parsedArgs))
    .WithSize(size: GetSizeFromSettings(minSize), minSize: minSize)
    .Icon(AssetExtensions.LoadWindowIcon("assets/eyes.ico")));

static Size GetSizeFromSettings(Size minSize) {
  var settings = SettingsFiles.Get.GetSettings<Settings>();
  return new Size(Math.Max(minSize.Width, settings.Width), Math.Max(minSize.Height, settings.Height));
}
