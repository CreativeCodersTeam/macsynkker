namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Thrown when <c>brew cleanup</c> exits with a non-zero exit code. Carries the captured error
/// output and the exit code for diagnostics.
/// </summary>
public class BrewCleanupFailedException(string message, string errorOutput, int exitCode) : Exception(message)
{
    /// <summary>Gets the standard error output captured from the <c>brew</c> process.</summary>
    public string ErrorOutput { get; } = errorOutput;

    /// <summary>Gets the exit code reported by the <c>brew</c> process.</summary>
    public int ExitCode { get; } = exitCode;
}
