using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.List;

/// <summary>
/// Lists the installed Homebrew formulae and casks on the console.
/// </summary>
[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, "list"], Description = "Shows Homebrew installed software")]
public class BrewListInstalledSoftwareCommand(IAnsiConsole ansiConsole, IBrewInstalledSoftware brewInstalledSoftware)
    : ICliCommand<BrewListInstalledSoftwareOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    /// <summary>
    /// Retrieves and displays the installed Homebrew software based on the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The listing options controlling output format and filters.</param>
    /// <returns>A <see cref="CommandResult"/> indicating success or failure.</returns>
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

            _ansiConsole.WriteLine();
        }

        if ((!options.Casks.HasValue && !options.Formulae.HasValue) || options.Formulae == true)
        {
            PrintFormulae(installedSoftware.GetFormulae(options.ShowOnlyOutdated), options.ShowAsListView);
        }

        return CommandResult.Success;
    }

    /// <summary>
    /// Prints the installed formulae to the console.
    /// </summary>
    /// <param name="installedSoftwareFormulae">The formulae to display.</param>
    /// <param name="optionsShowAsListView">
    /// <see langword="true"/> to render a table; otherwise, <see langword="false"/> for a simple list.
    /// </param>
    private void PrintFormulae(BrewFormulaModel[] installedSoftwareFormulae, bool optionsShowAsListView)
    {
        _ansiConsole.WriteLines("Installed HomeBrew formulae:", string.Empty);

        if (optionsShowAsListView)
        {
            _ansiConsole.PrintTable(installedSoftwareFormulae, [
                new TableColumnDef<BrewFormulaModel>(x => x.FullName, "FullName"),
                new TableColumnDef<BrewFormulaModel>(x =>
                    string.Join(",", x.Installed?.Select(y => y.Version) ?? []), "Installed"),
                new TableColumnDef<BrewFormulaModel>(x => x.Versions?.Stable, "Available"),
                new TableColumnDef<BrewFormulaModel>(
                    x => x.IsInstalledAsDependency(),
                    "Installed as dependency")
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
    }

    /// <summary>
    /// Prints the installed casks to the console.
    /// </summary>
    /// <param name="installedSoftwareCasks">The casks to display.</param>
    /// <param name="optionsShowAsListView">
    /// <see langword="true"/> to render a table; otherwise, <see langword="false"/> for a simple list.
    /// </param>
    private void PrintCasks(BrewCaskModel[] installedSoftwareCasks, bool optionsShowAsListView)
    {
        _ansiConsole.WriteLines("Installed HomeBrew casks:", string.Empty);

        if (optionsShowAsListView)
        {
            _ansiConsole.PrintTable(installedSoftwareCasks, [
                new TableColumnDef<BrewCaskModel>(x => string.Join(string.Empty, x.Name ?? []), "Name",
                    color: Color.Blue),
                new TableColumnDef<BrewCaskModel>(x => x.Installed, "Installed"),
                new TableColumnDef<BrewCaskModel>(x => x.Version, "Available"),
                new TableColumnDef<BrewCaskModel>(x => x.Tap, "Tap"),
                new TableColumnDef<BrewCaskModel>(x => x.FullToken, "Fulltoken"),
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
    }

    /// <summary>
    /// Extracts the primary version number from a cask version string that may contain multiple comma-separated parts.
    /// </summary>
    /// <param name="versionString">The raw version string from the cask model.</param>
    /// <returns>The extracted version, or an empty string if <paramref name="versionString"/> is empty.</returns>
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
