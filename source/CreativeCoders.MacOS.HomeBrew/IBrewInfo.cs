namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewInfo
{
    /// <summary>
    /// Determines whether Homebrew is installed and executable.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if Homebrew could be executed and reported itself;
    /// otherwise, <see langword="false"/>. A failing or missing <c>brew</c> executable is
    /// reported as <see langword="false"/> and does not throw.
    /// </returns>
    Task<bool> IsInstalledAsync();

    /// <summary>
    /// Gets the installed Homebrew version.
    /// </summary>
    /// <returns>
    /// The version reported by Homebrew, or an empty string if it could not be determined.
    /// A failing or missing <c>brew</c> executable yields an empty string and does not throw.
    /// </returns>
    Task<string> GetVersionAsync();
}
