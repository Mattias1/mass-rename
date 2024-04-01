namespace MassRename;

public class Args {
  public string? InitialDirectory { get; set; }

  public static Args ParseFrom(string[] args) {
    if (args.Length == 0) {
      return new Args();
    }

    return new Args {
        InitialDirectory = args[0]
    };
  }
}
