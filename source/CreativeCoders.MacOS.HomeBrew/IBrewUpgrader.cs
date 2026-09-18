namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewUpgrader
{
    Task UpgradeAsync(bool force = false, bool askForConfirmation = false);

    /// <summary>
    /// Upgrades a single installed package.
    /// </summary>
    /// <param name="appName">The name of the package to upgrade.</param>
    /// <param name="force">A value indicating whether the upgrade is forced.</param>
    /// <param name="askForConfirmation">
    /// A value indicating whether Homebrew asks for confirmation. When <see langword="false"/>
    /// the upgrade runs unattended.
    /// </param>
    /// <returns>A task that represents the asynchronous upgrade operation.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="appName"/> is empty or consists only of whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="appName"/> is <see langword="null"/>.</exception>
    Task UpgradeSoftwareAsync(string appName, bool force = false, bool askForConfirmation = false);
}
