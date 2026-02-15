using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Upgrade;

[UsedImplicitly]
public class BrewUpgradeOptions
{
    [OptionValue(0)] public string AppName { get; set; } = string.Empty;

    [OptionParameter('o', "outdated", HelpText = "Upgrade all outdated software")]
    public bool UpgradeOutdated { get; set; }

    [OptionParameter('f', "force", HelpText = "Force upgrade of software")]
    public bool Force { get; set; }

    [OptionParameter('h', "haltonerror", HelpText = "Halt on error upgrading an app")]
    public bool HaltOnError { get; set; }
}
