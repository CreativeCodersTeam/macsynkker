using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

/// <summary>Options for the <c>brew cleanup-show</c> CLI command.</summary>
[UsedImplicitly]
public class BrewReclaimableSpaceOptions : BrewCleanUpOptions
{
    /// <summary>
    /// When set, prints the individual entries that would be removed by
    /// <c>brew cleanup --dry-run</c> together with their reported size in addition
    /// to the total reclaimable disk space.
    /// </summary>
    [OptionParameter('d', "details", HelpText = "Show individual entries with their size in addition to the total")]
    public bool Details { get; set; }
}
