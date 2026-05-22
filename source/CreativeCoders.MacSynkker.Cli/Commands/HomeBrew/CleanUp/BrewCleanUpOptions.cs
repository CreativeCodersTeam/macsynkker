using CreativeCoders.Cli.Core;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

/// <summary>Options for the <c>brew cleanup</c> CLI commands.</summary>
[UsedImplicitly]
public class BrewCleanUpOptions : IOptionsValidation
{
    /// <summary>
    /// Maps to <c>brew cleanup --prune=&lt;days&gt;</c>. Mutually exclusive with
    /// <see cref="PruneAll"/>. Must be zero or positive.
    /// </summary>
    [OptionParameter('p', "prune", HelpText = "Remove cache files older than the given number of days")]
    public int? PruneDays { get; set; }

    /// <summary>Maps to <c>brew cleanup --prune=all</c>. Mutually exclusive with <see cref="PruneDays"/>.</summary>
    [OptionParameter('a', "prune-all", HelpText = "Remove all cache files (--prune=all)")]
    public bool PruneAll { get; set; }

    /// <summary>Builds the <see cref="BrewCleanupOptions"/> instance to pass to <c>IBrewCleanup</c>.</summary>
    public BrewCleanupOptions ToBrewCleanupOptions()
    {
        var options = new BrewCleanupOptions();

        if (PruneAll)
        {
            options.Prune = BrewPruneOption.All;
        }
        else if (PruneDays.HasValue)
        {
            options.Prune = BrewPruneOption.Days(PruneDays.Value);
        }

        return options;
    }

    public Task<OptionsValidationResult> ValidateAsync()
    {
        if (PruneAll && PruneDays.HasValue)
        {
            return Task.FromResult(
                OptionsValidationResult.Invalid(["--prune and --prune-all are mutually exclusive"]));
        }

        return Task.FromResult(PruneDays is < 0
            ? OptionsValidationResult.Invalid(["--prune must be zero or a positive number of days"])
            : OptionsValidationResult.Valid());
    }
}
