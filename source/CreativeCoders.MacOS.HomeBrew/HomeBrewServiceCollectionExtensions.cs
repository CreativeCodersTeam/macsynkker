using CreativeCoders.ProcessUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.MacOS.HomeBrew;

public static class HomeBrewServiceCollectionExtensions
{
    public static IServiceCollection AddHomeBrew(this IServiceCollection services)
    {
        services.AddProcessUtils();

        services.TryAddSingleton<IBrewInfo, BrewInfo>();
        services.TryAddSingleton<IBrewInstalledSoftware, BrewInstalledSoftware>();
        services.TryAddSingleton<IBrewUpgrader, BrewUpgrader>();

        return services;
    }
}
