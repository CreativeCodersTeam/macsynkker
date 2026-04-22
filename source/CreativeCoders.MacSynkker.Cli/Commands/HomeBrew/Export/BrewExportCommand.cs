using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Export;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Export;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "export"],
    Description = "Exports installed Homebrew software to a JSON file")]
public class BrewExportCommand(IAnsiConsole ansiConsole, IBrewExporter brewExporter)
    : ICliCommand<BrewExportOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewExporter _brewExporter = Ensure.NotNull(brewExporter);

    public async Task<CommandResult> ExecuteAsync(BrewExportOptions options)
    {
        _ansiConsole.Write($"Exporting installed Homebrew software to '{options.OutputPath}' ... ");

        await _brewExporter.ExportToFileAsync(options.OutputPath).ConfigureAwait(false);

        _ansiConsole.MarkupLine("[green]Done[/]");

        return CommandResult.Success;
    }
}
