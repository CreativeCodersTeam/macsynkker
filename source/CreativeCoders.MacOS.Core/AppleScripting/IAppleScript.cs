namespace CreativeCoders.MacOS.Core.AppleScripting;

public interface IAppleScript
{
    Task ExecuteAsync(string[] scriptCode, bool createTemporaryFile = false);

    Task ExecuteFileAsync(string filePath);

    Task<string> ExecuteWithResultAsync(string[] scriptCode, bool createTemporaryFile = false);

    Task<string> ExecuteFileWithResultAsync(string filePath);
}
