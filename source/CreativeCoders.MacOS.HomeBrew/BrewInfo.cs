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

    public async Task<bool> IsInstalledAsync()
    {
        return (await _executor.ExecuteAsync().ConfigureAwait(false))?.StartsWith("Homebrew") == true;
    }

    public async Task<string> GetVersionAsync()
    {
        return (await _executor.ExecuteAsync().ConfigureAwait(false))?
            .Split(' ')
            .Skip(1)
            .FirstOrDefault() ?? string.Empty;
    }
}
