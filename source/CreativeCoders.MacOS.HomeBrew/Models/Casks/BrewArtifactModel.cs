using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents the various artifact entries of a cask (uninstall/app/zap).
/// </summary>
public class BrewArtifactModel
{
    /// <summary>Gets or sets uninstall instructions.</summary>
    [JsonPropertyName("uninstall")] public BrewUninstallModel[]? Uninstall { get; set; }

    /// <summary>Gets or sets the list of apps included by the cask.</summary>
    [JsonPropertyName("app")] public string[]? App { get; set; }

    /// <summary>Gets or sets zap instructions for cleanup.</summary>
    [JsonPropertyName("zap")] public BrewZapModel[]? Zap { get; set; }
}