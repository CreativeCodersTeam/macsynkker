using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a concrete installation instance of a formula on the system.
/// </summary>
public class BrewInstalledFormulaModel
{
    /// <summary>Gets or sets the installed version.</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>Gets or sets the used options during installation.</summary>
    [JsonPropertyName("used_options")] public string[]? UsedOptions { get; set; }

    /// <summary>Gets or sets a value indicating whether this was built from a bottle.</summary>
    [JsonPropertyName("built_as_bottle")] public bool? BuiltAsBottle { get; set; }

    /// <summary>Gets or sets a value indicating whether this was poured from a bottle.</summary>
    [JsonPropertyName("poured_from_bottle")] public bool? PouredFromBottle { get; set; }

    /// <summary>Gets or sets the installation UNIX time (seconds since epoch).</summary>
    [JsonPropertyName("time")] public long? Time { get; set; }

    /// <summary>Gets or sets runtime dependencies that were pulled in for this installation.</summary>
    [JsonPropertyName("runtime_dependencies")] public BrewRuntimeDependencyModel[]? RuntimeDependencies { get; set; }

    /// <summary>Gets or sets a value indicating whether this was installed as a dependency.</summary>
    [JsonPropertyName("installed_as_dependency")] public bool? InstalledAsDependency { get; set; }

    /// <summary>Gets or sets a value indicating whether this was installed on explicit user request.</summary>
    [JsonPropertyName("installed_on_request")] public bool? InstalledOnRequest { get; set; }
}