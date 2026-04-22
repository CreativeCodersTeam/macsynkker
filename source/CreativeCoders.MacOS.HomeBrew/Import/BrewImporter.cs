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

    public async Task ImportAsync(BrewExportModel exportModel)
    {
        Ensure.NotNull(exportModel);

        var failures = new List<BrewInstallFailedException>();

        // Add all non-default taps once before installing so that formulae / casks from custom
        // taps can be resolved by `brew install`.
        foreach (var tap in CollectDistinctTaps(exportModel))
        {
            await TryRunAsync(() => _installer.TapAsync(tap), failures).ConfigureAwait(false);
        }

        foreach (var formula in exportModel.Formulae)
        {
            if (string.IsNullOrWhiteSpace(formula.Name))
            {
                continue;
            }

            await TryRunAsync(() => _installer.InstallFormulaAsync(formula.Name), failures).ConfigureAwait(false);
        }

        foreach (var cask in exportModel.Casks)
        {
            if (string.IsNullOrWhiteSpace(cask.Token))
            {
                continue;
            }

            await TryRunAsync(() => _installer.InstallCaskAsync(cask.Token), failures).ConfigureAwait(false);
        }

        if (failures.Count > 0)
        {
            throw new BrewImportFailedException(failures);
        }
    }

    public async Task ImportFromFileAsync(string filePath)
    {
        var exportModel = await ReadFileAsync(filePath).ConfigureAwait(false);

        await ImportAsync(exportModel).ConfigureAwait(false);
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

    private static async Task TryRunAsync(Func<Task> action, List<BrewInstallFailedException> failures)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (BrewInstallFailedException e)
        {
            failures.Add(e);
        }
    }
}
