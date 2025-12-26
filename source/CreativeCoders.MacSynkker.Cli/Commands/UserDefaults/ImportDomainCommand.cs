using CreativeCoders.Cli.Core;
using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

public class ImportDomainCommand : ICliCommand<ImportDomainOptions>
{
    public Task<CommandResult> ExecuteAsync(ImportDomainOptions options)
    {
        throw new NotImplementedException();
    }
}

public class ImportDomainOptions
{
    [OptionParameter('i', "input", HelpText = "The filename or directory to import the user defaults from",
        IsRequired = true)]
    public string InputPath { get; set; } = string.Empty;

    [OptionParameter('d', "domain", HelpText = "The domain name to import the user defaults to")]
    public string DomainName { get; set; } = string.Empty;
}
