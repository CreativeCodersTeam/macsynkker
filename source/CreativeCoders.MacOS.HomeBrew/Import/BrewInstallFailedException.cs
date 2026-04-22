namespace CreativeCoders.MacOS.HomeBrew.Import;

/// <summary>
/// Identifies which kind of <c>brew</c> operation failed.
/// </summary>
public enum BrewInstallTargetKind
{
    Tap,
    Formula,
    Cask
}

/// <summary>
/// Thrown when a single <c>brew tap</c> or <c>brew install</c> call performed by the
/// <see cref="IBrewInstaller"/> fails.
/// </summary>
public class BrewInstallFailedException(
    BrewInstallTargetKind kind,
    string target,
    string errorOutput,
    int exitCode)
    : Exception($"Brew {kind.ToString().ToLowerInvariant()} of '{target}' failed")
{
    /// <summary>Gets the kind of operation that failed.</summary>
    public BrewInstallTargetKind Kind { get; } = kind;

    /// <summary>Gets the tap, formula name or cask token of the failed operation.</summary>
    public string Target { get; } = target;

    /// <summary>Gets the standard-error output of the failed <c>brew</c> call.</summary>
    public string ErrorOutput { get; } = errorOutput;

    /// <summary>Gets the exit code reported by the failed <c>brew</c> call.</summary>
    public int ExitCode { get; } = exitCode;
}
