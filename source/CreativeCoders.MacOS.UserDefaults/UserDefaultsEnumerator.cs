using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;

namespace CreativeCoders.MacOS.UserDefaults;

public class UserDefaultsEnumerator(IProcessExecutorBuilder<string[]> processExecutorBuilder) : IUserDefaultsEnumerator
{
    private readonly IProcessExecutor<string[]> _getDefaultsDomains = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("defaults")
        .SetArguments(["domains"])
        .SetOutputParser<SplitLinesOutputParser>(x =>
        {
            x.Separators = [","];
            x.SplitOptions = StringSplitOptions.RemoveEmptyEntries;
            x.TrimLines = true;
        })
        .Build();

    public async Task<IEnumerable<DefaultsDomain>> GetDomainsAsync()
    {
        var domainNames = await _getDefaultsDomains.ExecuteAsync().ConfigureAwait(false);

        return domainNames?.Select(x => new DefaultsDomain(x)) ?? [];
    }

    public async Task<IEnumerable<string>> GetDomainNamesAsync()
    {
        return await _getDefaultsDomains.ExecuteAsync().ConfigureAwait(false) ?? [];
    }
}
