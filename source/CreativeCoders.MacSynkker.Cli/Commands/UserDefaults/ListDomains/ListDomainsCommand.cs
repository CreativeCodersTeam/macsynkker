using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.ListDomains;

[UsedImplicitly]
[CliCommand([UserDefaultsCommandGroup.Name, "domains", "list"], Description = "Lists all domains for user defaults")]
public class ListDomainsCommand(IAnsiConsole ansiConsole, IUserDefaultsEnumerator userDefaultsEnumerator) : ICliCommand
{
    private readonly IUserDefaultsEnumerator _userDefaultsEnumerator = Ensure.NotNull(userDefaultsEnumerator);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync()
    {
        _ansiConsole.PrintBlock()
            .WriteLine("Show all user defaults domains")
            .WriteLine();

        var domains = await _userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false);

        domains.OrderBy(x => x).ForEach(x =>
            _ansiConsole.WriteLine(x));

        return new CommandResult();
    }
}
