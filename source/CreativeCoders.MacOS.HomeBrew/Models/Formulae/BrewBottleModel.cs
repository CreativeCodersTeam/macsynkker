using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents bottle information for a formula.
/// </summary>
public class BrewBottleModel
{
    /// <summary>Gets or sets bottle information for the stable channel.</summary>
    [JsonPropertyName("stable")] public BrewBottleStableModel? Stable { get; set; }
}