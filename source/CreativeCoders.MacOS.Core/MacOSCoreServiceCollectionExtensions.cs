using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.MacOS.Core;

public static class MacOSCoreServiceCollectionExtensions
{
    public static IServiceCollection AddMacOSCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IProgramLocator, DefaultProgramLocator>();

        return services;
    }
}
