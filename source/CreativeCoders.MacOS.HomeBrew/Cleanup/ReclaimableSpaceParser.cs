using System.Globalization;
using System.Text.RegularExpressions;

namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Parses the human-readable size hint emitted by <c>brew cleanup --dry-run</c>
/// (e.g. <c>"This operation would free approximately 1.2GB of disk space."</c>).
/// </summary>
internal static partial class ReclaimableSpaceParser
{
    [GeneratedRegex(
        @"(?<value>\d+(?:\.\d+)?)\s*(?<unit>B|KB|MB|GB|TB|PB)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SizeRegex();

    /// <summary>
    /// Returns the largest size found in <paramref name="brewOutput"/> converted to bytes.
    /// Returns <c>0</c> when no size hint can be detected.
    /// </summary>
    public static long ParseBytes(string? brewOutput)
    {
        if (string.IsNullOrWhiteSpace(brewOutput))
        {
            return 0;
        }

        long max = 0;

        foreach (var matchGroups in SizeRegex().Matches(brewOutput).Select(match => match.Groups))
        {
            if (!double.TryParse(matchGroups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out var value))
            {
                continue;
            }

            var unit = matchGroups["unit"].Value.ToUpperInvariant();
            var multiplier = unit switch
            {
                "B" => 1L,
                "KB" => 1024L,
                "MB" => 1024L * 1024L,
                "GB" => 1024L * 1024L * 1024L,
                "TB" => 1024L * 1024L * 1024L * 1024L,
                "PB" => 1024L * 1024L * 1024L * 1024L * 1024L,
                _ => 0L
            };

            var bytes = (long)(value * multiplier);

            if (bytes > max)
            {
                max = bytes;
            }
        }

        return max;
    }
}
