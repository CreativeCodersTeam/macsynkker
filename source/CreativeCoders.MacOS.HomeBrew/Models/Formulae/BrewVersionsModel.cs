using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents version metadata for a formula.
/// </summary>
public class BrewVersionsModel
{
    /// <summary>Gets or sets the stable version string.</summary>
    [JsonPropertyName("stable")] public string? Stable { get; set; }

    /// <summary>Gets or sets the head reference, if any.</summary>
    [JsonPropertyName("head")] public string? Head { get; set; }

    /// <summary>Gets or sets a value indicating whether a bottle is available.</summary>
    [JsonPropertyName("bottle")] public bool? Bottle { get; set; }
}