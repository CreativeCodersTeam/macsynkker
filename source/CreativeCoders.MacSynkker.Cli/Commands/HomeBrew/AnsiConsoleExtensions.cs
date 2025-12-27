using Spectre.Console;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

public static class AnsiConsoleExtensions
{
    public static void PrintTable<T>(this IAnsiConsole ansiConsole, IEnumerable<T> items,
        TableColumnDef<T>[] columns, Action<Table>? configureTable = null)
    {
        var table = new Table
        {
            ShowHeaders = false,
            Border = TableBorder.None
        };

        foreach (var tableColumnDef in columns)
        {
            table.AddColumn(tableColumnDef.Title ?? string.Empty, x => { x.Width = tableColumnDef.Width; });

            if (!string.IsNullOrWhiteSpace(tableColumnDef.Title))
            {
                table.ShowHeaders = true;
            }
        }

        configureTable?.Invoke(table);

        if (table.ShowHeaders)
        {
            table.AddRow(columns.Select(x => new string('=', x.Title?.Length ?? 0)).ToArray());
        }

        foreach (var item in items)
        {
            table.AddRow(columns.Select(x => x.GetValue(item)).ToArray());
        }

        ansiConsole.Write(table);
    }
}
