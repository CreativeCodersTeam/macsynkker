using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

[UsedImplicitly]
public class BrewListInstalledSoftwareOptions
{
    [OptionParameter('c', "casks")] public bool? Casks { get; set; }

    [OptionParameter('f', "formulae")] public bool? Formulae { get; set; }

    [OptionParameter('l', "listview", HelpText = "Show as table list view")]
    public bool ShowAsListView { get; set; }

    [OptionParameter('o', "outdated", HelpText = "Show only outdated software")]
    public bool ShowOnlyOutdated { get; set; }
}
