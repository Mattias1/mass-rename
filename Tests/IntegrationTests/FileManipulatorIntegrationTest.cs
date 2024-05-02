using FluentAssertions;
using MassRename;
using System.Reflection;
using Xunit;

namespace Tests.IntegrationTests;

public class FileManipulatorIntegrationTest {
  /*
    That whoosh sound file is from https://freesound.org/people/qubodup/sounds/60013/
    > qubodup - September 7th, 2008:
    > I use a bamboo stick to generate some wind *swash* sounds. I used following bamboo stick:
    > http://farm3.staticflickr.com/2393/2821170610_ebf7605a32_m.jpg
    > Find more similar sounds in the bamboo stick swosh, whoosh, whosh, swhoosh... sounds pack.
    > This recording was made with a Zoom H2.
    > I release it to the public domain.
    */
  private string RootDir { get; } = Path.GetDirectoryName(Assembly.GetAssembly(
      typeof(FileManipulatorIntegrationTest))?.Location) ?? throw new InvalidOperationException("No rootdir found");

  [Fact]
  public void TestAllTheThings() {
    // Test the whole FileManipulator class in one test. Mostly because the setup is annoying otherwise

    // Setup
    SetupMusicFile().Should().BeTrue("it should've copied the music file");

    // Empty file
    var musicData = FileManipulator.GetMusicData(".", "whoosh.flac");
    musicData.FileName.Should().Be("whoosh.flac");
    musicData.Title.Should().BeNullOrEmpty();
    musicData.Album.Should().BeNullOrEmpty();
    musicData.Artists.Should().StartWith("qubodup");

    // Store data
    var error = FileManipulator.RenameFiles(".", "whoosh.flac", "## Some comment\n"
        + "whoosh-edit.flac | w-title | w-album | w-artist1; w-artist2", true);
    error.Should().BeNull();

    // Verify data
    musicData = FileManipulator.GetMusicData(".", "whoosh-edit.flac");
    musicData.FileName.Should().Be("whoosh-edit.flac");
    musicData.Title.Should().Be("w-title");
    musicData.Album.Should().Be("w-album");
    musicData.Artists.Should().Be("w-artist1; w-artist2");
  }

  private bool SetupMusicFile() {
    foreach (var file in Directory.EnumerateFiles(RootDir, "*.flac")) {
      File.Delete(file); // Remove potential artifacts from previous test runs
    }

    var to = Path.GetFullPath(Path.Join(RootDir, "whoosh.flac"));
    var possiblePaths = new[] { "assets/", "Tests/assets/", "../assets/" };
    foreach (var possiblePath in possiblePaths) {
      try {
        var from = Path.GetFullPath(Path.Join(RootDir, possiblePath, "60013__qubodup__whoosh.flac"));
        File.Copy(from, to);
        return true;
      } catch (Exception) {
        // We'll try another path, it might depend on where this test is called from
      }
    }
    return false;
  }
}
