using CreativeCoders.Cli.Core;
using CreativeCoders.Core.IO;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.MacSynkker.Cli.Commands.UserDefaults;

[UsedImplicitly]
public class ExportDomainOptions : IOptionsValidation
{
    [OptionValue(0, HelpText = "The domain name to export")]
    public string DomainName { get; set; } = string.Empty;

    [OptionParameter('o', "output",
        HelpText = "The filename or directory to export the user defaults to", IsRequired = true)]
    public string OutputPath { get; set; } = string.Empty;

    [OptionParameter('f', "format", HelpText = "The plist format to export the domain to")]
    public PlistFormat PlistFormat { get; set; } = PlistFormat.Original;

    [OptionParameter('a', "all", HelpText = "Export all domains to the specified output directory")]
    public bool ExportAllDomains { get; set; }

    public Task<OptionsValidationResult> ValidateAsync()
    {
        var messages = new List<string>();

        switch (ExportAllDomains)
        {
            case false when string.IsNullOrWhiteSpace(DomainName):
                messages.Add("Domain name is required when exporting a single domain");
                break;
            case true when FileSys.File.Exists(OutputPath):
                messages.Add("Output must not be a file when exporting all domains");
                break;
        }

        return new Task<OptionsValidationResult>(() => new OptionsValidationResult(messages.Count == 0, messages));
    }
}
