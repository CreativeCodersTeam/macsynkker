using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.Import;

[UsedImplicitly]
public class BrewImportOptions
{
    [OptionParameter('i', "input", HelpText = "The file path to import the Homebrew software from",
        IsRequired = true)]
    public string InputPath { get; set; } = string.Empty;
}
