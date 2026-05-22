namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>Options for a <c>brew cleanup</c> invocation.</summary>
public class BrewCleanupOptions
{
    /// <summary>
    /// Optional <c>--prune</c> setting. When <c>null</c> the option is omitted and Homebrew's
    /// default behaviour applies.
    /// </summary>
    public BrewPruneOption? Prune { get; set; }
}
