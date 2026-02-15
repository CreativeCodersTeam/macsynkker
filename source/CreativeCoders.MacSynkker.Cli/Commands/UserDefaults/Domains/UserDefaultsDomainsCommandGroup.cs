using CreativeCoders.Cli.Core;
using CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;
using CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains;

[assembly: CliCommandGroup([UserDefaultsCommandGroup.Name, UserDefaultsDomainsCommandGroup.Name],
    "Commands for managing MacOS user defaults domains")]

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains;

public static class UserDefaultsDomainsCommandGroup
{
    public const string Name = "domains";
}
