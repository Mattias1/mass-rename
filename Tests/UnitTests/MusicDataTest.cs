using FluentAssertions;
using MassRename;
using Xunit;

namespace Tests.UnitTests;

public class MusicDataTest {
  [Fact]
  public void MusicDataFromString() {
    var musicData = FileManipulator.MusicData.FromString("test.mp3 | title | album | artist1, artist2  ");
    musicData.FileName.Should().Be("test.mp3");
    musicData.Title.Should().Be("title");
    musicData.Album.Should().Be("album");
    musicData.Artists.Should().Be("artist1, artist2");
  }

  [Fact]
  public void MusicDataToString() {
    var musicData = new FileManipulator.MusicData("test.mp3", "title", "album", "artist1, artist2");
    musicData.ToString().Should().Be("test.mp3 | title | album | artist1, artist2");
  }
}
