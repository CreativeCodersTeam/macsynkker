using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Export;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Export;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "export"],
    Description = "Exports installed Homebrew software to a JSON file")]

/// <summary>
/// Exports the installed Homebrew software to a JSON file.
/// </summary>
public class BrewExportCommand(IAnsiConsole ansiConsole, IBrewExporter brewExporter)
    : ICliCommand<BrewExportOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewExporter _brewExporter = Ensure.NotNull(brewExporter);

    /// <summary>
    /// Exports the installed Homebrew formulae and casks to the file path specified in <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The export options containing the output path and dependency filter.</param>
    /// <returns>A <see cref="CommandResult"/> indicating success or failure.</returns>
    public async Task<CommandResult> ExecuteAsync(BrewExportOptions options)
    {
        _ansiConsole.Write($"Exporting installed Homebrew software to '{options.OutputPath}' ... ");

        await _brewExporter.ExportToFileAsync(options.OutputPath, options.IncludeDependencies).ConfigureAwait(false);

        _ansiConsole.MarkupLine("[green]Done[/]");

        return CommandResult.Success;
    }
}
