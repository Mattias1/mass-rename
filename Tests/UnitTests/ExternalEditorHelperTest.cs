using FluentAssertions;
using MassRename.UI;
using Xunit;

namespace Tests.UnitTests;

public class ExternalEditorHelperTest {
  [Fact]
  public void ParseNormalNameAndArgs() {
    var (cmd, args) = ExternalEditorHelper.ParseCommand("gvim --no-fork");
    cmd.Should().Be("gvim");
    args.Should().Be("--no-fork");
  }

  [Fact]
  public void ParseCommandWithEscapedSpace() {
    var (cmd, args) = ExternalEditorHelper.ParseCommand("C:/Program\\ Files/Vim/vim8/gvim.exe --music --open C:/Some\\ Dir");
    cmd.Should().Be("C:/Program\\ Files/Vim/vim8/gvim.exe");
    args.Should().Be("--music --open C:/Some\\ Dir");
  }

  [Fact]
  public void ParseCommandWithSpaceInQuotes() {
    var (cmd, args) = ExternalEditorHelper.ParseCommand("\"C:\\Program Files\\Vim\\vim8\\gvim.exe\" --music --open \"C:\\Some Dir\"");
    cmd.Should().Be("\"C:\\Program Files\\Vim\\vim8\\gvim.exe\"");
    args.Should().Be("--music --open \"C:\\Some Dir\"");
  }

  [Fact]
  public void ParseEmptyString() {
    var (cmd, args) = ExternalEditorHelper.ParseCommand("");
    cmd.Should().Be("");
    args.Should().Be("");
  }
}
