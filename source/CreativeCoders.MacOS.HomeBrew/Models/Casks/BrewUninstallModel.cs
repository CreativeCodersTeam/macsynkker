using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models;

/// <summary>
/// Represents uninstall instructions for a cask.
/// </summary>
public class BrewUninstallModel
{
    /// <summary>Gets or sets the bundle identifier(s) to quit before uninstall.</summary>
    [JsonPropertyName("quit")] public string? Quit { get; set; }
}