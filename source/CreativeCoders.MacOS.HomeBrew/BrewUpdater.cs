using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew;

public class BrewUpdater(IProcessExecutorBuilder<string> processExecutorBuilder) : IBrewUpdater
{
    private readonly IProcessExecutor<string> _updateBrewExecutor = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("brew")
        .SetArguments(["update", "{{force}}"])
        .ShouldThrowOnError()
        .Build();

    public async Task UpdateAsync(bool force = false)
    {
        try
        {
            await _updateBrewExecutor.ExecuteAsync(new { force = force ? "-f" : "" })
                .ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewUpdateException("Brew update failed", e.ErrorOutput, e.ExitCode);
        }
    }
}
