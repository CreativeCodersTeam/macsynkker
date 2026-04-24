using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
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

        _ansiConsole.MarkupLine($"Importing Homebrew software from '{options.InputPath}'");
        _ansiConsole.WriteLine();

        var progress = new SynchronousProgress<BrewImportProgress>(OnProgress);

        try
        {
            await _brewImporter.ImportFromFileAsync(options.InputPath, progress).ConfigureAwait(false);

            _ansiConsole.WriteLine();
            _ansiConsole.MarkupLine("[green]Import completed successfully[/]");
        }
        catch (BrewImportFailedException e)
        {
            _ansiConsole.WriteLine();
            _ansiConsole.MarkupLine($"[red]Import completed with {e.Failures.Count} error(s)[/]");
        }

        return CommandResult.Success;
    }

    private void OnProgress(BrewImportProgress p)
    {
        switch (p.State)
        {
            case BrewImportStepState.Starting:
                _ansiConsole.Write($"{GetStepLabel(p.Step)} '{p.Target}' ... ");
                break;

            case BrewImportStepState.Succeeded:
                _ansiConsole.MarkupLine("[green]Done[/]");
                break;

            case BrewImportStepState.Failed:
                _ansiConsole.MarkupLine("[red]Failed[/]");

                if (p.Error is not null)
                {
                    _ansiConsole.WriteLine(p.Error.ErrorOutput);
                }

                break;
        }
    }

    private static string GetStepLabel(BrewImportStep step) => step switch
    {
        BrewImportStep.Tap => "Tapping",
        BrewImportStep.InstallFormula => "Installing formula",
        BrewImportStep.InstallCask => "Installing cask",
        _ => "Processing"
    };
}
