namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewInfo
{
    Task<bool> IsInstalledAsync();

    Task<string> GetVersionAsync();
}
