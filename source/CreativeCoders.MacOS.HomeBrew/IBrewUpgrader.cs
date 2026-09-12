namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewUpgrader
{
    Task UpgradeAsync(bool force = false, bool askForConfirmation = false);

    Task UpgradeSoftwareAsync(string appName, bool force = false, bool askForConfirmation = false);
}
