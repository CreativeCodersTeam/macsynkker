using CreativeCoders.Cli.Core;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Upgrade;

[UsedImplicitly]
public class BrewUpgradeOptions : IOptionsValidation
{
    [OptionValue(0, HelpText = "App name to upgrade. If specified, upgrade outdated option is not allowed.")]
    public string AppName { get; set; } = string.Empty;

    [OptionParameter('o', "outdated",
        HelpText = "Upgrade all outdated software. If specified, app name option is not allowed.")]
    public bool UpgradeOutdated { get; set; }

    [OptionParameter('f', "force", HelpText = "Force upgrade of software")]
    public bool Force { get; set; }

    [OptionParameter('h', "haltonerror", HelpText = "Halt on error upgrading an app")]
    public bool HaltOnError { get; set; }

    public Task<OptionsValidationResult> ValidateAsync()
    {
        if (string.IsNullOrWhiteSpace(AppName) && !UpgradeOutdated)
        {
            return Task.FromResult(
                OptionsValidationResult.Invalid(["Either app name or upgrade outdated option must be specified"]));
        }

        if (!string.IsNullOrWhiteSpace(AppName) && UpgradeOutdated)
        {
            return Task.FromResult(
                OptionsValidationResult.Invalid(["App name and upgrade outdated option are mutually exclusive"]));
        }

        return Task.FromResult(OptionsValidationResult.Valid());
    }
}
