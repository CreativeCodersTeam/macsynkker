using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

[UsedImplicitly]
[CliCommand(["brew", "list"])]
public class BrewListInstalledSoftwareCommand(IAnsiConsole ansiConsole, IBrewInstalledSoftware brewInstalledSoftware)
    : ICliCommand<BrewListInstalledSoftwareOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    public async Task<CommandResult> ExecuteAsync(BrewListInstalledSoftwareOptions options)
    {
        _ansiConsole.WriteLine("List installed HomeBrew software");

        if (options.ShowOnlyOutdated)
        {
            _ansiConsole.WriteLine("Only outdated software will be shown");
        }

        var installedSoftware = await _brewInstalledSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        if ((!options.Casks.HasValue && !options.Formulae.HasValue) || options.Casks == true)
        {
            PrintCasks(installedSoftware.GetCasks(options.ShowOnlyOutdated), options.ShowAsListView);
        }

        if ((!options.Casks.HasValue && !options.Formulae.HasValue) || options.Formulae == true)
        {
            PrintFormulae(installedSoftware.GetFormulae(options.ShowOnlyOutdated), options.ShowAsListView);
        }

        return CommandResult.Success;
    }

    private void PrintFormulae(BrewFormulaModel[] installedSoftwareFormulae, bool optionsShowAsListView)
    {
        _ansiConsole.WriteLine("Installed HomeBrew formulae:");
        _ansiConsole.WriteLine();

        if (optionsShowAsListView)
        {
            _ansiConsole.PrintTable(installedSoftwareFormulae, [
                new TableColumnDef<BrewFormulaModel>(x => x.Name, "Name", color: Color.Blue),
                new TableColumnDef<BrewFormulaModel>(x =>
                    string.Join(",", x.Installed?.Select(y => y.Version) ?? []), "Installed"),
                new TableColumnDef<BrewFormulaModel>(x => x.Versions?.Stable, "Available")
            ]);
        }
        else
        {
            foreach (var installedSoftwareFormula in installedSoftwareFormulae)
            {
                _ansiConsole.WriteLine(
                    $"- {installedSoftwareFormula.Name} ({installedSoftwareFormula.Versions?.Stable})");
            }
        }

        _ansiConsole.WriteLine();
    }

    private void PrintCasks(BrewCaskModel[] installedSoftwareCasks, bool optionsShowAsListView)
    {
        _ansiConsole.WriteLine("Installed HomeBrew casks:");
        _ansiConsole.WriteLine();

        if (optionsShowAsListView)
        {
            _ansiConsole.PrintTable(installedSoftwareCasks, [
                new TableColumnDef<BrewCaskModel>(x => string.Join(string.Empty, x.Name ?? []), "Name",
                    color: Color.Blue),
                new TableColumnDef<BrewCaskModel>(x => x.Installed, "Installed"),
                new TableColumnDef<BrewCaskModel>(x => x.Version, "Available"),
                new TableColumnDef<BrewCaskModel>(x => x.Tap, "Tap")
            ]);
        }
        else
        {
            foreach (var installedSoftwareCask in installedSoftwareCasks)
            {
                _ansiConsole.WriteLine(
                    $"- {installedSoftwareCask.Name?.FirstOrDefault() ?? "unknown"} ({ExtractCaskVersion(installedSoftwareCask.Installed)}) [{installedSoftwareCask.Installed}]");
            }
        }

        _ansiConsole.WriteLine();
    }

    private static string ExtractCaskVersion(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
        {
            return string.Empty;
        }

        var versionSplitterIndex = versionString.IndexOf(',');

        if (versionSplitterIndex == -1)
        {
            return versionString;
        }

        var firstVersion = versionString[..versionSplitterIndex];

        var secondVersion = versionString[(versionSplitterIndex + 1)..];

        return secondVersion.StartsWith(firstVersion, StringComparison.InvariantCultureIgnoreCase)
            ? secondVersion
            : firstVersion;
    }
}
