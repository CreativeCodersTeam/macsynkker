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
            .Where(x => x.Installed?.Any(y => y.Version != x.Versions?.Stable) == true)
            .ToArray();
    }

    public static BrewFormulaModel[] GetFormulae(this BrewInstalledModel installedModel, bool onlyOutdated)
    {
        return onlyOutdated
            ? installedModel.GetOutdatedFormulae()
            : installedModel.Formulae;
    }
}
