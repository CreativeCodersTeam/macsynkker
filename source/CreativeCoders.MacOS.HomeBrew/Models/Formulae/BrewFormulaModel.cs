using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Formulae;

/// <summary>
/// Represents a Homebrew formula definition as returned by <c>brew info --json</c>.
/// </summary>
public class BrewFormulaModel
{
    /// <summary>Gets or sets the short name of the formula.</summary>
    [JsonPropertyName("name")] public string? Name { get; set; }

    /// <summary>Gets or sets the full name of the formula including tap if present.</summary>
    [JsonPropertyName("full_name")] public string? FullName { get; set; }

    /// <summary>Gets or sets the tap (repository) the formula belongs to.</summary>
    [JsonPropertyName("tap")] public string? Tap { get; set; }

    /// <summary>Gets or sets historical names for the formula.</summary>
    [JsonPropertyName("oldnames")] public string[]? OldNames { get; set; }

    /// <summary>Gets or sets alternative names (aliases) for the formula.</summary>
    [JsonPropertyName("aliases")] public string[]? Aliases { get; set; }

    /// <summary>Gets or sets versioned variants of this formula, if any.</summary>
    [JsonPropertyName("versioned_formulae")] public string[]? VersionedFormulae { get; set; }

    /// <summary>Gets or sets the description of the formula.</summary>
    [JsonPropertyName("desc")] public string? Desc { get; set; }

    /// <summary>Gets or sets the license identifier (SPDX) of the formula.</summary>
    [JsonPropertyName("license")] public string? License { get; set; }

    /// <summary>Gets or sets the homepage URL.</summary>
    [JsonPropertyName("homepage")] public string? Homepage { get; set; }

    /// <summary>Gets or sets the versions block (stable/head/bottle).</summary>
    [JsonPropertyName("versions")] public BrewVersionsModel? Versions { get; set; }

    /// <summary>Gets or sets the URLs used to fetch sources/binaries.</summary>
    [JsonPropertyName("urls")] public BrewUrlsModel? Urls { get; set; }

    /// <summary>Gets or sets the formula revision number.</summary>
    [JsonPropertyName("revision")] public int? Revision { get; set; }

    /// <summary>Gets or sets the version scheme used by the formula.</summary>
    [JsonPropertyName("version_scheme")] public int? VersionScheme { get; set; }

    /// <summary>Gets or sets the compatibility version, if provided.</summary>
    [JsonPropertyName("compatibility_version")] public string? CompatibilityVersion { get; set; }

    /// <summary>Gets or sets whether autobump is enabled for the formula.</summary>
    [JsonPropertyName("autobump")] public bool? Autobump { get; set; }

    /// <summary>Gets or sets an optional message explaining why autobump is disabled.</summary>
    [JsonPropertyName("no_autobump_message")] public string? NoAutobumpMessage { get; set; }

    /// <summary>Gets or sets whether to skip livecheck for the formula.</summary>
    [JsonPropertyName("skip_livecheck")] public bool? SkipLivecheck { get; set; }

    /// <summary>Gets or sets bottled artifact information.</summary>
    [JsonPropertyName("bottle")] public BrewBottleModel? Bottle { get; set; }

    /// <summary>Gets or sets a condition when a bottle may be poured only if a command returns true.</summary>
    [JsonPropertyName("pour_bottle_only_if")] public string? PourBottleOnlyIf { get; set; }

    /// <summary>Gets or sets a value indicating whether the formula is keg-only.</summary>
    [JsonPropertyName("keg_only")] public bool? KegOnly { get; set; }

    /// <summary>Gets or sets the reason for being keg-only, if any.</summary>
    [JsonPropertyName("keg_only_reason")] public string? KegOnlyReason { get; set; }

    /// <summary>Gets or sets compile-time options (rarely used).</summary>
    [JsonPropertyName("options")] public string[]? Options { get; set; }

    /// <summary>Gets or sets build-time dependencies.</summary>
    [JsonPropertyName("build_dependencies")] public string[]? BuildDependencies { get; set; }

    /// <summary>Gets or sets runtime dependencies.</summary>
    [JsonPropertyName("dependencies")] public string[]? Dependencies { get; set; }

    /// <summary>Gets or sets test-only dependencies.</summary>
    [JsonPropertyName("test_dependencies")] public string[]? TestDependencies { get; set; }

    /// <summary>Gets or sets recommended dependencies (optional set).</summary>
    [JsonPropertyName("recommended_dependencies")] public string[]? RecommendedDependencies { get; set; }

    /// <summary>Gets or sets optional dependencies.</summary>
    [JsonPropertyName("optional_dependencies")] public string[]? OptionalDependencies { get; set; }

    /// <summary>Gets or sets macOS-provided libraries used by this formula.</summary>
    [JsonPropertyName("uses_from_macos")] public string[]? UsesFromMacos { get; set; }

    /// <summary>Gets or sets version bounds for macOS uses, if present.</summary>
    [JsonPropertyName("uses_from_macos_bounds")] public string[]? UsesFromMacosBounds { get; set; }

    /// <summary>Gets or sets additional requirements if any. Represented as strings for simplicity.</summary>
    [JsonPropertyName("requirements")] public string[]? Requirements { get; set; }

    /// <summary>Gets or sets conflicting formulae.</summary>
    [JsonPropertyName("conflicts_with")] public string[]? ConflictsWith { get; set; }

    /// <summary>Gets or sets reasons for conflicts if provided.</summary>
    [JsonPropertyName("conflicts_with_reasons")] public string[]? ConflictsWithReasons { get; set; }

    /// <summary>Gets or sets files to be linked overwriting conflicts.</summary>
    [JsonPropertyName("link_overwrite")] public string[]? LinkOverwrite { get; set; }

    /// <summary>Gets or sets any additional caveats.</summary>
    [JsonPropertyName("caveats")] public string? Caveats { get; set; }

    /// <summary>Gets or sets the list of installed formula instances.</summary>
    [JsonPropertyName("installed")] public BrewInstalledFormulaModel[]? Installed { get; set; }

    /// <summary>Gets or sets the currently linked keg version if any.</summary>
    [JsonPropertyName("linked_keg")] public string? LinkedKeg { get; set; }

    /// <summary>Gets or sets a value indicating whether the formula is pinned.</summary>
    [JsonPropertyName("pinned")] public bool? Pinned { get; set; }

    /// <summary>Gets or sets a value indicating whether an update is available.</summary>
    [JsonPropertyName("outdated")] public bool? Outdated { get; set; }

    /// <summary>Gets or sets a value indicating whether the formula is deprecated.</summary>
    [JsonPropertyName("deprecated")] public bool? Deprecated { get; set; }

    /// <summary>Gets or sets the deprecation date, if deprecated.</summary>
    [JsonPropertyName("deprecation_date")] public string? DeprecationDate { get; set; }

    /// <summary>Gets or sets the deprecation reason, if any.</summary>
    [JsonPropertyName("deprecation_reason")] public string? DeprecationReason { get; set; }

    /// <summary>Gets or sets a suggested replacement formula if this one is deprecated.</summary>
    [JsonPropertyName("deprecation_replacement_formula")] public string? DeprecationReplacementFormula { get; set; }

    /// <summary>Gets or sets a suggested replacement cask if applicable.</summary>
    [JsonPropertyName("deprecation_replacement_cask")] public string? DeprecationReplacementCask { get; set; }

    /// <summary>Gets or sets a value indicating whether the formula is disabled.</summary>
    [JsonPropertyName("disabled")] public bool? Disabled { get; set; }

    /// <summary>Gets or sets the disable date, if disabled.</summary>
    [JsonPropertyName("disable_date")] public string? DisableDate { get; set; }

    /// <summary>Gets or sets the disable reason, if any.</summary>
    [JsonPropertyName("disable_reason")] public string? DisableReason { get; set; }

    /// <summary>Gets or sets the replacement formula, if disabled.</summary>
    [JsonPropertyName("disable_replacement_formula")] public string? DisableReplacementFormula { get; set; }

    /// <summary>Gets or sets the replacement cask, if disabled.</summary>
    [JsonPropertyName("disable_replacement_cask")] public string? DisableReplacementCask { get; set; }

    /// <summary>Gets or sets whether a post-install step is defined.</summary>
    [JsonPropertyName("post_install_defined")] public bool? PostInstallDefined { get; set; }

    /// <summary>Gets or sets service information for the formula, if any.</summary>
    [JsonPropertyName("service")] public object? Service { get; set; }

    /// <summary>Gets or sets the tap git head commit SHA.</summary>
    [JsonPropertyName("tap_git_head")] public string? TapGitHead { get; set; }

    /// <summary>Gets or sets the ruby source path of the formula.</summary>
    [JsonPropertyName("ruby_source_path")] public string? RubySourcePath { get; set; }

    /// <summary>Gets or sets the checksum of the ruby source.</summary>
    [JsonPropertyName("ruby_source_checksum")] public BrewRubySourceChecksumModel? RubySourceChecksum { get; set; }
}
