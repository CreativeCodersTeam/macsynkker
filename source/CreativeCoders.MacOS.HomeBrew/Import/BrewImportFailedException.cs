namespace CreativeCoders.MacOS.HomeBrew.Import;

/// <summary>
/// Aggregates all single-package failures that occurred during a Homebrew import. The import
/// itself runs through to completion; this exception is thrown at the very end when at least
/// one package failed to install.
/// </summary>
public class BrewImportFailedException : Exception
{
    public BrewImportFailedException(IReadOnlyCollection<BrewInstallFailedException> failures)
        : base($"Brew import failed for {failures.Count} package(s)")
    {
        Failures = failures;
    }

    /// <summary>Gets the individual install failures collected during the import.</summary>
    public IReadOnlyCollection<BrewInstallFailedException> Failures { get; }
}
