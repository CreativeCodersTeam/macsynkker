using CreativeCoders.Core;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.Core;

public interface IProgramLocator
{
    Task<string?> LocateProgramAsync(string programName);

    string? LocateProgram(string programName);
}

public class DefaultProgramLocator(IProcessExecutorBuilder<string> processExecutorBuilder) : IProgramLocator
{
    private readonly IProcessExecutor<string> _executor = Ensure.NotNull(processExecutorBuilder)
        .SetFileName("which")
        .Build();

    public Task<string?> LocateProgramAsync(string programName)
    {
        return _executor.ExecuteAsync([programName]);
    }

    public string? LocateProgram(string programName)
    {
        return _executor.Execute([programName]);
    }
}
