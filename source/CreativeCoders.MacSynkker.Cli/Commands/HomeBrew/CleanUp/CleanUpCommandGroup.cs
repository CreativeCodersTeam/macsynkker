using CreativeCoders.Cli.Core;
using CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;
using CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

[assembly: CliCommandGroup([HomebrewCommandGroup.Name, CleanUpCommandGroup.Name],
    "Commands for cleaning up Homebrew caches")]

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

public static class CleanUpCommandGroup
{
    public const string Name = "cleanup";
}
