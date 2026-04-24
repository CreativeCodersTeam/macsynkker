using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.MacOS.HomeBrew.Models.Export;

/// <summary>
/// Represents a single Homebrew cask entry inside an export file.
/// </summary>
[UsedImplicitly]
public class BrewExportCaskModel
{
    /// <summary>Gets or sets the cask token (preferably the full token including the tap).</summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tap the cask belongs to. <c>null</c> when the cask resides in the
    /// default cask tap (<c>homebrew/cask</c>).
    /// </summary>
    [JsonPropertyName("tap")]
    public string? Tap { get; set; }
}
