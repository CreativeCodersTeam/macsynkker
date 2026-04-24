namespace CreativeCoders.MacOS.HomeBrew.Import;

/// <summary>Identifies the kind of operation being performed during an import step.</summary>
public enum BrewImportStep
{
    Tap,
    InstallFormula,
    InstallCask
}

/// <summary>Identifies whether an import step is about to start, has succeeded, or has failed.</summary>
public enum BrewImportStepState
{
    Starting,
    Succeeded,
    Failed
}

/// <summary>
/// Reports the current progress of a Homebrew import operation. Passed to
/// <see cref="IProgress{T}"/> callbacks so callers can display live feedback.
/// </summary>
public class BrewImportProgress
{
    /// <summary>Gets the kind of operation being performed.</summary>
    public required BrewImportStep Step { get; init; }

    /// <summary>Gets whether the step is starting, succeeded, or failed.</summary>
    public required BrewImportStepState State { get; init; }

    /// <summary>Gets the tap, formula name, or cask token of the current step.</summary>
    public required string Target { get; init; }

    /// <summary>Gets the failure details when <see cref="State"/> is <see cref="BrewImportStepState.Failed"/>.</summary>
    public BrewInstallFailedException? Error { get; init; }
}
