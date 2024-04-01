namespace MassRename;

public static class FileManipulator {
  public static string? RenameFiles(string? pathPrefix, string? oldNames, string? newNames) {
    if (string.IsNullOrWhiteSpace(pathPrefix)) {
      return "No path selected";
    }
    if (string.IsNullOrWhiteSpace(oldNames) || string.IsNullOrWhiteSpace(newNames)) {
      return "No filenames provided (from or to)";
    }

    char[] delimiter = Environment.NewLine.ToCharArray();
    string[] original = oldNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
    string[] replacement = newNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
    string prefix = pathPrefix + Path.DirectorySeparatorChar;

    if (original.Length == 0) {
      return "No files to rename";
    }
    if (original.Length != replacement.Length) {
      return "The number of files is not equal";
    }

    for (int i = 0; i < original.Length; i++) {
      try {
        string originalPath = prefix + original[i];
        string newPath = prefix + replacement[i];

        if (originalPath == newPath) {
          continue;
        }

        if (IsDirectory(originalPath)) {
          Directory.Move(originalPath, newPath);
        } else {
          File.Move(originalPath, newPath);
        }
      } catch (Exception ex) {
        return $"Error trying to rename the file (#{i}): {original[i]}{Environment.NewLine}Message: {ex.Message}";
      }
    }
    return null;
  }

  // Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist.
  private static bool IsDirectory(string path) {
    if (Directory.Exists(path) || File.Exists(path)) {
      var fileAttr = File.GetAttributes(path);
      return (fileAttr & FileAttributes.Directory) == FileAttributes.Directory;
    }
    throw new FileNotFoundException("The path doesn't exist.");
  }
}
