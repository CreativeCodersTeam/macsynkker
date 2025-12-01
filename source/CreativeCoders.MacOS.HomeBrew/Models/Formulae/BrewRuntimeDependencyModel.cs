using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a runtime dependency item within an installed formula entry.
/// </summary>
public class BrewRuntimeDependencyModel
{
    /// <summary>Gets or sets the full name of the dependency.</summary>
    [JsonPropertyName("full_name")] public string? FullName { get; set; }

    /// <summary>Gets or sets the version of the dependency.</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>Gets or sets the revision number of the dependency.</summary>
    [JsonPropertyName("revision")] public int? Revision { get; set; }

    /// <summary>Gets or sets the bottle rebuild number.</summary>
    [JsonPropertyName("bottle_rebuild")] public int? BottleRebuild { get; set; }

    /// <summary>Gets or sets the package version string.</summary>
    [JsonPropertyName("pkg_version")] public string? PackageVersion { get; set; }

    /// <summary>Gets or sets whether the dependency was declared directly by the formula.</summary>
    [JsonPropertyName("declared_directly")] public bool? DeclaredDirectly { get; set; }
}