using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Cli;

public interface IBrewExecutor
{
    Task<BrewInstalledModel> GetInstalledSoftwareAsync();

    Task<BrewCaskModel[]> GetInstalledCasksAsync();

    Task<BrewFormulaModel[]> GetInstalledFormulasAsync();
}
