using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Info;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "info"], Description = "Shows Homebrew info")]
public class BrewInfoCommand(IAnsiConsole ansiConsole, IBrewInfo brewInfo) : ICliCommand<InfoOptions>
{
    private readonly IBrewInfo _brewInfo = Ensure.NotNull(brewInfo);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(InfoOptions options)
    {
        _ansiConsole.WriteLine("HomeBrew Info");

        var isInstalled = await _brewInfo.IsInstalledAsync().ConfigureAwait(false);

        _ansiConsole.MarkupLine($"Installed: {isInstalled}");

        if (!isInstalled)
        {
            return CommandResult.Success;
        }

        var version = await _brewInfo.GetVersionAsync().ConfigureAwait(false);

        _ansiConsole.MarkupLine($"Version: {version}");

        return CommandResult.Success;
    }
}

[UsedImplicitly]
public class InfoOptions
{
}
