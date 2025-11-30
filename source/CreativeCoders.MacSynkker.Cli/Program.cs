using System.Runtime.Versioning;

namespace CreativeCoders.MacSynkker.Cli;

internal static class Program
{
    [SupportedOSPlatform( "macos")]
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
    }
}
