namespace CreativeCoders.MacOS.HomeBrew;

public class BrewCore : IBrewCore
{
    public BrewCore()
    {

    }

    public bool IsInstalled => false;

    public Task InstallHomeBrewAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IBrewSoftware>> GetInstalledSoftwareAsync(InstallationType installationType)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IBrewCask>> GetInstalledCasksAsync(InstallationType installationType)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IBrewFormula>> GetInstalledFormulasAsync(InstallationType installationType)
    {
        throw new NotImplementedException();
    }

    public Task<IBrewCask> GetCaskAsync(string caskName)
    {
        throw new NotImplementedException();
    }

    public Task<IBrewFormula> GetFormulaAsync(string formulaName)
    {
        throw new NotImplementedException();
    }
}

public enum InstallationType
{
    Install,
    Dependency
}

public interface IBrewCore
{
    bool IsInstalled { get; }

    Task InstallHomeBrewAsync();

    Task<IEnumerable<IBrewSoftware>> GetInstalledSoftwareAsync(InstallationType installationType);

    Task<IEnumerable<IBrewCask>> GetInstalledCasksAsync(InstallationType installationType);

    Task<IEnumerable<IBrewFormula>> GetInstalledFormulasAsync(InstallationType installationType);

    Task<IBrewCask> GetCaskAsync(string caskName);

    Task<IBrewFormula> GetFormulaAsync(string formulaName);
}

public interface IBrewSoftware
{
    string Name { get; }

    string Version { get; }

    IEnumerable<IBrewSoftware> Dependencies { get; }
}

public interface IBrewCask : IBrewSoftware
{

}

public interface IBrewFormula : IBrewSoftware
{

}
