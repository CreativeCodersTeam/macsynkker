using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[UsedImplicitly]
public class ImportDomainOptions
{
    [OptionParameter('i', "input", HelpText = "The filename or directory to import the user defaults from",
        IsRequired = true)]
    public string InputPath { get; set; } = string.Empty;

    [OptionParameter('d', "domain", HelpText = "The domain name to import the user defaults to")]
    public string DomainName { get; set; } = string.Empty;
}
