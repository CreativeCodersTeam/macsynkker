using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.MacOS.HomeBrew.Models.Export;

/// <summary>
/// Root model that represents the contents of a Homebrew export file. Holds the list of
/// installed formulae and casks that should be re-installed on import.
/// </summary>
[UsedImplicitly]
public class BrewExportModel
{
    /// <summary>Gets or sets the formulae to be exported / installed.</summary>
    [JsonPropertyName("formulae")]
    public BrewExportFormulaModel[] Formulae { get; set; } = [];

    /// <summary>Gets or sets the casks to be exported / installed.</summary>
    [JsonPropertyName("casks")]
    public BrewExportCaskModel[] Casks { get; set; } = [];
}
