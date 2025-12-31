using CreativeCoders.Core.Collections;
using CreativeCoders.DependencyInjection;
using CreativeCoders.MacOS.Core;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.ProcessUtils;
using Microsoft.Extensions.DependencyInjection;

namespace SampleConsoleApp;

static class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddProcessUtils();
        services.AddMacOSCore();
        services.AddHomeBrew();
        services.AddObjectFactory();

        var sp = services.BuildServiceProvider();

        var programLocator = sp.GetRequiredService<IProgramLocator>();

        var brewPath = await programLocator.LocateProgramAsync("brew").ConfigureAwait(false);

        Console.Write("brew path:");
        Console.WriteLine(brewPath);

        var brew = sp.GetRequiredService<IBrewInstalledSoftware>();

        var installedSoftware = await brew.GetInstalledSoftwareAsync().ConfigureAwait(false);

        Console.WriteLine("Installed formulae:");

        installedSoftware.Formulae.ForEach(x => Console.WriteLine($"- {x.Name}: {x.Versions?.Stable}"));

        Console.WriteLine();
        Console.WriteLine("Installed casks:");

        installedSoftware.Casks.ForEach(x =>
            Console.WriteLine($"- {x.Name?.FirstOrDefault("NAME_NOT_FOUND")}: {x.Version}"));
    }
}
