using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

[UsedImplicitly]
[CliCommand(["brew", "upgrade"])]
public class BrewUpgradeCommand(IBrewUpgrader brewUpgrader) : ICliCommand<BrewUpgradeOptions>
{
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
            await _brewUpgrader.UpgradeAllOutdatedAsync().ConfigureAwait(false);
        }

        return CommandResult.Success;
    }
}

public class BrewUpgradeOptions
{
    [OptionValue(0)] public string AppName { get; set; } = string.Empty;

    [OptionParameter('o', "outdated", HelpText = "Upgrade all outdated software")]
    public bool UpgradeOutdated { get; set; }
}
