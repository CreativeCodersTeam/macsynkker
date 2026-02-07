using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents the stable URL details for fetching the formula tarball.
/// </summary>
public class BrewStableUrlModel
{
    /// <summary>Gets or sets the URL.</summary>
    [JsonPropertyName("url")] public string? Url { get; set; }

    /// <summary>Gets or sets the VCS tag, if applicable.</summary>
    [JsonPropertyName("tag")] public string? Tag { get; set; }

    /// <summary>Gets or sets the VCS revision, if applicable.</summary>
    [JsonPropertyName("revision")] public string? Revision { get; set; }

    /// <summary>Gets or sets the download strategy ("using"), if specified.</summary>
    [JsonPropertyName("using")] public string? Using { get; set; }

    /// <summary>Gets or sets the checksum of the archive.</summary>
    [JsonPropertyName("checksum")] public string? Checksum { get; set; }
}