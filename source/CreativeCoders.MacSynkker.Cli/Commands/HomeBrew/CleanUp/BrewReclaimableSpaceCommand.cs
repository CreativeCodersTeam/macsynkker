using System.Globalization;
using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew.CleanUp;

/// <summary>Shows how much disk space <c>brew cleanup</c> would reclaim.</summary>
[UsedImplicitly]
[CliCommand([HomebrewCommandGroup.Name, CleanUpCommandGroup.Name + "-show"],
    Description = "Show disk space brew cleanup would reclaim (dry-run)")]
public class BrewReclaimableSpaceCommand(IBrewCleanup brewCleanup, IAnsiConsole ansiConsole)
    : ICliCommand<BrewReclaimableSpaceOptions>
{
    private static readonly string[] UnitSuffixes = ["B", "KB", "MB", "GB", "TB", "PB"];

    private readonly IBrewCleanup _brewCleanup = Ensure.NotNull(brewCleanup);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public async Task<CommandResult> ExecuteAsync(BrewReclaimableSpaceOptions options)
    {
        Ensure.NotNull(options);

        var brewOptions = options.ToBrewCleanupOptions();

        _ansiConsole.MarkupLine("Calculating reclaimable space ...");

        try
        {
            if (options.Details)
            {
                var details = await _brewCleanup.GetReclaimableSpaceDetailsAsync(brewOptions)
                    .ConfigureAwait(false);

                WriteDetails(details);
            }
            else
            {
                var bytes = await _brewCleanup.GetReclaimableSpaceAsync(brewOptions).ConfigureAwait(false);

                WriteTotal(bytes);
            }
        }
        catch (BrewCleanupFailedException e)
        {
            _ansiConsole.MarkupLine("[red]Failed to query reclaimable space[/]");
            _ansiConsole.WriteLine(e.ErrorOutput);
        }

        return CommandResult.Success;
    }

    private void WriteTotal(long bytes)
    {
        _ansiConsole.MarkupLine(
            $"Reclaimable space: [green]{FormatBytes(bytes)}[/] ({bytes:N0} bytes)");
    }

    private void WriteDetails(ReclaimableSpace details)
    {
        if (details.Items.Count == 0)
        {
            _ansiConsole.MarkupLine("[yellow]No reclaimable entries found.[/]");
        }
        else
        {
            var table = new Table()
                .AddColumn("Path")
                .AddColumn(new TableColumn("Size").RightAligned())
                .AddColumn(new TableColumn("Bytes").RightAligned());

            foreach (var item in details.Items)
            {
                table.AddRow(
                    Markup.Escape(item.Path),
                    FormatBytes(item.SizeInBytes),
                    item.SizeInBytes.ToString("N0", CultureInfo.InvariantCulture));
            }

            _ansiConsole.Write(table);
        }

        WriteTotal(details.TotalBytes);
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes <= 0)
        {
            return "0 B";
        }

        double value = bytes;
        var unit = 0;

        while (value >= 1024 && unit < UnitSuffixes.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return string.Create(CultureInfo.InvariantCulture, $"{value:0.##} {UnitSuffixes[unit]}");
    }
}
