namespace CreativeCoders.MacOS.HomeBrew;

public class BrewUpgradeFailedException(string appName, string errorOutput, int exitCode)
    : BrewUpgradeException($"Upgrade of '{appName}' failed", errorOutput, exitCode)
{
    public string AppName { get; } = appName;
}