using System.IO.Abstractions;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Core.IO;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.UserDefaults;

public class UserDefaultsImporter(
    IProcessExecutorBuilder processExecutorBuilder,
    IFileSystem fileSystem,
    IUserDefaultsEnumerator userDefaultsEnumerator) : IUserDefaultsImporter
{
    private readonly IProcessExecutor _importDomain = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("defaults")
        .SetArguments(["import", "{{domain}}", "{{filePath}}"])
        .Build();

    public Task ImportDomainAsync(string domainName, string filePath)
    {
        return _importDomain.ExecuteAsync(new { domain = domainName, filePath });
    }

    public async Task ImportAllDomainsAsync(string directoryPath, bool onlyImportExistingDomains = true)
    {
        if (onlyImportExistingDomains)
        {
            var domainNames = (await userDefaultsEnumerator.GetDomainNamesAsync().ConfigureAwait(false))
                .ToArray();

            await ImportAllDomainsAsync(directoryPath, domainName => domainNames.Contains(domainName))
                .ConfigureAwait(false);
        }
        else
        {
            await ImportAllDomainsAsync(directoryPath, domainName => true).ConfigureAwait(false);
        }
    }

    public Task ImportAllDomainsAsync(string directoryPath, Func<string, bool> domainNameFilter)
    {
        return fileSystem.Directory.EnumerateFiles(directoryPath, "*.plist")
            .Where(domainNameFilter)
            .ForEachAsync(async filePath =>
                await ImportDomainAsync(fileSystem.Path.GetFileNameWithoutExtension(filePath), filePath)
                    .ConfigureAwait(false)
            );
    }
}
