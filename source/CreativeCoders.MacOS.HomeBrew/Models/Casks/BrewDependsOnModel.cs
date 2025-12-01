using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents dependency constraints of a cask.
/// </summary>
public class BrewDependsOnModel
{
    /// <summary>Gets or sets macOS version constraints.</summary>
    [JsonPropertyName("macos")] public BrewMacosModel? Macos { get; set; }
}