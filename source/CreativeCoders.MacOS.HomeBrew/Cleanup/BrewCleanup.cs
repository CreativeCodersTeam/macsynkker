using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Default <see cref="IBrewCleanup"/> implementation. Uses <see cref="IProcessExecutor{TResult}"/>
/// to invoke <c>brew cleanup</c>, mirroring the structure of <c>BrewUpgrader</c> and
/// <c>BrewInstaller</c>.
/// </summary>
public class BrewCleanup : IBrewCleanup
{
    private readonly IProcessExecutor<string> _cleanupExecutor;

    public BrewCleanup(IProcessExecutorBuilder<string> processExecutorBuilder)
    {
        Ensure.NotNull(processExecutorBuilder);

        _cleanupExecutor = processExecutorBuilder
            .SetFileName("brew")
            .SetArguments(["cleanup", "{{prune}}", "{{dryRun}}"])
            .ShouldThrowOnError()
            .Build();
    }

    public async Task CleanupAsync(BrewCleanupOptions? options = null)
    {
        await ExecuteAsync(options, dryRun: false).ConfigureAwait(false);
    }

    public async Task<long> GetReclaimableSpaceAsync(BrewCleanupOptions? options = null)
    {
        var details = await GetReclaimableSpaceDetailsAsync(options).ConfigureAwait(false);

        return details.TotalBytes;
    }

    public async Task<ReclaimableSpace> GetReclaimableSpaceDetailsAsync(BrewCleanupOptions? options = null)
    {
        var output = await ExecuteAsync(options, dryRun: true).ConfigureAwait(false);

        return ReclaimableSpaceParser.Parse(output);
    }

    private async Task<string?> ExecuteAsync(BrewCleanupOptions? options, bool dryRun)
    {
        var prune = options?.Prune?.ToCommandLineArgument() ?? string.Empty;

        try
        {
            return await _cleanupExecutor
                .ExecuteAsync(new { prune, dryRun = dryRun ? "--dry-run" : string.Empty })
                .ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewCleanupFailedException("Brew cleanup failed", e.ErrorOutput, e.ExitCode, e);
        }
    }
}
