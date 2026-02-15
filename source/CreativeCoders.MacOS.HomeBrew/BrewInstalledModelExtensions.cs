using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew;

public static class BrewInstalledModelExtensions
{
    public static BrewCaskModel[] GetOutdatedCasks(this BrewInstalledModel installedModel)
    {
        return installedModel.Casks
            .Where(x => x.Installed != x.Version)
            .ToArray();
    }

    public static BrewCaskModel[] GetCasks(this BrewInstalledModel installedModel, bool onlyOutdated)
    {
        return onlyOutdated
            ? installedModel.GetOutdatedCasks()
            : installedModel.Casks;
    }

    public static BrewFormulaModel[] GetOutdatedFormulae(this BrewInstalledModel installedModel)
    {
        return installedModel.Formulae
            .Where(IsFormulaOutdated)
            .ToArray();
    }

    private static bool IsFormulaOutdated(BrewFormulaModel formula)
    {
        return formula.Installed?.Any(y => !FormulaeVersionsAreEqual(y.Version, formula.Versions?.Stable)) == true;
    }

    private static bool FormulaeVersionsAreEqual(string? installedVersion, string? availableVersion)
    {
        if (installedVersion == null || availableVersion == null)
        {
            return installedVersion == availableVersion;
        }

        return installedVersion == availableVersion || installedVersion.StartsWith($"{availableVersion}_");
    }

    public static BrewFormulaModel[] GetFormulae(this BrewInstalledModel installedModel, bool onlyOutdated)
    {
        return onlyOutdated
            ? installedModel.GetOutdatedFormulae()
            : installedModel.Formulae;
    }
}
