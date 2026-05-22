using System.Globalization;
using System.Text.RegularExpressions;

namespace CreativeCoders.MacOS.HomeBrew.Cleanup;

/// <summary>
/// Parses the human-readable output emitted by <c>brew cleanup --dry-run</c>
/// (e.g. <c>"Would remove: /path/to/file (1.2MB)"</c> and
/// <c>"This operation would free approximately 1.2GB of disk space."</c>).
/// </summary>
internal static partial class ReclaimableSpaceParser
{
    [GeneratedRegex(
        @"^\s*(?:Would remove|Removing):\s+(?<path>.+?)\s*\((?<value>\d+(?:\.\d+)?)\s*(?<unit>B|KB|MB|GB|TB|PB)\)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ItemRegex();

    [GeneratedRegex(
        @"would\s+free\s+approximately\s+(?<value>\d+(?:\.\d+)?)\s*(?<unit>B|KB|MB|GB|TB|PB)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex TotalRegex();

    /// <summary>
    /// Returns the total reclaimable size found in <paramref name="brewOutput"/> in bytes.
    /// Returns <c>0</c> when no size hint can be detected.
    /// </summary>
    public static long ParseBytes(string? brewOutput)
    {
        return Parse(brewOutput).TotalBytes;
    }

    /// <summary>
    /// Parses the full <c>brew cleanup --dry-run</c> output into a <see cref="ReclaimableSpace"/>
    /// containing the total reclaimable bytes and the list of individual entries.
    /// </summary>
    /// <param name="brewOutput">Raw stdout of <c>brew cleanup --dry-run</c>.</param>
    /// <returns>
    /// A <see cref="ReclaimableSpace"/>. When <paramref name="brewOutput"/> is null or
    /// whitespace, an empty result with <c>TotalBytes = 0</c> is returned. When the
    /// output contains a "would free approximately"-line, its size is used as
    /// <see cref="ReclaimableSpace.TotalBytes"/>; otherwise the sum of the parsed
    /// item sizes is used.
    /// </returns>
    public static ReclaimableSpace Parse(string? brewOutput)
    {
        if (string.IsNullOrWhiteSpace(brewOutput))
        {
            return new ReclaimableSpace(0, []);
        }

        var items = new List<ReclaimableItem>();
        long itemsSum = 0;

        foreach (var line in brewOutput.Split('\n'))
        {
            var match = ItemRegex().Match(line);

            if (!match.Success)
            {
                continue;
            }

            if (!TryGetBytes(match.Groups["value"].Value, match.Groups["unit"].Value, out var bytes))
            {
                continue;
            }

            items.Add(new ReclaimableItem(match.Groups["path"].Value.Trim(), bytes));
            itemsSum += bytes;
        }

        var totalMatch = TotalRegex().Match(brewOutput);

        long totalBytes;

        if (totalMatch.Success
            && TryGetBytes(totalMatch.Groups["value"].Value, totalMatch.Groups["unit"].Value, out var parsedTotal))
        {
            totalBytes = parsedTotal;
        }
        else
        {
            totalBytes = itemsSum;
        }

        return new ReclaimableSpace(totalBytes, items);
    }

    private static bool TryGetBytes(string value, string unit, out long bytes)
    {
        bytes = 0;

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            return false;
        }

        var multiplier = unit.ToUpperInvariant() switch
        {
            "B" => 1L,
            "KB" => 1024L,
            "MB" => 1024L * 1024L,
            "GB" => 1024L * 1024L * 1024L,
            "TB" => 1024L * 1024L * 1024L * 1024L,
            "PB" => 1024L * 1024L * 1024L * 1024L * 1024L,
            _ => 0L
        };

        if (multiplier == 0L)
        {
            return false;
        }

        bytes = (long)(number * multiplier);

        return true;
    }
}
