using System.IO.Abstractions;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.UserDefaults;

public class UserDefaultsExporter(
    IProcessExecutorBuilder processExecutorBuilder) : IUserDefaultsExporter
{
    private readonly IProcessExecutor _exportDomain = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("defaults")
        .SetArguments(["export", "{{domain}}", "{{filePath}}"])
        .Build();

    public Task ExportDomainAsync(string domainName, string filePath)
    {
        return _exportDomain.ExecuteAsync(new { domain = domainName, filePath });
    }

    public Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath)
    {
        return domainNames.ForEachAsync(async domainName =>
            await ExportDomainAsync(domainName, Path.Combine(directoryPath, $"{domainName}.plist"))
                .ConfigureAwait(false));
    }
}
