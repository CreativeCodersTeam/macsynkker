using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a compile-time option for a Homebrew formula.
/// </summary>
[UsedImplicitly]
public class BrewFormulaOptionModel
{
    /// <summary>Gets or sets the option flag (e.g. <c>--without-mono</c>).</summary>
    [JsonPropertyName("option")]
    public string? Option { get; set; }

    /// <summary>Gets or sets the human-readable description of the option.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
