using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Import;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Import;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "import"],
    Description = "Imports and installs Homebrew software from a JSON file")]
public class BrewImportCommand(IAnsiConsole ansiConsole, IBrewImporter brewImporter)
    : ICliCommand<BrewImportOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewImporter _brewImporter = Ensure.NotNull(brewImporter);

    public async Task<CommandResult> ExecuteAsync(BrewImportOptions options)
    {
        if (!File.Exists(options.InputPath))
        {
            _ansiConsole.MarkupLine($"[red]File not found: {options.InputPath}[/]");

            return MacSynkkerCliExitCodes.FileNotFound;
        }

        _ansiConsole.Write($"Importing Homebrew software from '{options.InputPath}' ... ");

        try
        {
            await _brewImporter.ImportFromFileAsync(options.InputPath).ConfigureAwait(false);

            _ansiConsole.MarkupLine("[green]Done[/]");
        }
        catch (BrewImportFailedException e)
        {
            _ansiConsole.MarkupLine("[yellow]Completed with errors[/]");
            _ansiConsole.WriteLine();

            foreach (var failure in e.Failures)
            {
                _ansiConsole.MarkupLine(
                    $"[red]Failed to install {failure.Kind.ToString().ToLowerInvariant()}: {failure.Target}[/]");
                _ansiConsole.WriteLine(failure.ErrorOutput);
            }
        }

        return CommandResult.Success;
    }
}
