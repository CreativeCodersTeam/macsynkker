using CreativeCoders.MacOS.HomeBrew.Models.Export;

namespace CreativeCoders.MacOS.HomeBrew.Export;

/// <summary>
/// Exports the locally installed Homebrew software to a serializable model or to a JSON file.
/// </summary>
public interface IBrewExporter
{
    /// <summary>
    /// Builds a <see cref="BrewExportModel"/> based on the currently installed Homebrew software.
    /// </summary>
    /// <param name="includeDependencies">
    /// <see langword="true"/> to include formulae installed as dependencies;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>A <see cref="BrewExportModel"/> representing the installed software.</returns>
    Task<BrewExportModel> CreateExportModelAsync(bool includeDependencies);

    /// <summary>
    /// Builds the export model and writes it as JSON to the file at <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The absolute or relative path of the target file.</param>
    /// <param name="includeDependencies">
    /// <see langword="true"/> to include formulae installed as dependencies;
    /// otherwise, <see langword="false"/>.
    /// </param>
    Task ExportToFileAsync(string filePath, bool includeDependencies);
}
