using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents zap cleanup instructions.
/// </summary>
public class BrewZapModel
{
    /// <summary>Gets or sets items that should be moved to trash when zapping.</summary>
    [JsonPropertyName("trash")] public string[]? Trash { get; set; }
}