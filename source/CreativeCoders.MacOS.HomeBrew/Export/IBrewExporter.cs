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
    Task<BrewExportModel> CreateExportModelAsync();

    /// <summary>
    /// Builds the export model and writes it as JSON to the file at <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">Absolute or relative path of the target file.</param>
    Task ExportToFileAsync(string filePath);
}
