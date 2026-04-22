using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.MacOS.HomeBrew.Models.Export;

/// <summary>
/// Represents a single Homebrew formula entry inside an export file.
/// </summary>
[UsedImplicitly]
public class BrewExportFormulaModel
{
    /// <summary>Gets or sets the formula name (preferably the full name including the tap).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tap the formula belongs to. <c>null</c> when the formula resides in the
    /// default core tap (<c>homebrew/core</c>).
    /// </summary>
    [JsonPropertyName("tap")]
    public string? Tap { get; set; }
}
