namespace CreativeCoders.MacOS.HomeBrew;

/// <summary>
/// Wraps the <c>brew</c> CLI calls needed to add taps and install formulae or casks.
/// </summary>
public interface IBrewInstaller
{
    /// <summary>Adds a Homebrew tap (<c>brew tap &lt;tap&gt;</c>).</summary>
    Task TapAsync(string tap);

    /// <summary>Installs a formula (<c>brew install &lt;name&gt;</c>).</summary>
    Task InstallFormulaAsync(string name);

    /// <summary>Installs a cask (<c>brew install --cask &lt;token&gt;</c>).</summary>
    Task InstallCaskAsync(string token);
}
