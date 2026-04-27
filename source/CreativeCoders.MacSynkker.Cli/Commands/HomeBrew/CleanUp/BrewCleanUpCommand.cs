using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

/// <summary>Runs <c>brew cleanup</c> via <see cref="IBrewCleanup"/>.</summary>
[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, CleanUpCommandGroup.Name],
    Description = "Run brew cleanup to free disk space")]
public class BrewCleanUpCommand(IBrewCleanup brewCleanup, IAnsiConsole ansiConsole)
    : ICliCommand<BrewCleanUpOptions>
{
    private readonly IBrewCleanup _brewCleanup = Ensure.NotNull(brewCleanup);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(BrewCleanUpOptions options)
    {
        Ensure.NotNull(options);

        var brewOptions = options.ToBrewCleanupOptions();

        _ansiConsole.MarkupLine("Running [bold]brew cleanup[/] ...");

        try
        {
            await _brewCleanup.CleanupAsync(brewOptions).ConfigureAwait(false);

            _ansiConsole.MarkupLine("[green]Cleanup completed successfully[/]");
        }
        catch (BrewCleanupFailedException e)
        {
            _ansiConsole.MarkupLine("[red]Cleanup failed[/]");
            _ansiConsole.WriteLine(e.ErrorOutput);
        }

        return CommandResult.Success;
    }
}
