using CreativeCoders.MacOS.HomeBrew.Models;

namespace CreativeCoders.MacOS.HomeBrew;

public interface IBrewInstalledSoftware
{
    Task<BrewInstalledModel> GetInstalledSoftwareAsync();
}
