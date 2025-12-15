using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[CliCommand(["defaults", "domains", "list"], Description = "Lists all domains for user defaults")]
public class ListDomainsCommand(IAnsiConsole ansiConsole, IUserDefaults userDefaults) : ICliCommand
{
    private readonly IUserDefaults _userDefaults = Ensure.NotNull(userDefaults);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync()
    {
        _ansiConsole.PrintBlock()
            .WriteLine("Show all user defaults domains")
            .WriteLine();

        var domains = await _userDefaults.GetDomainsAsync().ConfigureAwait(false);

        domains.OrderBy(x => x.Name).ForEach(x =>
            _ansiConsole.WriteLine(x.Name));

        return new CommandResult();
    }
}
