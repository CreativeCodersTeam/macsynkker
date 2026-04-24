using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Export;

[UsedImplicitly]

/// <summary>
/// Represents the command-line options for the Homebrew export command.
/// </summary>
public class BrewExportOptions
{
    /// <summary>Gets or sets the file path to write the export JSON to.</summary>
    [OptionParameter('o', "output", HelpText = "The file path to export the installed software to",
        IsRequired = true)]
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether formulae installed as dependencies are included in the export.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include dependencies; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    [OptionParameter('d', "dependency", HelpText = "Include dependencies in the export")]
    public bool IncludeDependencies { get; set; }
}
