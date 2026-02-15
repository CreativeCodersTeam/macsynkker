using CreativeCoders.Cli.Core;
using CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[assembly: CliCommandGroup([UserDefaultsCommandGroup.Name], "Commands for managing MacOS user defaults")]

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

public static class UserDefaultsCommandGroup
{
    public const string Name = "defaults";
}
