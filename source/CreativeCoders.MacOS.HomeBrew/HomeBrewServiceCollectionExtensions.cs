using CreativeCoders.MacOS.HomeBrew.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.MacOS.HomeBrew;

public static class HomeBrewServiceCollectionExtensions
{
    public static IServiceCollection AddHomeBrew(this IServiceCollection services)
    {
        services.TryAddSingleton<IBrewExecutor, BrewExecutor>();
        services.TryAddSingleton<IBrewCore, BrewCore>();

        return services;
    }
}
