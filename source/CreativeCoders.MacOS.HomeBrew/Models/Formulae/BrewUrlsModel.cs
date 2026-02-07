using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents the URLs used by a formula.
/// </summary>
public class BrewUrlsModel
{
    /// <summary>Gets or sets the stable URL block.</summary>
    [JsonPropertyName("stable")] public BrewStableUrlModel? Stable { get; set; }
}