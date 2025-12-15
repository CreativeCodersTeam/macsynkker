using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Core.IO;
using CreativeCoders.DependencyInjection;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;

namespace CreativeCoders.MacOS.UserDefaults;

public class DefaultUserDefaults(IObjectFactory objectFactory) : IUserDefaults
{
    private readonly IProcessExecutor<string[]> _getDefaultsDomains = Ensure.NotNull(objectFactory)
        .GetInstance<IProcessExecutorBuilder<string[]>>()
        .SetFileName("defaults")
        .SetArguments(["domains"])
        .SetOutputParser<SplitLinesOutputParser>(x =>
        {
            x.Separators = [","];
            x.SplitOptions = StringSplitOptions.RemoveEmptyEntries;
            x.TrimLines = true;
        })
        .Build();

    private readonly IProcessExecutor _exportDomain = Ensure.NotNull(objectFactory)
        .GetInstance<IProcessExecutorBuilder>()
        .SetFileName("defaults")
        .SetArguments(["export", "{{domain}}", "{{filePath}}"])
        .Build();

    private readonly IProcessExecutor _importDomain = Ensure.NotNull(objectFactory)
        .GetInstance<IProcessExecutorBuilder>()
        .SetFileName("defaults")
        .SetArguments(["import", "{{domain}}", "{{filePath}}"])
        .Build();

    public async Task<IEnumerable<DefaultsDomain>> GetDomainsAsync()
    {
        var domainNames = await _getDefaultsDomains.ExecuteAsync().ConfigureAwait(false);

        return domainNames?.Select(x => new DefaultsDomain(x)).ToArray() ?? [];
    }

    public Task ExportDomainAsync(string domainName, string filePath)
    {
        return _exportDomain.ExecuteAsync(new { domain = domainName, filePath });
    }

    public Task ImportDomainAsync(string domainName, string filePath)
    {
        return _importDomain.ExecuteAsync(new { domain = domainName, filePath });
    }

    public Task ExportDomainsAsync(IEnumerable<string> domainNames, string directoryPath)
    {
        return domainNames.ForEachAsync(async domainName =>
            await ExportDomainAsync(domainName, Path.Combine(directoryPath, $"{domainName}.plist"))
                .ConfigureAwait(false));
    }

    public Task ImportAllDomainsAsync(string directoryPath)
    {
        return FileSys.Directory.EnumerateFiles(directoryPath, "*.plist")
            .ForEachAsync(async filePath =>
                await ImportDomainAsync(Path.GetFileNameWithoutExtension(filePath), filePath)
                    .ConfigureAwait(false)
            );
    }
}
