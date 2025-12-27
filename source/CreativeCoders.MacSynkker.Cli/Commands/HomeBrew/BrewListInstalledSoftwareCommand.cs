using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.SysConsole.Cli.Parsing;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

[CliCommand(["brew", "list"])]
public class BrewListInstalledSoftwareCommand(IAnsiConsole ansiConsole, IBrewInstalledSoftware brewInstalledSoftware)
    : ICliCommand<BrewListInstalledSoftwareOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    public async Task<CommandResult> ExecuteAsync(BrewListInstalledSoftwareOptions options)
    {
        var installedSoftware = await _brewInstalledSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        _ansiConsole.WriteLine("Installed HomeBrew casks:");

        foreach (var installedSoftwareCask in installedSoftware.Casks)
        {
            _ansiConsole.WriteLine(
                $"- {installedSoftwareCask.Name?.FirstOrDefault() ?? "unkown"} ({ExtractCaskVersion(installedSoftwareCask.Version)}) [{installedSoftwareCask.Version}]");
        }

        _ansiConsole.WriteLine();
        _ansiConsole.WriteLine("Installed HomeBrew formulae:");

        foreach (var installedSoftwareFormula in installedSoftware.Formulae)
        {
            _ansiConsole.WriteLine($"- {installedSoftwareFormula.Name} ({installedSoftwareFormula.Versions?.Stable})");
        }

        return CommandResult.Success;
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

public class BrewListInstalledSoftwareOptions
{
    [OptionParameter('c', "casks")] public bool? Casks { get; set; }

    [OptionParameter('f', "formulae")] public bool? Formulae { get; set; }
}
