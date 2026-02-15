using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Upgrade;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "upgrade"])]
public class BrewUpgradeCommand(
    IBrewUpgrader brewUpgrader,
    IBrewInstalledSoftware brewInstalledSoftware,
    IAnsiConsole ansiConsole)
    : ICliCommand<BrewUpgradeOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    private readonly IBrewUpgrader _brewUpgrader = Ensure.NotNull(brewUpgrader);

    public async Task<CommandResult> ExecuteAsync(BrewUpgradeOptions options)
    {
        await _brewUpgrader.UpgradeAsync().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(options.AppName))
        {
            await _brewUpgrader.UpgradeSoftwareAsync(options.AppName).ConfigureAwait(false);
        }
        else if (options.UpgradeOutdated)
        {
            await UpgradeAllOutdatedAsync(options).ConfigureAwait(false);
        }

        return CommandResult.Success;
    }

    private async Task UpgradeAllOutdatedAsync(BrewUpgradeOptions options)
    {
        var installedSoftware = await _brewInstalledSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        var outdatedCaskNames = installedSoftware.GetOutdatedCasks()
            .Select(x => x.FullToken)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OfType<string>();

        foreach (var outdatedCaskName in outdatedCaskNames)
        {
            var success = await UpgradeSoftwareAsync(outdatedCaskName, true).ConfigureAwait(false);

            if (!success && options.HaltOnError)
            {
                return;
            }
        }

        var outdatedFormulaeNames = installedSoftware.GetOutdatedFormulae()
            .Select(x => x.FullName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OfType<string>();

        foreach (var outdatedFormulaeName in outdatedFormulaeNames)
        {
            var success = await UpgradeSoftwareAsync(outdatedFormulaeName, false).ConfigureAwait(false);

            if (!success && options.HaltOnError)
            {
                return;
            }
        }
    }

    private async Task<bool> UpgradeSoftwareAsync(string appName, bool cask)
    {
        _ansiConsole.Write($"Upgrading outdated {GetSoftwareKind(cask)} '{appName}' ... ");

        try
        {
            await _brewUpgrader.UpgradeSoftwareAsync(appName).ConfigureAwait(false);

            _ansiConsole.MarkupLine("[green]Done[/]");

            return true;
        }
        catch (BrewUpgradeFailedException e)
        {
            _ansiConsole.MarkupLine("[red]Failed[/]");

            _ansiConsole.WriteLine(e.ErrorOutput);

            return false;
        }
    }

    private static string GetSoftwareKind(bool cask) => cask ? "cask" : "formula";
}
