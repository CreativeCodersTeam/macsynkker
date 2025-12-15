using CreativeCoders.DependencyInjection;
using CreativeCoders.ProcessUtils;
using Microsoft.Extensions.DependencyInjection;

namespace CreativeCoders.MacOS.UserDefaults;

public static class UserDefaultsServiceCollectionExtensions
{
    public static void AddMacOSUserDefaults(this IServiceCollection services)
    {
        services.AddProcessUtils();
        services.AddSingleton<IUserDefaults, DefaultUserDefaults>();
        services.AddObjectFactory();
    }
}
