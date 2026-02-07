namespace CreativeCoders.MacOS.HomeBrew;

public class BrewUpgradeException(string message, string errorOutput, int exitCode) : Exception(message)
{
    public string ErrorOutput { get; } = errorOutput;

    public int ExitCode { get; } = exitCode;
}