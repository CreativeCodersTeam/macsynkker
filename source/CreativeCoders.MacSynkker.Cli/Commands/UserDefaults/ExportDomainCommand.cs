using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[UsedImplicitly]
[CliCommand(["defaults", "domains", "export"])]
public class ExportDomainCommand(
    IUserDefaultsExporter userDefaultsExporter,
    IUserDefaultsEnumerator userDefaultsEnumerator,
    IAnsiConsole ansiConsole)
    : ICliCommand<ExportDomainOptions>
{
    private readonly IUserDefaultsEnumerator _userDefaultsEnumerator = Ensure.NotNull(userDefaultsEnumerator);

    private readonly IUserDefaultsExporter _userDefaultsExporter = Ensure.NotNull(userDefaultsExporter);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(ExportDomainOptions options)
    {
        if (options.ExportAllDomains)
        {
            return await ExportAllDomainsAsync(options).ConfigureAwait(false);
        }

        _ansiConsole.PrintBlock()
            .WriteLine($"Export domain '{options.DomainName}' to '{options.OutputFileName}'")
            .WriteLine();

        await _userDefaultsExporter.ExportDomainAsync(options.DomainName, options.OutputFileName, options.PlistFormat)
            .ConfigureAwait(false);

        return new CommandResult();
    }

    private async Task<CommandResult> ExportAllDomainsAsync(ExportDomainOptions options)
    {
        _ansiConsole.PrintBlock()
            .WriteLine($"Export domain '{options.DomainName}' to '{options.OutputFileName}'")
            .WriteLine();

        var domainNames = await _userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false);

        await _userDefaultsExporter.ExportDomainsAsync(domainNames, options.OutputFileName, options.PlistFormat)
            .ConfigureAwait(false);

        return new CommandResult();
    }
}
