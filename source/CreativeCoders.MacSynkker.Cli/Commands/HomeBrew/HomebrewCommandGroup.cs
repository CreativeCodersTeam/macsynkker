using CreativeCoders.Cli.Core;
using CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

[assembly: CliCommandGroup([HomebrewCommandGroup.Name], "Commands for managing Homebrew packages")]

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

public static class HomebrewCommandGroup
{
    public const string Name = "brew";
}
