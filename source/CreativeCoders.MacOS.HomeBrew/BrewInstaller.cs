using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Import;
using CreativeCoders.ProcessUtils.Execution;

namespace CreativeCoders.MacOS.HomeBrew;

/// <summary>
/// Default <see cref="IBrewInstaller"/> implementation. Uses <see cref="IProcessExecutor{TResult}"/>
/// instances (analogous to <c>BrewUpgrader</c>) to invoke the <c>brew</c> CLI.
/// </summary>
public class BrewInstaller : IBrewInstaller
{
    private readonly IProcessExecutor<string> _tapExecutor;

    private readonly IProcessExecutor<string> _installFormulaExecutor;

    private readonly IProcessExecutor<string> _installCaskExecutor;

    public BrewInstaller(IProcessExecutorBuilder<string> processExecutorBuilder)
    {
        Ensure.NotNull(processExecutorBuilder);

        _tapExecutor = processExecutorBuilder
            .SetFileName("brew")
            .SetArguments(["tap", "{{tap}}"])
            .ShouldThrowOnError()
            .Build();

        _installFormulaExecutor = processExecutorBuilder
            .SetFileName("brew")
            .SetArguments(["install", "{{name}}"])
            .ShouldThrowOnError()
            .Build();

        _installCaskExecutor = processExecutorBuilder
            .SetFileName("brew")
            .SetArguments(["install", "--cask", "{{token}}"])
            .ShouldThrowOnError()
            .Build();
    }

    public async Task TapAsync(string tap)
    {
        Ensure.IsNotNullOrWhitespace(tap);

        try
        {
            await _tapExecutor.ExecuteAsync(new { tap }).ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewInstallFailedException(BrewInstallTargetKind.Tap, tap, e.ErrorOutput, e.ExitCode);
        }
    }

    public async Task InstallFormulaAsync(string name)
    {
        Ensure.IsNotNullOrWhitespace(name);

        try
        {
            await _installFormulaExecutor.ExecuteAsync(new { name }).ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewInstallFailedException(BrewInstallTargetKind.Formula, name, e.ErrorOutput, e.ExitCode);
        }
    }

    public async Task InstallCaskAsync(string token)
    {
        Ensure.IsNotNullOrWhitespace(token);

        try
        {
            await _installCaskExecutor.ExecuteAsync(new { token }).ConfigureAwait(false);
        }
        catch (ProcessExecutionFailedException e)
        {
            throw new BrewInstallFailedException(BrewInstallTargetKind.Cask, token, e.ErrorOutput, e.ExitCode);
        }
    }
}
