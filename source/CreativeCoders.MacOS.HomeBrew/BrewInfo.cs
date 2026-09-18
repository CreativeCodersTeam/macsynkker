using System.ComponentModel;
using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew;

public class BrewInfo(IProcessExecutorBuilder<string> executorBuilder) : IBrewInfo
{
    private readonly IProcessExecutor<string> _executor = Ensure.NotNull(executorBuilder)
        .SetFileName("brew")
        .SetArguments(["--version"])
        .ShouldThrowOnError()
        .Build();

    /// <inheritdoc />
    public async Task<bool> IsInstalledAsync()
    {
        return (await TryExecuteAsync().ConfigureAwait(false))?.StartsWith("Homebrew") == true;
    }

    /// <inheritdoc />
    public async Task<string> GetVersionAsync()
    {
        return (await TryExecuteAsync().ConfigureAwait(false))?
            .Split('\n')
            .FirstOrDefault()?
            .Split(' ')
            .Skip(1)
            .FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Runs <c>brew --version</c> and returns <see langword="null"/> instead of throwing when brew
    /// cannot be executed, so a failed probe is reported as "not installed" rather than as an error.
    /// </summary>
    /// <returns>The process output, or <see langword="null"/> if brew could not be executed.</returns>
    private async Task<string?> TryExecuteAsync()
    {
        try
        {
            return await _executor.ExecuteAsync().ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException)
        {
            return null;
        }
        catch (Win32Exception)
        {
            return null;
        }
    }
}
