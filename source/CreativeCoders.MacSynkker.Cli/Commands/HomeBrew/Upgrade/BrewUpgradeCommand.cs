using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Upgrade;

[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "upgrade"], Description = "Upgrade Homebrew installed software")]
public class BrewUpgradeCommand(
    IBrewUpdater brewUpdater,
    IBrewUpgrader brewUpgrader,
    IBrewInstalledSoftware brewInstalledSoftware,
    IAnsiConsole ansiConsole)
    : ICliCommand<BrewUpgradeOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    private readonly IBrewUpdater _brewUpdater = Ensure.NotNull(brewUpdater);

    private readonly IBrewUpgrader _brewUpgrader = Ensure.NotNull(brewUpgrader);

    public async Task<CommandResult> ExecuteAsync(BrewUpgradeOptions options)
    {
        _ansiConsole.Write("Updating Homebrew ... ");

        await _brewUpdater.UpdateAsync().ConfigureAwait(false);

        _ansiConsole.MarkupLine("[green]Done[/]");

        if (!string.IsNullOrWhiteSpace(options.AppName))
        {
            await UpgradeSoftwareCoreAsync(options.AppName).ConfigureAwait(false);
        }
        else if (options.UpgradeOutdated)
        {
            await UpgradeAllOutdatedAsync(options).ConfigureAwait(false);
        }

        return CommandResult.Success;
    }

    private Task UpgradeSoftwareCoreAsync(string appName)
    {
        if (!appName.StartsWith("dotnet-", StringComparison.OrdinalIgnoreCase))
        {
            return _brewUpgrader.UpgradeSoftwareAsync(appName);
        }

        _ansiConsole.MarkupLine(
            "Skipping upgrade of a dotnet part cause its possible, that its needed by this app"
                .ToWarningMarkup());

        return Task.CompletedTask;
    }

    private async Task UpgradeAllOutdatedAsync(BrewUpgradeOptions options)
    {
        var installedSoftware = await _brewInstalledSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        var outdatedCasks = installedSoftware.GetOutdatedCasks().ToArray();
        var outdatedFormulae = installedSoftware.GetOutdatedFormulae().ToArray();

        if (!outdatedCasks.Any() && !outdatedFormulae.Any())
        {
            _ansiConsole.MarkupLine("No outdated software found".ToInfoMarkup());
            return;
        }

        var outdatedCaskNames = outdatedCasks
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

        var outdatedFormulaeNames = outdatedFormulae
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
            await UpgradeSoftwareCoreAsync(appName).ConfigureAwait(false);

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
