using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;

namespace CreativeCoders.MacOS.HomeBrew;

public class BrewInstalledSoftware(IProcessExecutorBuilder<BrewInstalledModel> executorBuilder) : IBrewInstalledSoftware
{
    private readonly IProcessExecutor<BrewInstalledModel> _executor = Ensure.NotNull(executorBuilder)
        .SetFileName("brew")
        .SetArguments(["info", "--installed", "--json=v2"])
        .SetOutputParser<JsonOutputParser<BrewInstalledModel>>()
        .ShouldThrowOnError()
        .Build();

    public async Task<BrewInstalledModel> GetInstalledSoftwareAsync()
    {
        return await _executor.ExecuteAsync().ConfigureAwait(false) ?? new BrewInstalledModel();
    }
}
