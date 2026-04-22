using CreativeCoders.MacOS.HomeBrew.Models.Export;

namespace CreativeCoders.MacOS.HomeBrew.Import;

/// <summary>
/// Reads a previously written export file and re-installs the contained Homebrew software via
/// <see cref="IBrewInstaller"/>.
/// </summary>
public interface IBrewImporter
{
    /// <summary>Deserializes the JSON export file at <paramref name="filePath"/>.</summary>
    Task<BrewExportModel> ReadFileAsync(string filePath);

    /// <summary>Installs every formula and cask listed in <paramref name="exportModel"/>.</summary>
    /// <param name="exportModel">The model containing the software to install.</param>
    /// <param name="progress">Optional progress callback that is notified before and after each step.</param>
    Task ImportAsync(BrewExportModel exportModel, IProgress<BrewImportProgress>? progress = null);

    /// <summary>Convenience: <see cref="ReadFileAsync"/> followed by <see cref="ImportAsync"/>.</summary>
    /// <param name="filePath">Path to the JSON export file.</param>
    /// <param name="progress">Optional progress callback that is notified before and after each step.</param>
    Task ImportFromFileAsync(string filePath, IProgress<BrewImportProgress>? progress = null);
}
