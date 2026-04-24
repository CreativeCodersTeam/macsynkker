using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Casks;

/// <summary>
/// Represents zap cleanup instructions.
/// </summary>
public class BrewZapModel
{
    /// <summary>Gets or sets items that should be moved to trash when zapping.</summary>
    [JsonPropertyName("trash")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Trash { get; set; }

    /// <summary>Gets or sets directories to remove when zapping.</summary>
    [JsonPropertyName("rmdir")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Rmdir { get; set; }

    /// <summary>Gets or sets package identifiers to forget via pkgutil when zapping.</summary>
    [JsonPropertyName("pkgutil")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Pkgutil { get; set; }

    /// <summary>Gets or sets files to delete when zapping.</summary>
    [JsonPropertyName("delete")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Delete { get; set; }

    /// <summary>Gets or sets launchctl services to remove when zapping.</summary>
    [JsonPropertyName("launchctl")]
    [JsonConverter(typeof(SingleOrArrayConverter))]
    public string[]? Launchctl { get; set; }
}
