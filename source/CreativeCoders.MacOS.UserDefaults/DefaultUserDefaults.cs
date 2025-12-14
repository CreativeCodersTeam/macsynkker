using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;

namespace CreativeCoders.MacOS.UserDefaults;

public class DefaultUserDefaults(IProcessExecutorBuilder<string[]> processExecutorBuilder) : IUserDefaults
{
    private readonly IProcessExecutor<string[]> _executor = Ensure.NotNull(processExecutorBuilder)
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
        var domainNames = await _executor.ExecuteAsync().ConfigureAwait(false);

        return domainNames?.Select(x => new DefaultsDomain(x)) ?? [];
    }
}
