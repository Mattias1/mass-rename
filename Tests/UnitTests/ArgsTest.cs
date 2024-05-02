using FluentAssertions;
using MassRename;
using Xunit;

namespace Tests.UnitTests;

public class ArgsTest {
  [Fact]
  public void ParseNullArray() {
    var args = Args.ParseFrom(null);
    args.InitialDirectory.Should().BeNull();
    args.Music.Should().BeNull();
    args.OpenInEditor.Should().BeFalse();
    args.SetEditor.Should().BeNull();
  }

  [Fact]
  public void ParseEmptyArray() {
    var args = Args.ParseFrom([]);
    args.InitialDirectory.Should().BeNull();
    args.Music.Should().BeNull();
    args.OpenInEditor.Should().BeFalse();
    args.SetEditor.Should().BeNull();
  }

  [Fact]
  public void ParseOpenInEditor() {
    var args = Args.ParseFrom(["--open"]);
    args.OpenInEditor.Should().BeTrue();
  }

  [Fact]
  public void ParseMusicAndDirectory() {
    var args = Args.ParseFrom(["-m", "/home/test/Music/"]);
    args.InitialDirectory.Should().Be("/home/test/Music/");
    args.Music.Should().BeTrue();
  }

  [Fact]
  public void ParseNoMusicAndNoDirectory() {
    var args = Args.ParseFrom(["--no-music"]);
    args.InitialDirectory.Should().BeNull();
    args.Music.Should().BeFalse();
  }
}
