using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Export;

[UsedImplicitly]
public class BrewExportOptions
{
    [OptionParameter('o', "output", HelpText = "The file path to export the installed software to",
        IsRequired = true)]
    public string OutputPath { get; set; } = string.Empty;
}
