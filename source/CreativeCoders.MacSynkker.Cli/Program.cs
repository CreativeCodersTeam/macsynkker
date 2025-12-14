using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CreativeCoders.Cli.Hosting;
using CreativeCoders.Cli.Hosting.Help;
using CreativeCoders.MacOS.HomeBrew;
using CreativeCoders.MacOS.UserDefaults;

namespace CreativeCoders.MacSynkker.Cli;

internal static class Program
{
    [SupportedOSPlatform( "macos")]
    public static async Task<int> Main(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Console.WriteLine("This program can only be executed on macOS.");

            return int.MaxValue;
        }

        var result = await CliHostBuilder.Create()
            .ConfigureServices(x =>
            {
                x.AddMacOSUserDefaults();
                x.AddHomeBrew();
            })
            .EnableHelp(HelpCommandKind.CommandOrArgument)
            .Build()
            .RunAsync(args).ConfigureAwait(false);

        return result.ExitCode;
    }
}
