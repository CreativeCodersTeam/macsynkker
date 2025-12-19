using CreativeCoders.MacOS.Core.Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.MacOS.Core;

public static class MacOSCoreServiceCollectionExtensions
{
    public static IServiceCollection AddMacOSCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IProgramLocator, DefaultProgramLocator>();
        services.TryAddSingleton<ICoreFoundation, MacOSCoreFoundation>();

        return services;
    }
}
