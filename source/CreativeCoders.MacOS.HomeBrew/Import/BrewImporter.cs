using System.Text.Json;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Models.Export;

namespace CreativeCoders.MacOS.HomeBrew.Import;

/// <summary>
/// Default <see cref="IBrewImporter"/> implementation. Delegates the actual <c>brew</c> calls to
/// an injected <see cref="IBrewInstaller"/> and orchestrates taps, formulae and casks.
/// </summary>
public class BrewImporter : IBrewImporter
{
    private readonly IBrewInstaller _installer;

    public BrewImporter(IBrewInstaller installer)
    {
        _installer = Ensure.NotNull(installer);
    }

    public async Task<BrewExportModel> ReadFileAsync(string filePath)
    {
        Ensure.IsNotNullOrWhitespace(filePath);

        await using var stream = File.OpenRead(filePath);

        var model = await JsonSerializer.DeserializeAsync<BrewExportModel>(stream).ConfigureAwait(false);

        return model ?? new BrewExportModel();
    }

    public async Task ImportAsync(BrewExportModel exportModel, IProgress<BrewImportProgress>? progress = null)
    {
        Ensure.NotNull(exportModel);

        var failures = new List<BrewInstallFailedException>();

        foreach (var tap in CollectDistinctTaps(exportModel))
        {
            await TryRunAsync(BrewImportStep.Tap, tap, () => _installer.TapAsync(tap), failures, progress)
                .ConfigureAwait(false);
        }

        foreach (var formula in exportModel.Formulae)
        {
            if (string.IsNullOrWhiteSpace(formula.Name))
            {
                continue;
            }

            await TryRunAsync(BrewImportStep.InstallFormula, formula.Name,
                () => _installer.InstallFormulaAsync(formula.Name), failures, progress).ConfigureAwait(false);
        }

        foreach (var cask in exportModel.Casks)
        {
            if (string.IsNullOrWhiteSpace(cask.Token))
            {
                continue;
            }

            await TryRunAsync(BrewImportStep.InstallCask, cask.Token,
                () => _installer.InstallCaskAsync(cask.Token), failures, progress).ConfigureAwait(false);
        }

        if (failures.Count > 0)
        {
            throw new BrewImportFailedException(failures);
        }
    }

    public async Task ImportFromFileAsync(string filePath, IProgress<BrewImportProgress>? progress = null)
    {
        var exportModel = await ReadFileAsync(filePath).ConfigureAwait(false);

        await ImportAsync(exportModel, progress).ConfigureAwait(false);
    }

    private static IEnumerable<string> CollectDistinctTaps(BrewExportModel exportModel)
    {
        var taps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var formula in exportModel.Formulae)
        {
            if (!string.IsNullOrWhiteSpace(formula.Tap))
            {
                taps.Add(formula.Tap);
            }
        }

        foreach (var cask in exportModel.Casks)
        {
            if (!string.IsNullOrWhiteSpace(cask.Tap))
            {
                taps.Add(cask.Tap);
            }
        }

        return taps;
    }

    private static async Task TryRunAsync(
        BrewImportStep step,
        string target,
        Func<Task> action,
        List<BrewInstallFailedException> failures,
        IProgress<BrewImportProgress>? progress)
    {
        progress?.Report(new BrewImportProgress
        {
            Step = step, State = BrewImportStepState.Starting, Target = target
        });

        try
        {
            await action().ConfigureAwait(false);

            progress?.Report(new BrewImportProgress
            {
                Step = step, State = BrewImportStepState.Succeeded, Target = target
            });
        }
        catch (BrewInstallFailedException e)
        {
            failures.Add(e);

            progress?.Report(new BrewImportProgress
            {
                Step = step, State = BrewImportStepState.Failed, Target = target, Error = e
            });
        }
    }
}
