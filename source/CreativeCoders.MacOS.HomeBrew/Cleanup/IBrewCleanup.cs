namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>Runs <c>brew cleanup</c> and inspects the disk space it would free.</summary>
public interface IBrewCleanup
{
    /// <summary>
    /// Executes <c>brew cleanup</c> using the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Cleanup options or <c>null</c> for Homebrew defaults.</param>
    /// <exception cref="BrewCleanupFailedException">When the <c>brew</c> process fails.</exception>
    Task CleanupAsync(BrewCleanupOptions? options = null);

    /// <summary>
    /// Executes <c>brew cleanup --dry-run</c> using the given <paramref name="options"/> and
    /// returns the amount of disk space (in bytes) that the cleanup would reclaim. Returns
    /// <c>0</c> when the brew output does not contain a parseable size hint.
    /// </summary>
    /// <param name="options">Cleanup options or <c>null</c> for Homebrew defaults.</param>
    /// <exception cref="BrewCleanupFailedException">When the <c>brew</c> process fails.</exception>
    Task<long> GetReclaimableSpaceAsync(BrewCleanupOptions? options = null);

    /// <summary>
    /// Executes <c>brew cleanup --dry-run</c> using the given <paramref name="options"/> and
    /// returns both the total amount of disk space (in bytes) that the cleanup would reclaim
    /// and a detailed list of individual entries that would be removed together with their
    /// reported size.
    /// </summary>
    /// <param name="options">Cleanup options or <c>null</c> for Homebrew defaults.</param>
    /// <returns>
    /// A <see cref="ReclaimableSpace"/> containing the total reclaimable bytes and the list
    /// of <see cref="ReclaimableItem"/> entries parsed from the brew output. The result is
    /// empty when the output does not contain any parseable size information.
    /// </returns>
    /// <exception cref="BrewCleanupFailedException">When the <c>brew</c> process fails.</exception>
    Task<ReclaimableSpace> GetReclaimableSpaceDetailsAsync(BrewCleanupOptions? options = null);
}
