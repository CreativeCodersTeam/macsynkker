using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a specific bottle build for a channel (e.g., stable).
/// </summary>
public class BrewBottleStableModel
{
    /// <summary>Gets or sets the rebuild number of the bottle.</summary>
    [JsonPropertyName("rebuild")] public int? Rebuild { get; set; }

    /// <summary>Gets or sets the root URL of bottle registry.</summary>
    [JsonPropertyName("root_url")] public string? RootUrl { get; set; }

    /// <summary>Gets or sets bottle files keyed by platform (e.g., "all", "arm64_ventura").</summary>
    [JsonPropertyName("files")] public Dictionary<string, BrewBottleFileEntryModel>? Files { get; set; }
}