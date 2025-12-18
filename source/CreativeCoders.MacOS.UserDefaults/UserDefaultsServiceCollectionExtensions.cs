using System.IO.Abstractions;
using CreativeCoders.ProcessUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.MacOS.UserDefaults;

public static class UserDefaultsServiceCollectionExtensions
{
    public static void AddMacOSUserDefaults(this IServiceCollection services,
        bool useCoreFoundationPlistConverter = false)
    {
        services.TryAddSingleton<IFileSystem, FileSystem>();
        services.AddProcessUtils();
        services.TryAddSingleton<IUserDefaultsEnumerator, UserDefaultsEnumerator>();
        services.TryAddSingleton<IUserDefaultsExporter, UserDefaultsExporter>();
        services.TryAddSingleton<IUserDefaultsImporter, UserDefaultsImporter>();

        if (useCoreFoundationPlistConverter)
        {
            services.TryAddSingleton<IPlistConverter, CoreFoundationPlistConverter>();
        }
        else
        {
            services.TryAddSingleton<IPlistConverter, PlutilPlistConverter>();
        }
    }
}
