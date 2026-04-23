using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Casks;

/// <summary>
/// Represents uninstall instructions for a cask.
/// </summary>
public class BrewUninstallModel
{
    /// <summary>Gets or sets the bundle identifier(s) to quit before uninstall.</summary>
    [JsonPropertyName("quit")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Quit { get; set; }

    /// <summary>Gets or sets the package identifier(s) to forget via pkgutil.</summary>
    [JsonPropertyName("pkgutil")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Pkgutil { get; set; }

    /// <summary>Gets or sets the launchctl service(s) to remove.</summary>
    [JsonPropertyName("launchctl")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Launchctl { get; set; }

    /// <summary>Gets or sets the file(s) to delete.</summary>
    [JsonPropertyName("delete")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Delete { get; set; }

    /// <summary>Gets or sets the directories to remove.</summary>
    [JsonPropertyName("rmdir")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Rmdir { get; set; }

    /// <summary>Gets or sets a script to execute during uninstall.</summary>
    [JsonPropertyName("script")]
    public JsonNode? Script { get; set; }
}
