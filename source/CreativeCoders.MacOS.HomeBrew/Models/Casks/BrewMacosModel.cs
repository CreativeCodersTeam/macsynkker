using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents macOS constraints for the cask.
/// </summary>
public class BrewMacosModel
{
    /// <summary>Gets or sets versions that are greater or equal to the listed ones.</summary>
    [JsonPropertyName(">=")] public string[]? GreaterOrEqual { get; set; }
}