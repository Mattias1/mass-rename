namespace MassRename;

public class Args {
  public string? SetEditor { get; private set; }
  public bool OpenInEditor { get; private set; }
  public string? InitialDirectory { get; private set; }
  public bool? Music { get; private set; }
  public bool PrintedHelp { get; private set; }

  public static Args ParseFrom(string[]? args) {
    var result = new Args();
    for (int i = 0; i < args?.Length; i++) {
      switch (args[i]) {
        case "-h":
        case "--help":
        case "-v":
        case "--version":
          PrintHelp();
          result.PrintedHelp = true;
          break;

        case "--set-editor":
          result.SetEditor = NextArg(args, ref i);
          break;
        case "-o":
        case "--open":
          result.OpenInEditor = true;
          break;

        case "-m":
        case "--music":
          result.Music = true;
          break;
        case "-n":
        case "--no-music":
          result.Music = false;
          break;

        default:
          result.InitialDirectory = args[i];
          break;
      }
    }

    return result;
  }

  private static string NextArg(string[] args, ref int i) => args[++i];

  private static void PrintHelp() {
    Console.WriteLine($"Mass Rename v2");
    Console.WriteLine($"Usage: massrename [options] [positional arguments]");
    Console.WriteLine();
    Console.WriteLine($"All arguments are optional");
    Console.WriteLine();
    Console.WriteLine($"positional arguments:");
    Console.WriteLine($"directory:             Open all files in this directory");
    Console.WriteLine();
    Console.WriteLine($"options:");
    Console.WriteLine($"-m, --music:           Set music data");
    Console.WriteLine($"-n, --no-music:        Don't music data");
    Console.WriteLine($"-o, --open:            Open the files from the given directory in the editor");
    Console.WriteLine($"--set-editor [editor]: Set the editor (default '{Settings.DEFAULT_EDITOR}')");
  }
}
