using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults.Domains.ListDomains;

[UsedImplicitly]
public class ListDomainsOptions
{
    [OptionParameter('f', "filter", HelpText = "Filter domains by name pattern.")]
    public string FilterPattern { get; set; } = string.Empty;
}
