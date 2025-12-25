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
            .WriteLine($"Export domain '{options.DomainName}' to '{options.OutputPath}'")
            .WriteLine();

        await _userDefaultsExporter.ExportDomainAsync(options.DomainName, options.OutputPath, options.PlistFormat)
            .ConfigureAwait(false);

        return new CommandResult();
    }

    private async Task<CommandResult> ExportAllDomainsAsync(ExportDomainOptions options)
    {
        _ansiConsole.PrintBlock()
            .WriteLine($"Export all domains to '{options.OutputPath}'")
            .WriteLine();

        var domainNames = (await _userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false))
            .ToArray();

        foreach (var domainName in domainNames)
        {
            _ansiConsole.Write($"Exporting domain '{domainName}' ... ");

            var outputFileName = Path.Combine(options.OutputPath, $"{domainName}.plist");

            await _userDefaultsExporter.ExportDomainAsync(domainName, outputFileName, options.PlistFormat)
                .ConfigureAwait(false);

            _ansiConsole.MarkupLine("[green]Done[/]");
        }

        _ansiConsole.WriteLine();
        _ansiConsole.WriteLine($"Exported {domainNames.Length} domains to '{options.OutputPath}'");

        return new CommandResult();
    }
}
