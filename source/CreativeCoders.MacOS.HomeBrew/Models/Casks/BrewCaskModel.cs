using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.MacOS.HomeBrew.Models.Casks;

/// <summary>
/// Represents a Homebrew cask definition as returned by <c>brew info --cask --json</c>.
/// </summary>
[UsedImplicitly]
public class BrewCaskModel
{
    /// <summary>Gets or sets the short token of the cask.</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>Gets or sets the full token of the cask.</summary>
    [JsonPropertyName("full_token")]
    public string? FullToken { get; set; }

    /// <summary>Gets or sets the list of old tokens.</summary>
    [JsonPropertyName("old_tokens")]
    public string[]? OldTokens { get; set; }

    /// <summary>Gets or sets the tap (repository) the cask belongs to.</summary>
    [JsonPropertyName("tap")]
    public string? Tap { get; set; }

    /// <summary>Gets or sets the display names of the cask.</summary>
    [JsonPropertyName("name")]
    public string[]? Name { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>Gets or sets the homepage URL.</summary>
    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    /// <summary>Gets or sets the download URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets additional URL specifications.</summary>
    [JsonPropertyName("url_specs")]
    public BrewUrlSpecsModel? UrlSpecs { get; set; }

    /// <summary>Gets or sets the cask version.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>Gets or sets a value indicating whether autobump is enabled.</summary>
    [JsonPropertyName("autobump")]
    public bool? Autobump { get; set; }

    /// <summary>Gets or sets the message explaining why autobump is disabled.</summary>
    [JsonPropertyName("no_autobump_message")]
    public string? NoAutobumpMessage { get; set; }

    /// <summary>Gets or sets a value indicating whether to skip livecheck.</summary>
    [JsonPropertyName("skip_livecheck")]
    public bool? SkipLivecheck { get; set; }

    /// <summary>Gets or sets the installed version (if any).</summary>
    [JsonPropertyName("installed")]
    public string? Installed { get; set; }

    /// <summary>Gets or sets the installed UNIX time (seconds since epoch).</summary>
    [JsonPropertyName("installed_time")]
    public long? InstalledTime { get; set; }

    /// <summary>Gets or sets the bundle version (CFBundleVersion).</summary>
    [JsonPropertyName("bundle_version")]
    public string? BundleVersion { get; set; }

    /// <summary>Gets or sets the bundle short version (CFBundleShortVersionString).</summary>
    [JsonPropertyName("bundle_short_version")]
    public string? BundleShortVersion { get; set; }

    /// <summary>Gets or sets a value indicating whether this cask is outdated.</summary>
    [JsonPropertyName("outdated")]
    public bool? Outdated { get; set; }

    /// <summary>Gets or sets the SHA256 checksum of the artifact.</summary>
    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }

    /// <summary>Gets or sets the artifacts (uninstall/app/zap) configuration.</summary>
    [JsonPropertyName("artifacts")]
    public BrewArtifactModel[]? Artifacts { get; set; }

    /// <summary>Gets or sets additional caveats shown to the user.</summary>
    [JsonPropertyName("caveats")]
    public string? Caveats { get; set; }

    /// <summary>Gets or sets dependency constraints (e.g., macOS version).</summary>
    [JsonPropertyName("depends_on")]
    public BrewDependsOnModel? DependsOn { get; set; }

    /// <summary>Gets or sets conflicting casks (if any).</summary>
    [JsonPropertyName("conflicts_with")]
    public JsonNode? ConflictsWith { get; set; }

    /// <summary>Gets or sets the container info, if present (rare).</summary>
    [JsonPropertyName("container")]
    public object? Container { get; set; }

    /// <summary>Gets or sets an optional rename list.</summary>
    [JsonPropertyName("rename")]
    public string[]? Rename { get; set; }

    /// <summary>Gets or sets whether the cask auto-updates itself.</summary>
    [JsonPropertyName("auto_updates")]
    public bool? AutoUpdates { get; set; }

    /// <summary>Gets or sets a value indicating whether the cask is deprecated.</summary>
    [JsonPropertyName("deprecated")]
    public bool? Deprecated { get; set; }

    /// <summary>Gets or sets the deprecation date, if deprecated.</summary>
    [JsonPropertyName("deprecation_date")]
    public string? DeprecationDate { get; set; }

    /// <summary>Gets or sets the deprecation reason, if any.</summary>
    [JsonPropertyName("deprecation_reason")]
    public string? DeprecationReason { get; set; }

    /// <summary>Gets or sets the suggested replacement formula, if any.</summary>
    [JsonPropertyName("deprecation_replacement_formula")]
    public string? DeprecationReplacementFormula { get; set; }

    /// <summary>Gets or sets the suggested replacement cask, if any.</summary>
    [JsonPropertyName("deprecation_replacement_cask")]
    public string? DeprecationReplacementCask { get; set; }

    /// <summary>Gets or sets a value indicating whether the cask is disabled.</summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    /// <summary>Gets or sets the disable date, if disabled.</summary>
    [JsonPropertyName("disable_date")]
    public string? DisableDate { get; set; }

    /// <summary>Gets or sets the disable reason, if any.</summary>
    [JsonPropertyName("disable_reason")]
    public string? DisableReason { get; set; }

    /// <summary>Gets or sets the replacement formula, if disabled.</summary>
    [JsonPropertyName("disable_replacement_formula")]
    public string? DisableReplacementFormula { get; set; }

    /// <summary>Gets or sets the replacement cask, if disabled.</summary>
    [JsonPropertyName("disable_replacement_cask")]
    public string? DisableReplacementCask { get; set; }

    /// <summary>Gets or sets the tap git head commit SHA.</summary>
    [JsonPropertyName("tap_git_head")]
    public string? TapGitHead { get; set; }

    /// <summary>Gets or sets the preferred languages (if any).</summary>
    [JsonPropertyName("languages")]
    public string[]? Languages { get; set; }

    /// <summary>Gets or sets the ruby source path of the cask.</summary>
    [JsonPropertyName("ruby_source_path")]
    public string? RubySourcePath { get; set; }

    /// <summary>Gets or sets the checksum of the ruby source.</summary>
    [JsonPropertyName("ruby_source_checksum")]
    public BrewRubySourceChecksumModel? RubySourceChecksum { get; set; }
}
