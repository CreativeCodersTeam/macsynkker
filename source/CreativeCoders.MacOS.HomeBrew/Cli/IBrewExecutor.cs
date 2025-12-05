using CreativeCoders.DependencyInjection;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;

namespace CreativeCoders.MacOS.HomeBrew.Cli;

public interface IBrewExecutor
{
    Task<bool> GetIsInstalledAsync();

    Task<BrewInstalledModel> GetInstalledSoftwareAsync();

    Task<BrewCaskModel[]> GetInstalledCasksAsync();

    Task<BrewFormulaModel[]> GetInstalledFormulasAsync();
}

public class BrewExecutor : IBrewExecutor
{
    private readonly IProcessExecutor<string> _brewGetVersionExecutor;

    private readonly IProcessExecutor<BrewInstalledModel> _brewSoftwareInstalledExecutor;

    public BrewExecutor(IObjectFactory objectFactory)
    {
        _brewGetVersionExecutor = objectFactory.GetInstance<IProcessExecutorBuilder<string>>()
            .SetFileName("brew")
            .SetArguments(["--version"])
            .ShouldThrowOnError()
            .Build();

        _brewSoftwareInstalledExecutor = objectFactory.GetInstance<IProcessExecutorBuilder<BrewInstalledModel>>()
            .SetFileName("brew")
            .SetArguments(["info", "--installed", "--json=v2"])
            .SetOutputParser<JsonOutputParser<BrewInstalledModel>>()
            .ShouldThrowOnError()
            .Build();
    }

    public async Task<bool> GetIsInstalledAsync()
    {
        return (await _brewGetVersionExecutor.ExecuteAsync().ConfigureAwait(false))?.StartsWith("Homebrew") == true;
    }

    public async Task<BrewInstalledModel> GetInstalledSoftwareAsync()
    {
        return (await _brewSoftwareInstalledExecutor.ExecuteAsync().ConfigureAwait(false)) ??
               throw new InvalidOperationException("Invalid brew installed software output");
    }

    public Task<BrewCaskModel[]> GetInstalledCasksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BrewFormulaModel[]> GetInstalledFormulasAsync()
    {
        throw new NotImplementedException();
    }
}
