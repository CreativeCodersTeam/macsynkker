using CreativeCoders.Core;

namespace CreativeCoders.MacSynkker.Cli.Commands.HomeBrew;

public class TableColumnDef<T>(Func<T, object?> valueSelector, string? title = null, int? width = null)
{
    private readonly Func<T, object?> _valueSelector = Ensure.NotNull(valueSelector);

    public string GetValue(T item) => _valueSelector(item)?.ToString() ?? string.Empty;

    public string? Title { get; } = title;

    public int? Width { get; } = width;
}
