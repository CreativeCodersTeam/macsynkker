using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Cli.Parsing;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[UsedImplicitly]
[CliCommand(["defaults", "domains", "export"])]
public class ExportDomainCommand(
    IUserDefaultsExporter userDefaultsExporter,
    IPlistConverter plistConverter,
    IAnsiConsole ansiConsole)
    : ICliCommand<ExportDomainOptions>
{
    private readonly IPlistConverter _plistConverter = Ensure.NotNull(plistConverter);

    private readonly IUserDefaultsExporter _userDefaultsExporter = Ensure.NotNull(userDefaultsExporter);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(ExportDomainOptions options)
    {
        _ansiConsole.PrintBlock()
            .WriteLine($"Export domain '{options.DomainName}' to '{options.OutputFileName}'")
            .WriteLine();

        await _userDefaultsExporter.ExportDomainAsync(options.DomainName, options.OutputFileName).ConfigureAwait(false);

        if (options.PlistFormat != PlistFormat.Original)
        {
            await _plistConverter
                .ConvertFileAsync(options.OutputFileName, options.OutputFileName, options.PlistFormat)
                .ConfigureAwait(false);
        }


        return new CommandResult();
    }
}

[UsedImplicitly]
public class ExportDomainOptions
{
    [OptionValue(0, HelpText = "The domain name to export", IsRequired = true)]
    public string DomainName { get; set; } = string.Empty;

    [OptionParameter('o', "output",
        HelpText = "The filename to export the domain to", IsRequired = true)]
    public string OutputFileName { get; set; } = string.Empty;

    [OptionParameter('f', "format", HelpText = "The plist format to export the domain to")]
    public PlistFormat PlistFormat { get; set; } = PlistFormat.Original;
}
