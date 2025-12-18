using System.IO.Abstractions;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.UserDefaults;

public class UserDefaultsExporter(
    IProcessExecutorBuilder processExecutorBuilder,
    IPlistConverter plistConverter) : IUserDefaultsExporter
{
    private readonly IPlistConverter _plistConverter = Ensure.NotNull(plistConverter);

    private readonly IProcessExecutor _exportDomain = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("defaults")
        .SetArguments(["export", "{{domain}}", "{{filePath}}"])
        .Build();

    public async Task ExportDomainAsync(string domainName, string filePath, PlistFormat format = PlistFormat.Original)
    {
        await _exportDomain.ExecuteAsync(new { domain = domainName, filePath }).ConfigureAwait(false);

        if (format != PlistFormat.Original)
        {
            await _plistConverter.ConvertFileAsync(filePath, filePath, format).ConfigureAwait(false);
        }
    }

    public Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath,
        PlistFormat format = PlistFormat.Original)
    {
        return domainNames.ForEachAsync(async domainName =>
            await ExportDomainAsync(domainName, Path.Combine(directoryPath, $"{domainName}.plist"), format)
                .ConfigureAwait(false));
    }
}
