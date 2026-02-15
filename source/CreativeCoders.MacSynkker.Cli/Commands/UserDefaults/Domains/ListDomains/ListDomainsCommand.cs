using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Text;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains.ListDomains;

[UsedImplicitly]
[CliCommand([UserDefaultsCommandGroup.Name, UserDefaultsDomainsCommandGroup.Name, "list"],
    Description = "Lists all domains for user defaults")]
public class ListDomainsCommand(IAnsiConsole ansiConsole, IUserDefaultsEnumerator userDefaultsEnumerator)
    : ICliCommand<ListDomainsOptions>
{
    private readonly IUserDefaultsEnumerator _userDefaultsEnumerator = Ensure.NotNull(userDefaultsEnumerator);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(ListDomainsOptions options)
    {
        _ansiConsole.WriteLines("Show all user defaults domains", string.Empty);

        var domains = await _userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(options.FilterPattern))
        {
            domains = domains.Where(x => PatternMatcher.MatchesPattern(x, options.FilterPattern)).ToArray();
        }

        _ansiConsole.WriteLines(domains.OrderBy(x => x).Select(x => $"- {x}").ToArray());

        return new CommandResult();
    }
}
