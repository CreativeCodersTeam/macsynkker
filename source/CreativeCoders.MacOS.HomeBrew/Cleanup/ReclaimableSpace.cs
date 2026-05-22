namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Aggregated result of a <c>brew cleanup --dry-run</c> invocation: the total amount
/// of disk space that would be reclaimed and the individual entries that contribute
/// to that total.
/// </summary>
/// <param name="TotalBytes">Total reclaimable disk space in bytes.</param>
/// <param name="Items">Detailed list of entries that would be removed.</param>
public record ReclaimableSpace(long TotalBytes, IReadOnlyList<ReclaimableItem> Items);
