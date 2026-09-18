using AwesomeAssertions;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class SynchronousProgressTests
{
    [Fact]
    public void Report_InvokesHandlerWithReportedValue()
    {
        // Arrange
        var received = 0;
        var sut = new SynchronousProgress<int>(x => received = x);

        // Act
        sut.Report(42);

        // Assert
        received.Should().Be(42);
    }

    /// <summary>
    /// Verifies the behaviour that distinguishes <see cref="SynchronousProgress{T}"/> from
    /// <see cref="Progress{T}"/>: the handler has already run once <c>Report</c> returns instead
    /// of being posted to a synchronization context or the thread pool.
    /// </summary>
    [Fact]
    public void Report_InvokesHandlerInlineOnCallingThread()
    {
        // Arrange
        var handlerThreadId = 0;
        var sut = new SynchronousProgress<int>(_ => handlerThreadId = Environment.CurrentManagedThreadId);

        // Act
        sut.Report(1);

        // Assert
        handlerThreadId.Should().Be(Environment.CurrentManagedThreadId);
    }

    [Fact]
    public void Report_WhenCalledMultipleTimes_PassesEveryValueInOrder()
    {
        // Arrange
        var received = new List<string>();
        IProgress<string> sut = new SynchronousProgress<string>(received.Add);

        // Act
        sut.Report("a");
        sut.Report("b");
        sut.Report("c");

        // Assert
        received.Should().Equal("a", "b", "c");
    }

    [Fact]
    public void Report_WhenValueIsNull_InvokesHandlerWithNull()
    {
        // Arrange
        var invoked = false;
        string? received = "initial";
        var sut = new SynchronousProgress<string?>(x =>
        {
            invoked = true;
            received = x;
        });

        // Act
        sut.Report(null);

        // Assert
        invoked.Should().BeTrue();
        received.Should().BeNull();
    }

    [Fact]
    public void Report_WhenHandlerThrows_ExceptionPropagatesToCaller()
    {
        // Arrange
        var sut = new SynchronousProgress<int>(_ => throw new InvalidOperationException("boom"));

        // Act
        var act = () => sut.Report(1);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("boom");
    }
}
