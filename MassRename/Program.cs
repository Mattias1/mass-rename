using Avalonia;
using AvaloniaExtensions;
using Avalonia.Markup.Declarative;
using MassRename;

var parsedArgs = Args.ParseFrom(args);

var minSize = new Size(700, 400);
AvaloniaExtensionsApp.Init()
  .WithSettingsFile<Settings>("./mass-rename-settings.json")
  .StartDesktopApp(() => ExtendedWindow.Init("Mass rename tool", () => new MassRenameControl(parsedArgs))
    .WithSize(size: SettingsFiles.Get.GetSettings<Settings>().Size ?? minSize, minSize: minSize)
    .Icon(AssetExtensions.LoadWindowIcon("assets/eyes.ico")));
