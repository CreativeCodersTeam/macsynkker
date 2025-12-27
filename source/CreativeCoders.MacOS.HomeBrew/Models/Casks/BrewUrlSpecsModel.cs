using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Casks;

/// <summary>
/// Represents additional URL specifications for the cask.
/// </summary>
public class BrewUrlSpecsModel
{
    /// <summary>Gets or sets the verified host/path for the URL.</summary>
    [JsonPropertyName("verified")]
    public string? Verified { get; set; }
}
