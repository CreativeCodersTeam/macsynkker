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
    Task ImportAsync(BrewExportModel exportModel);

    /// <summary>Convenience: <see cref="ReadFileAsync"/> followed by <see cref="ImportAsync"/>.</summary>
    Task ImportFromFileAsync(string filePath);
}
