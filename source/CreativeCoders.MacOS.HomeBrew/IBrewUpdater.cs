namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewUpdater
{
    Task UpdateAsync(bool force = false);
}
