using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a single bottle file entry.
/// </summary>
public class BrewBottleFileEntryModel
{
    /// <summary>Gets or sets the cellar type, e.g., <c>:any_skip_relocation</c>.</summary>
    [JsonPropertyName("cellar")] public string? Cellar { get; set; }

    /// <summary>Gets or sets the URL to the bottle layer/blob.</summary>
    [JsonPropertyName("url")] public string? Url { get; set; }

    /// <summary>Gets or sets the SHA-256 of the bottle.</summary>
    [JsonPropertyName("sha256")] public string? Sha256 { get; set; }
}