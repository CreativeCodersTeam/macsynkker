namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Represents the value passed to the <c>--prune</c> option of <c>brew cleanup</c>. A prune option
/// either targets cache files older than a specified number of days or removes all cache files.
/// </summary>
public sealed class BrewPruneOption
{
    private readonly int? _days;

    private readonly bool _all;

    private BrewPruneOption(int? days, bool all)
    {
        _days = days;
        _all = all;
    }

    /// <summary>
    /// Creates a prune option that removes cache files older than <paramref name="days"/> days.
    /// </summary>
    /// <param name="days">Age threshold in days. Must be zero or positive.</param>
    public static BrewPruneOption Days(int days)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days), days, "Days must be zero or positive.");
        }

        return new BrewPruneOption(days, all: false);
    }

    /// <summary>Creates a prune option that removes all cache files.</summary>
    public static BrewPruneOption All { get; } = new BrewPruneOption(days: null, all: true);

    /// <summary>Returns the command-line argument representation, e.g. <c>--prune=7</c> or <c>--prune=all</c>.</summary>
    public string ToCommandLineArgument()
    {
        return _all ? "--prune=all" : $"--prune={_days}";
    }
}
