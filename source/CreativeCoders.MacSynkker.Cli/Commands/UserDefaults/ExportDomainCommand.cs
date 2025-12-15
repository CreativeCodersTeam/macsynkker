using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Cli.Parsing;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[CliCommand(["defaults", "domains", "export"])]
public class ExportDomainCommand(IUserDefaults userDefaults, IAnsiConsole ansiConsole)
    : ICliCommand<ExportDomainOptions>
{
    private readonly IUserDefaults _userDefaults = Ensure.NotNull(userDefaults);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(ExportDomainOptions options)
    {
        _ansiConsole.PrintBlock()
            .WriteLine($"Export domain '{options.DomainName}' to '{options.OutputFileName}'")
            .WriteLine();

        await _userDefaults.ExportDomainAsync(options.DomainName, options.OutputFileName).ConfigureAwait(false);

        return new CommandResult();
    }
}

public class ExportDomainOptions
{
    [OptionValue(0, HelpText = "The domain name to export", IsRequired = true)]
    public string DomainName { get; set; } = string.Empty;

    [OptionParameter('f', "filename",
        HelpText = "The filename to export the domain to", IsRequired = true)]
    public string OutputFileName { get; set; } = string.Empty;
}
