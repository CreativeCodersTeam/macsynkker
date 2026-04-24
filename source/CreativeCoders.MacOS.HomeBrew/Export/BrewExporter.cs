using System.Text.Json;
using System.Text.Json.Serialization;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Export;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Export;

/// <summary>
/// Default <see cref="IBrewExporter"/> implementation. Reads the installed software via
/// <see cref="IBrewInstalledSoftware"/> and projects it onto the slim
/// <see cref="BrewExportModel"/> used for the JSON export.
/// </summary>
public class BrewExporter(IBrewInstalledSoftware installedSoftware) : IBrewExporter
{
    private const string DefaultFormulaTap = "homebrew/core";

    private const string DefaultCaskTap = "homebrew/cask";

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IBrewInstalledSoftware _installedSoftware = Ensure.NotNull(installedSoftware);

    /// <inheritdoc />
    public async Task<BrewExportModel> CreateExportModelAsync(bool includeDependencies)
    {
        var installed = await _installedSoftware.GetInstalledSoftwareAsync().ConfigureAwait(false);

        return new BrewExportModel
        {
            Formulae = installed.Formulae
                .Where(x => !x.IsInstalledAsDependency() || includeDependencies)
                .Select(MapFormula)
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .ToArray(),
            Casks = installed.Casks
                .Select(MapCask)
                .Where(x => !string.IsNullOrWhiteSpace(x.Token))
                .ToArray()
        };
    }

    /// <inheritdoc />
    public async Task ExportToFileAsync(string filePath, bool includeDependencies)
    {
        Ensure.IsNotNullOrWhitespace(filePath);

        var exportModel = await CreateExportModelAsync(includeDependencies).ConfigureAwait(false);

        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(filePath);

        await JsonSerializer.SerializeAsync(stream, exportModel, JsonOptions).ConfigureAwait(false);
    }

    private static BrewExportFormulaModel MapFormula(BrewFormulaModel formula)
    {
        return new BrewExportFormulaModel
        {
            Name = formula.FullName ?? formula.Name ?? string.Empty,
            Tap = NormalizeTap(formula.Tap, DefaultFormulaTap)
        };
    }

    private static BrewExportCaskModel MapCask(BrewCaskModel cask)
    {
        return new BrewExportCaskModel
        {
            Token = cask.FullToken ?? cask.Token ?? string.Empty,
            Tap = NormalizeTap(cask.Tap, DefaultCaskTap)
        };
    }

    // Returns null when the tap matches the default tap so that the exported JSON stays slim.
    private static string? NormalizeTap(string? tap, string defaultTap)
    {
        if (string.IsNullOrWhiteSpace(tap))
        {
            return null;
        }

        return string.Equals(tap, defaultTap, StringComparison.OrdinalIgnoreCase)
            ? null
            : tap;
    }
}
