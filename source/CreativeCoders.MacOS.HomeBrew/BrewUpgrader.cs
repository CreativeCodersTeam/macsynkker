using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew;

public class BrewUpgrader(
    IProcessExecutorBuilder<string> processExecutorBuilder) : IBrewUpgrader
{
    private readonly IProcessExecutor<string> _upgradeBrewExecutor = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("brew")
        .SetArguments(["upgrade", "{{appName}}", "{{force}}", "{{askForConfirmation}}"])
        .ShouldThrowOnError()
        .Build();

    public async Task UpgradeAsync(bool force = false, bool askForConfirmation = false)
    {
        try
        {
            await _upgradeBrewExecutor.ExecuteAsync(new
                    { appName = "", force = force ? "-f" : "", askForConfirmation = !askForConfirmation ? "-y" : "" })
                .ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewUpgradeException("Brew upgrade failed", e.ErrorOutput, e.ExitCode);
        }
    }

    public async Task UpgradeSoftwareAsync(string appName, bool force = false, bool askForConfirmation = false)
    {
        Ensure.IsNotNullOrWhitespace(appName);

        try
        {
            await _upgradeBrewExecutor.ExecuteAsync(new
                    { appName, force = force ? "-f" : "", askForConfirmation = !askForConfirmation ? "-y" : "" })
                .ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewUpgradeFailedException(appName, e.ErrorOutput, e.ExitCode);
        }
    }
}
