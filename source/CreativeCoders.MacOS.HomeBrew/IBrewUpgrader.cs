using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewUpgrader
{
    Task UpgradeAsync();

    Task UpgradeAllOutdatedAsync();

    Task UpgradeSoftwareAsync(params IEnumerable<string> appNames);
}

public class BrewUpgrader(
    IProcessExecutorBuilder<string> processExecutorBuilder,
    IBrewInstalledSoftware brewInstalledSoftware) : IBrewUpgrader
{
    private readonly IBrewInstalledSoftware _brewInstalledSoftware = Ensure.NotNull(brewInstalledSoftware);

    private readonly IProcessExecutor<string> _upgradeBrewExecutor = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("brew")
        .SetArguments(["upgrade", "{{appName}}"])
        .ShouldThrowOnError()
        .Build();

    public async Task UpgradeAsync()
    {
        var result = await _upgradeBrewExecutor.ExecuteExAsync(new { appName = "" }).ConfigureAwait(false);

        Console.WriteLine(result.Value);
    }

    public async Task UpgradeAllOutdatedAsync()
    {
        var installedSoftware = await _brewInstalledSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        var outdatedCaskNames = installedSoftware.GetOutdatedCasks()
            .Select(x => x.FullToken)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OfType<string>();

        await UpgradeSoftwareAsync(outdatedCaskNames).ConfigureAwait(false);

        var outdatedFormulaeNames = installedSoftware.GetOutdatedFormulae()
            .Select(x => x.FullName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OfType<string>();

        await UpgradeSoftwareAsync(outdatedFormulaeNames).ConfigureAwait(false);
    }

    public async Task UpgradeSoftwareAsync(params IEnumerable<string> appNames)
    {
        foreach (var appName in appNames)
        {
            var result = await _upgradeBrewExecutor.ExecuteExAsync(new { appName }).ConfigureAwait(false);

            Console.WriteLine(result.Value);
        }
    }
}
