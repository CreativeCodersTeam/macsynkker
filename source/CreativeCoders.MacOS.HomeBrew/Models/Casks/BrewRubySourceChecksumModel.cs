using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents the checksum of the Ruby source file defining the cask.
/// </summary>
public class BrewRubySourceChecksumModel
{
    /// <summary>Gets or sets the SHA-256 checksum.</summary>
    [JsonPropertyName("sha256")] public string? Sha256 { get; set; }
}