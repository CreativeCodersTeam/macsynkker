using System.IO.Abstractions;
using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.IO;
using CreativeCoders.Core.Text;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains.ExportDomain;

[UsedImplicitly]
[CliCommand([UserDefaultsCommandGroup.Name, UserDefaultsDomainsCommandGroup.Name, "export"])]
public class ExportDomainCommand(
    IUserDefaultsExporter userDefaultsExporter,
    IUserDefaultsEnumerator userDefaultsEnumerator,
    IAnsiConsole ansiConsole,
    IFileSystem fileSystem)
    : ICliCommand<ExportDomainOptions>
{
    private readonly IFileSystem _fileSystem = Ensure.NotNull(fileSystem);

    private readonly IUserDefaultsEnumerator _userDefaultsEnumerator = Ensure.NotNull(userDefaultsEnumerator);

    private readonly IUserDefaultsExporter _userDefaultsExporter = Ensure.NotNull(userDefaultsExporter);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(ExportDomainOptions options)
    {
        if (options.ExportAllDomains)
        {
            return await ExportAllDomainsAsync(options).ConfigureAwait(false);
        }

        _ansiConsole.Write($"Exporting domain '{options.DomainName}' to '{options.OutputPath}' ... ");

        _fileSystem.Directory.EnsureDirectoryForFileNameExists(options.OutputPath);

        await _userDefaultsExporter.ExportDomainAsync(options.DomainName, options.OutputPath, options.PlistFormat)
            .ConfigureAwait(false);

        _ansiConsole.MarkupLines("[green]Done[/]", string.Empty);

        return CommandResult.Success;
    }

    private async Task<CommandResult> ExportAllDomainsAsync(ExportDomainOptions options)
    {
        _ansiConsole.WriteLines($"Export all domains to '{options.OutputPath}'", string.Empty);

        _fileSystem.Directory.EnsureDirectoryExists(options.OutputPath);

        var domainNames = (await _userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false))
            .ToArray();

        if (!string.IsNullOrWhiteSpace(options.Filter))
        {
            domainNames = domainNames
                .Where(x => PatternMatcher.MatchesPattern(x, options.Filter))
                .ToArray();
        }

        foreach (var domainName in domainNames)
        {
            _ansiConsole.Write($"Exporting domain '{domainName}' ... ");

            var outputFileName = Path.Combine(options.OutputPath, $"{domainName}.plist");

            await _userDefaultsExporter.ExportDomainAsync(domainName, outputFileName, options.PlistFormat)
                .ConfigureAwait(false);

            _ansiConsole.MarkupLine("[green]Done[/]");
        }

        _ansiConsole.WriteLines(string.Empty, $"Exported {domainNames.Length} domains to '{options.OutputPath}'");

        return new CommandResult();
    }
}
