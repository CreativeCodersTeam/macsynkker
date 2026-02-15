using System.IO.Abstractions;
using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.UserDefaults;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains.ImportDomain;

[UsedImplicitly]
[CliCommand([UserDefaultsCommandGroup.Name, UserDefaultsDomainsCommandGroup.Name, "import"],
    Description = "Imports a MacOS user defaults domain from a plist file")]
public class ImportDomainCommand(
    IFileSystem fileSystem,
    IAnsiConsole ansiConsole,
    IUserDefaultsImporter userDefaultsImporter) : ICliCommand<ImportDomainOptions>
{
    private readonly IUserDefaultsImporter _userDefaultsImporter = Ensure.NotNull(userDefaultsImporter);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IFileSystem _fileSystem = Ensure.NotNull(fileSystem);

    public async Task<CommandResult> ExecuteAsync(ImportDomainOptions options)
    {
        if (_fileSystem.Directory.Exists(options.InputPath))
        {
            return await ImportAllDomainsAsync(options).ConfigureAwait(false);
        }

        if (!_fileSystem.File.Exists(options.InputPath))
        {
            _ansiConsole.MarkupLine($"[red]File or directory not found: {options.InputPath}[/]");

            return MacSynkkerCliExitCodes.FileNotFound;
        }

        await ImportDomainAsync(options.DomainName, options.InputPath).ConfigureAwait(false);

        return CommandResult.Success;
    }

    private async Task<CommandResult> ImportAllDomainsAsync(ImportDomainOptions options)
    {
        var plistFiles = _fileSystem.Directory.EnumerateFiles(options.InputPath, "*.plist");

        foreach (var plistFile in plistFiles)
        {
            await ImportDomainAsync(string.Empty, plistFile).ConfigureAwait(false);
        }

        return CommandResult.Success;
    }

    private async Task ImportDomainAsync(string domainName, string filePath)
    {
        if (string.IsNullOrWhiteSpace(domainName))
        {
            domainName = _fileSystem.Path.GetFileNameWithoutExtension(filePath);
        }

        _ansiConsole.Write($"Importing domain '{domainName}' from '{filePath}' ... ");

        await _userDefaultsImporter.ImportDomainAsync(domainName, filePath)
            .ConfigureAwait(false);

        _ansiConsole.MarkupLine("[green]Done[/]");
    }
}
