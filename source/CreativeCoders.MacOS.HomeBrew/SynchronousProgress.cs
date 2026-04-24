using CreativeCoders.Core;

namespace CreativeCoders.MacOS.HomeBrew;

/// <summary>
/// An <see cref="IProgress{T}"/> implementation that invokes the callback synchronously on the
/// calling thread. Unlike <see cref="Progress{T}"/>, which posts to the captured
/// <see cref="SynchronizationContext"/> (or the <see cref="ThreadPool"/> when none exists),
/// this class guarantees that the handler runs inline before <see cref="Report"/> returns.
/// </summary>
public sealed class SynchronousProgress<T>(Action<T> handler) : IProgress<T>
{
    private readonly Action<T> _handler = Ensure.NotNull(handler);

    /// <inheritdoc />
    public void Report(T value) => _handler(value);
}
