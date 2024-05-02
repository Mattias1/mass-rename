namespace MassRename;

public class Settings {
  public const string DEFAULT_EDITOR = "gvim --nofork";

  public string? LastDir { get; set; }
  public string? Editor { get; set; }
  public bool SetMusicData { get; set; }
  public double Width { get; set; }
  public double Height { get; set; }
}
