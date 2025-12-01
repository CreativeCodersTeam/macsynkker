using CreativeCoders.Core;

namespace CreativeCoders.MacOS.Core.AppleScripting;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
///     Executes AppleScript code on macOS by invoking the system <c>osascript</c> command.
/// </summary>
/// <remarks>
///     This implementation shells out to the native <c>osascript</c> tool which ships with macOS. No external
///     dependencies are required. On non-macOS platforms a <see cref="PlatformNotSupportedException"/> is thrown.
/// </remarks>
public sealed class AppleScript : IAppleScript
{
    /// <inheritdoc />
    public async Task ExecuteAsync(string[] scriptCode, bool createTemporaryFile = false)
    {
        CheckForMacOS();

        Ensure.IsNotNullOrEmpty(scriptCode);

        await RunOsascriptAsync(scriptCode, captureOutput: false, createTemporaryFile).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ExecuteFileAsync(string filePath)
    {
        CheckForMacOS();

        if (filePath is null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"AppleScript file not found: {filePath}", filePath);
        }

        await RunOsascriptWithFileAsync(filePath, captureOutput: false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExecuteWithResultAsync(string[] scriptCode, bool createTemporaryFile = false)
    {
        CheckForMacOS();

        if (scriptCode is null)
        {
            throw new ArgumentNullException(nameof(scriptCode));
        }

        if (scriptCode.Length == 0)
        {
            throw new ArgumentException("AppleScript code must contain at least one line.", nameof(scriptCode));
        }

        return await RunOsascriptAsync(scriptCode, captureOutput: true, createTemporaryFile).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExecuteFileWithResultAsync(string filePath)
    {
        CheckForMacOS();

        if (filePath is null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"AppleScript file not found: {filePath}", filePath);
        }

        return await RunOsascriptWithFileAsync(filePath, captureOutput: true).ConfigureAwait(false);
    }

    private static void CheckForMacOS()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            throw new PlatformNotSupportedException("AppleScript can only be executed on macOS.");
        }
    }

    private static async Task<string> RunOsascriptAsync(
        string[] scriptCode,
        bool captureOutput,
        bool createTemporaryFile)
    {
        if (createTemporaryFile)
        {
            // Create a temporary file to store the AppleScript and execute it with osascript
            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                await File.WriteAllLinesAsync(tempFile, scriptCode, Encoding.UTF8).ConfigureAwait(false);
                return await RunOsascriptWithFileAsync(tempFile, captureOutput).ConfigureAwait(false);
            }
            finally
            {
                // Clean up the temporary file; ignoring IO errors on deletion by design.
                try
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch
                {
                    // Deliberately swallow exceptions here to avoid masking the original error from osascript.
                }
            }
        }

        // Build arguments: one -e per line to avoid shell quoting issues.
        var argsBuilder = new StringBuilder();
        foreach (var line in scriptCode)
        {
            if (line is null)
            {
                continue; // Skip null lines to be resilient; AppleScript ignores blank lines.
            }

            argsBuilder.Append(" -e ");
            argsBuilder.Append(EscapeForProcessArg(line));
        }

        return await StartProcessAsync("/usr/bin/osascript", argsBuilder.ToString(), captureOutput).ConfigureAwait(false);
    }

    private static async Task<string> RunOsascriptWithFileAsync(string filePath, bool captureOutput)
    {
        var escapedFile = EscapeForProcessArg(filePath);
        return await StartProcessAsync("/usr/bin/osascript", $" {escapedFile}", captureOutput).ConfigureAwait(false);
    }

    private static async Task<string> StartProcessAsync(string fileName, string arguments, bool captureOutput)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = captureOutput,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = new Process();
        proc.StartInfo = psi;
        proc.EnableRaisingEvents = false;

        return string.Empty;
    }

    private static string EscapeForProcessArg(string value)
    {
        // Surround with quotes and escape inner quotes. We avoid any shell expansion by passing arguments directly.
        var v = value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        return $"\"{v}\"";
    }
}
